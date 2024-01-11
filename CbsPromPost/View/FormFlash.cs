using CbsPromPost.Model;
using CbsPromPost.Other;
using CbsPromPost.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using Application = System.Windows.Forms.Application;
using Size = System.Drawing.Size;
using Timer = System.Windows.Forms.Timer;

namespace CbsPromPost.View;

public sealed partial class FormFlash : Form
{
    private readonly Timer _timer = new();

    private readonly WebCam _webCam;
    private readonly SharpDxMain _dx;
    private double _timePausedMinutes;
    private double _timeMinWorkMinutes;
    private DateTime _startTime;
    private DateTime _lastPaused;
    private DateTime _paused;
    private int _counts;
    private FormDroneConfig _formDrone;

    private int _counterClick;
    private DateTime _counterClickTime;
    private readonly SerialScanner _scanner;
    private readonly SerialBetaflight _betaflight;

    public FormFlash()
    {
        InitializeComponent();

        _scanner = new SerialScanner(Core.IoC.Services.GetRequiredService<ILogger<SerialScanner>>());
        _betaflight = new SerialBetaflight(Core.IoC.Services.GetRequiredService<ILogger<SerialBetaflight>>());

        buttonPause.Enabled = false;

        _formDrone = new FormDroneConfig(_betaflight);
        labelName.Text = $@"ПОСТ №{Core.Config.PostNumber:0}";
        _counts = 0;
        _timer.Interval = 100;
        foreach (var f in Core.Config.Firmwares)
        {
            comboBoxFirmware.Items.Add(f.Name);
        }
        comboBoxFirmware.SelectedIndex = 0;

        _webCam = new WebCam();
        _dx = new SharpDxMain(pictureBoxMain, -1);
        pictureBoxMain.SizeMode = PictureBoxSizeMode.StretchImage;

        var works = Core.IoC.Services.GetRequiredService<Works>();
        var work = works.Get(Core.Config.Type);
        labelWork.Text = work.Name;

        Text = $@"[КБ ЦБС] ПОСТ №{Core.Config.PostNumber:0}, прошивка и тестирование готовых изделий";
        Icon = EmbeddedResources.Get<Icon>("Sprites._user_change.ico");
        richTextBoxMain.Text = string.Empty;

        labelFpl.Text = Core.Config.Firmwares.Find(x => x.Name.Equals(comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]!.ToString()))!.FileFpl;
        labelHex.Text = Core.Config.Firmwares.Find(x => x.Name.Equals(comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]!.ToString()))!.FileBin;
        labelComScanner.Text = $@"ШК СКАНЕР [{Core.Config.ComScanner}]";
        labelComBeta.Text = $@"BetaFlight [{Core.Config.ComBeta}]";
        labelDfu.Text = $@"BetaFlight DFU mode [{Core.Config.UsbDfuVid}:{Core.Config.UsbDfuPid}]";

        var menu = new ContextMenuStrip();
        menu.Items.Add("ОЧИСТИТЬ", null, OnRichTextBoxClear);
        richTextBoxMain.ContextMenuStrip = menu;

        buttonPause.Click += ButtonPauseClick;
        buttonFinish.Click += ButtonFinishClick;

        _timer.Tick += TimerTick;
        _scanner.OnReadValue += ComReadString;
        labelName.MouseClick += NameClick;
        textBoxCli.KeyDown += CliKeyDown;
        Closed += OnClose;
        Shown += FormShown;
        buttonImpulseRC.Click += ButtonImpulseRc;
        buttonReset.Click += ButtonResetClick;
        buttonLoadBinImage.Click += ButtonImageBinReadClick;
        buttonLoadFpl.Click += ButtonFplReadClick;
        buttonClearFlash.Click += ButtonClearFlashClick;
        buttonWriteBinImage.Click += ButtonWriteRawImageClick;
        buttonWriteFpl.Click += ButtonWriteFplClick;
        buttonFullFlash.Click += ButtonFullFlashClick;
        buttonDroneConfig.Click += ButtonDroneConfigClick;
        comboBoxFirmware.SelectedValueChanged += FlashChanged;
        buttonBadDrone.Click += BadDrone;
        buttonOkDrone.Click += OkDrone;
    }

    private async void OkDrone(object? sender, EventArgs e)
    {
        var answ = await Core.IoC.Services.GetRequiredService<Station>().FinishBodyAsync(labelDroneId.Text, default);
        if (answ.Equals(string.Empty))
        {
            _counts++;
            new FormInfo(@"РАБОТА ЗАВЕРШЕНА", Color.LightGreen, Color.DarkGreen, 3000, new Size(600, 400))
                .Show(this);
            labelDroneId.Text = string.Empty; // Финиш работы
            if (!_formDrone.Visible) return;
            _formDrone.Close();
            _formDrone.Dispose();
            return;
        }

        new FormInfo(@$"{answ}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400)).Show(this);
    }

    private async void BadDrone(object? sender, EventArgs e)
    {
        // КОСТЫЛЬ
        var answ = await Core.IoC.Services.GetRequiredService<Station>().FinishBodyAsync(labelDroneId.Text, default);
        //if (answ.Equals(string.Empty))
        //{
        new FormInfo(@"ПЕРЕВЕДЕНО В БРАК", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400))
            .Show(this);
        labelDroneId.Text = string.Empty; // Финиш работы
        if (!_formDrone.Visible) return;
        _formDrone.Close();
        _formDrone.Dispose();
        //    return;
        //}
        //new FormInfo(@$"{answ}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400)).Show(this);
    }

    private void FlashChanged(object? sender, EventArgs e)
    {
        labelFpl.Text = Core.Config.Firmwares.Find(x => x.Name.Equals(comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]!.ToString()))!.FileFpl;
        labelHex.Text = Core.Config.Firmwares.Find(x => x.Name.Equals(comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]!.ToString()))!.FileBin;
    }

    private void ButtonDroneConfigClick(object? sender, EventArgs e)
    {
        if (_formDrone.Visible)
        {
            _formDrone.Activate();
            return;
        }

        if (_formDrone.IsDisposed) _formDrone = new FormDroneConfig(_betaflight);
        _formDrone.Show();
    }

    private async void ButtonFullFlashClick(object? sender, EventArgs e)
    {
        if (_formDrone.Visible)
        {
            _formDrone.Close();
            _formDrone.Dispose();
        }

        richTextBoxMain.Clear();
        richTextBoxMain.AppendText("СТАНДАРТИЗАЦИЯ ИЗДЕЛИЯ\r\n");
        richTextBoxMain.ScrollToCaret();

        var start = DateTime.Now;
        var ret = await FullFlash();
        if (ret < 0)
        {
            richTextBoxMain.SelectionBackColor = Color.LightPink;
            richTextBoxMain.AppendText("СТАНДАРТИЗАЦИЯ НЕ УДАЛАСЬ!!!!\r\n");
            richTextBoxMain.SelectionBackColor = Color.White;
            richTextBoxMain.ScrollToCaret();
        }
        else
        {
            richTextBoxMain.SelectionBackColor = Color.LightGreen;
            richTextBoxMain.AppendText(
                $"СТАНДАРТИЗАЦИЯ УСПЕШНА, затрачено {(DateTime.Now - start).TotalSeconds:0} сек.\r\n");
            richTextBoxMain.SelectionBackColor = Color.White;
            richTextBoxMain.ScrollToCaret();
        }
    }

    private async Task<int> FullFlash()
    {
        if (!_betaflight.IsAlive())
        {
            richTextBoxMain.SelectionBackColor = Color.LightPink;
            richTextBoxMain.AppendText("Контроллер не в режиме MSP/CLI!\r\n");
            richTextBoxMain.SelectionBackColor = Color.White;
            richTextBoxMain.ScrollToCaret();
            return -1;
        }

        var pb = Application.StartupPath + "DB\\_HEX\\" + Core.Config.Firmwares.Find(x => x.Name.Equals(comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]!.ToString()))!.FileBin;
        if (!File.Exists(pb)) return -2;
        var dataBin = await File.ReadAllBytesAsync(pb);
        var pf = Application.StartupPath + "DB\\_HEX\\" + Core.Config.Firmwares.Find(x => x.Name.Equals(comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]!.ToString()))!.FileFpl;
        if (!File.Exists(pf)) return -3;
        var dataFpl = await File.ReadAllLinesAsync(pf);

        // Переводим в CLI
        await WriteToCliAsync("#\r\n");
        await Task.Delay(1000);
        // Переводим в DFU
        await WriteToCliAsync("bl\r\n");
        await Task.Delay(5000);
        if (!_betaflight.IsAliveDfu()) return -4;
        if (await WriteBinAsync(dataBin) == false) return -5;

        // Ожидаем перезагрузки
        await Task.Delay(5000);
        if (await WriteFplAsync(dataFpl) == false) return -6;
        return 0;
    }

    private async void ButtonWriteFplClick(object? sender, EventArgs e)
    {
        if (_formDrone.Visible)
        {
            _formDrone.Close();
            _formDrone.Dispose();
        }

        if (!_betaflight.IsAlive())
        {
            richTextBoxMain.SelectionBackColor = Color.LightPink;
            richTextBoxMain.AppendText("Контроллер не в режиме MSP/CLI!\r\n");
            richTextBoxMain.SelectionBackColor = Color.White;
            richTextBoxMain.ScrollToCaret();
            return;
        }
        var fw = new OpenFileDialog { Title = @"Выберите FPL лист", Filter = @"(*.txt)|*.txt" };
        if (fw.ShowDialog(this) != DialogResult.OK) return;
        var data = await File.ReadAllLinesAsync(fw.FileName);
        await WriteFplAsync(data);
    }

    private async Task<bool> WriteFplAsync(IReadOnlyCollection<string> data)
    {
        return await Task.Run(async () =>
        {
            return await Invoke(async () =>
            {
                await WriteToCliAsync("#\r\n");
                await Task.Delay(1000);

                richTextBoxMain.AppendText($"CLI: ЗАПИСЬ FPL ЛИСТА, файл fpl [{data.Count:0} строк]\r\n");
                if (labelDroneId.Text.Equals(string.Empty)) labelDroneId.Text = @"TT000000";
                var ok = true;
                var containName = false;
                foreach (var s in data)
                {
                    if (s.Equals(string.Empty)) continue;
                    if (s[0].Equals('#')) continue;
                    var sw = s;
                    if (sw.Contains("set name = "))
                    {
                        containName = true;
                        sw = $"set name = SUDVT40 {labelDroneId.Text}";
                    }

                    var ret = await WriteToCliAsync(sw, 0);
                    if (!ret.Contains("ERROR")) continue;
                    ok = false;
                    break;
                }

                if (ok && !containName)
                {
                    var ret = await WriteToCliAsync($"set name = SUDVT40 {labelDroneId.Text}", 0);
                    ok = !ret.Contains("ERROR");
                }

                if (labelDroneId.Text.Equals(@"TT000000")) labelDroneId.Text = string.Empty;

                richTextBoxMain.AppendText(ok ? "CLI: УСПЕХ\r\n" : "CLI: ОШИБКА!!!\r\n");
                if (ok) await WriteToCliAsync("save", 100);
                return ok;
            });
        });
    }

    private async void ButtonWriteRawImageClick(object? sender, EventArgs e)
    {
        if (_formDrone.Visible)
        {
            _formDrone.Close();
            _formDrone.Dispose();
        }

        if (!_betaflight.IsAliveDfu())
        {
            richTextBoxMain.SelectionBackColor = Color.LightPink;
            richTextBoxMain.AppendText("Контроллер не в режиме DFU!\r\n");
            richTextBoxMain.SelectionBackColor = Color.White;
            richTextBoxMain.ScrollToCaret();
            return;
        }

        var fw = new OpenFileDialog { Title = @"Выберите RAW HEX", Filter = @"(*.bin)|*.bin" };
        if (fw.ShowDialog(this) != DialogResult.OK) return;
        var data = await File.ReadAllBytesAsync(fw.FileName);
        if (data.Length > SerialBetaflight.DfuFlashSize)
        {
            MessageBox.Show(
                @$"СЛИШКОМ БОЛЬШОЙ РАЗМЕР ПРОШИВКИ!         [file:{data.Length:0} байт] > [flash size: {SerialBetaflight.DfuFlashSize:0} байт]",
                @"ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        await WriteBinAsync(data);
    }

    private async Task<bool> WriteBinAsync(byte[] data)
    {
        return await Task.Run(async () =>
        {
            return await Invoke(async () =>
            {
                richTextBoxMain.AppendText(
                    $"DFU: ЗАПИСЬ ПРОШИКИ, ОБЛАСТЬ 0x{SerialBetaflight.DfuStartAddress:x8} - 0x{SerialBetaflight.DfuStartAddress + SerialBetaflight.DfuFlashSize:x8} файл hex [{data.Length:0} байт]\r\n");
                var res = await _betaflight.DfuRawBinWrite(data, 60000);
                await _betaflight.DfuExit();
                richTextBoxMain.AppendText(res >= 0 ? "DFU: УСПЕХ\r\n" : "DFU: ОШИБКА!!!\r\n");
                richTextBoxMain.ScrollToCaret();
                return res >= 0;
            });
        });
    }

    private async void ButtonClearFlashClick(object? sender, EventArgs e)
    {
        if (_formDrone.Visible)
        {
            _formDrone.Close();
            _formDrone.Dispose();
        }

        if (!_betaflight.IsAliveDfu())
        {
            richTextBoxMain.SelectionBackColor = Color.LightPink;
            richTextBoxMain.AppendText("Контроллер не в режиме DFU!\r\n");
            richTextBoxMain.SelectionBackColor = Color.White;
            richTextBoxMain.ScrollToCaret();
            return;
        }
        richTextBoxMain.AppendText(
            $"DFU: ОЧИСТКА ПРОШИКИ, ОБЛАСТЬ 0x{SerialBetaflight.DfuStartAddress:x8} - 0x{SerialBetaflight.DfuStartAddress + SerialBetaflight.DfuFlashSize:x8} [{SerialBetaflight.DfuFlashSize:0} байт]\r\n");
        var res = await _betaflight.DfuMassErase(60000);
        richTextBoxMain.AppendText(res >= 0 ? "DFU: УСПЕХ\r\n" : "DFU: ОШИБКА!!!\r\n");
    }

    private async void ButtonFplReadClick(object? sender, EventArgs e)
    {
        if (_formDrone.Visible)
        {
            _formDrone.Close();
            _formDrone.Dispose();
        }

        if (!_betaflight.IsAlive())
        {
            richTextBoxMain.SelectionBackColor = Color.LightPink;
            richTextBoxMain.AppendText("Контроллер не в режиме MSP/CLI!\r\n");
            richTextBoxMain.SelectionBackColor = Color.White;
            richTextBoxMain.ScrollToCaret();
            return;
        }

        richTextBoxMain.AppendText("CLI: СЧИТЫВАНИЕ FPL\r\n");
        await _betaflight.CliWrite("#\r\n");
        await Task.Delay(2000);
        var values = await _betaflight.CliWrite("dump\r\n");
        richTextBoxMain.AppendText(values.Count > 0 ? "CLI: УСПЕХ\r\n" : "CLI: ОШИБКА!!!\r\n");
        if (values.Count <= 0) return;
        var res = string.Join(string.Empty, values);
        var fd = new SaveFileDialog { Title = @"FPL", FileName = "_fpl_betafly.txt" };
        fd.ShowDialog(this);
        await File.WriteAllTextAsync(fd.FileName, res.Replace("dump\r\n", string.Empty));
    }

    private async void ButtonImageBinReadClick(object? sender, EventArgs e)
    {
        if (_formDrone.Visible)
        {
            _formDrone.Close();
            _formDrone.Dispose();
        }

        if (!_betaflight.IsAliveDfu())
        {
            richTextBoxMain.SelectionBackColor = Color.LightPink;
            richTextBoxMain.AppendText("Контроллер не в режиме DFU!\r\n");
            richTextBoxMain.SelectionBackColor = Color.White;
            richTextBoxMain.ScrollToCaret();
            return;
        }

        richTextBoxMain.AppendText(
            $"DFU: СЧИТЫВАНИЕ ПРОШИВКИ, ОБЛАСТЬ 0x{SerialBetaflight.DfuStartAddress:x8} - 0x{SerialBetaflight.DfuStartAddress + SerialBetaflight.DfuFlashSize:x8} [{SerialBetaflight.DfuFlashSize:0} байт]\r\n");
        var image = await _betaflight.DfuRawBinReadAll();
        richTextBoxMain.AppendText(image.Length > 0 ? "DFU: УСПЕХ\r\n" : "DFU: ОШИБКА!!!\r\n");
        if (image.Length <= 0) return;

        var fd = new SaveFileDialog { Title = @"RAW HEX", FileName = "_raw_betafly.bin" };
        fd.ShowDialog(this);
        await File.WriteAllBytesAsync(fd.FileName, image);
    }

    private void ButtonImpulseRc(object? sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Process.Start($"{Application.StartupPath}DB\\ImpulseRC_Driver_Fixer.exe");
        }
        catch (Exception ex)
        {
            new FormInfo($"ОШИБКА ЗАПУСКА\r\n[{ex.Message}]", Color.LightPink, Color.DarkRed, 3000, new Size(1000, 800)).Show(this);
        }
    }

    private void OnRichTextBoxClear(object? sender, EventArgs e)
    {
        richTextBoxMain.Text = string.Empty;
    }

    private async void ButtonResetClick(object? sender, EventArgs e)
    {
        if (_betaflight.IsAliveDfu())
        {
            richTextBoxMain.AppendText(string.Join(string.Empty, await _betaflight.DfuExit()));
            richTextBoxMain.ScrollToCaret();
            return;
        }

        await WriteToCliAsync("#");
        await Task.Delay(500);
        await WriteToCliAsync("exit");
    }

    private void FormShown(object? sender, EventArgs e)
    {
        _scanner.StartAsync(Core.Config.ComScanner);
        _ = _betaflight.StartAsync(Core.Config.ComBeta);
        _ = _betaflight.StartUsbAsync(int.Parse(Core.Config.UsbDfuVid, System.Globalization.NumberStyles.HexNumber), int.Parse(Core.Config.UsbDfuPid, System.Globalization.NumberStyles.HexNumber));

        _webCam.StartAsync(-1);
        _webCam.OnNewVideoFrame += NewFrame;

        var s = Core.IoC.Services.GetRequiredService<Station>();
        labelUser.Text = s.User.Name;
        if (!s.User.Name.Equals(string.Empty))
        {
            labelTime.Text = (DateTime.Now - s.WorkStart).TotalSeconds.ToSecTime();
            _startTime = s.WorkStart;
            _lastPaused = s.WorkStart;
        }
        else
        {
            _startTime = DateTime.Now;
            _lastPaused = DateTime.Now;
        }
        _paused = DateTime.MinValue;
        _timer.Start();
    }

    private void ProgressChange(int value)
    {
        progressBarMain.Value = Math.Min(Math.Max(progressBarMain.Minimum, value), progressBarMain.Maximum);
    }

    private async void CliKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        var txt = textBoxCli.Text;
        textBoxCli.Text = string.Empty;
        await WriteToCliAsync(txt);
    }

    private void ComReadString(string com, string text)
    {
        Invoke(async () =>
        {
            if (labelUser.Text.Equals(string.Empty)) return;

            var notOk = text.Length != 8;
            if (!notOk && text[..2] != "TT") notOk = true;
            if (!notOk && !long.TryParse(text[2..5], out _)) notOk = true;
            if (!notOk && !long.TryParse(text[5..8], out _)) notOk = true;

            if (notOk)
            {
                new FormInfo(@$"НЕИЗВЕСТНЫЙ ШК: {text}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400))
                    .Show(this);
                return;
            }

            if (labelDroneId.Text.Equals(string.Empty)) // Это первичное сканирование
            {
                labelDroneId.Text = text;
                return;
            }

            if (!labelDroneId.Text.Equals(text))
            {
                new FormInfo($"НЕ ВЕРНЫЙ ШК: [{text}]\r\nОЖИДАЕМ [{labelDroneId.Text}]", Color.LightPink, Color.DarkRed,
                    3000, new Size(600, 400)).Show(this);
                return;
            }

            var answ = await Core.IoC.Services.GetRequiredService<Station>().FinishBodyAsync(labelDroneId.Text, default);
            if (answ.Equals(string.Empty))
            {
                _counts++;
                new FormInfo(@"РАБОТА ЗАВЕРШЕНА", Color.LightGreen, Color.DarkGreen, 3000, new Size(600, 400))
                    .Show(this);
                labelDroneId.Text = string.Empty; // Финиш работы
                if (!_formDrone.Visible) return;
                _formDrone.Close();
                _formDrone.Dispose();
                return;
            }

            new FormInfo(@$"{answ}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400)).Show(this);
        });
    }

    private void NewFrame(Mat mat)
    {
        if (mat.Empty()) return;
        _dx.FrameUpdateAsync(mat);
        if (_formDrone.Visible) _formDrone.UpdateFrame(mat);
        //if (labelDroneId.Text.Equals(string.Empty)) return;
        //Cv2.ImWrite($"CAPTURE\\_{DateTime.Now.Ticks:0}.jpg", mat);
    }

    private void OnClose(object? sender, EventArgs e)
    {
        _webCam.OnNewVideoFrame -= NewFrame;
        _webCam.Dispose();
        _dx.Dispose();
        _timer.Stop();
    }

    private void NameClick(object? sender, MouseEventArgs e)
    {
        _counterClick++;
        _counterClickTime = DateTime.Now;
        if (_counterClick <= 6) return;

        new FormSettings().ShowDialog(this);
    }

    private async void TimerTick(object? sender, EventArgs e)
    {
        if ((DateTime.Now - _counterClickTime).TotalMilliseconds > 1000) _counterClick = 0;

        labelTimer.Text = DateTime.Now.ToString("HH:mm:ss.fff");
        ProgressChange(_betaflight.ProgressValue);

        var works = Core.IoC.Services.GetRequiredService<Works>();
        var work = works.Get(Core.Config.Type);

        labelComScanner.BackColor = _scanner.IsAlive() ? Color.LightGreen : Color.LightPink;
        labelComBeta.BackColor = _betaflight.IsAlive() ? Color.LightGreen : Color.LightPink;
        labelDfu.BackColor = _betaflight.IsAliveDfu() ? Color.LightGreen : Color.LightPink;

        var s = Core.IoC.Services.GetRequiredService<Station>();
        if (s.User.Name.Equals(string.Empty) && !labelUser.Text.Equals(string.Empty) | s.User.Name.Equals(string.Empty) && labelUser.Text.Equals(string.Empty)) // Выключение работы
        {
            if (!Core.Config.TestMode) labelDroneId.Text = string.Empty;
            labelUser.Text = string.Empty;
            groupBoxButtons.Enabled = false;
            buttonPause.Enabled = false;
            buttonFinish.Enabled = false;
            _startTime = DateTime.Now;
            _lastPaused = DateTime.Now;
            _paused = DateTime.MinValue;
            labelTime.Text = @"РАБОТА НЕ ВЕДЕТСЯ";
            labelTime.ForeColor = Color.DarkRed;
            labelWork.Text = work.Name;
            buttonPause.Text = @"ОТДЫХ";
            label1.Text = string.Empty;
            labelCount.Text = string.Empty;
            buttonBadDrone.Enabled = false;
            buttonOkDrone.Enabled = false;
            _counts = 0;
            return;
        }

        if (!s.User.Name.Equals(string.Empty) && labelUser.Text.Equals(string.Empty))
        {
            //groupBoxButtons.Enabled = true;
            buttonFinish.Enabled = true;
            _startTime = DateTime.Now;
            _lastPaused = DateTime.Now;
            _paused = DateTime.MinValue;
        }

        buttonBadDrone.Enabled = !labelDroneId.Text.Equals(string.Empty);
        buttonOkDrone.Enabled = !labelDroneId.Text.Equals(string.Empty);
        if (!Core.Config.TestMode) groupBoxButtons.Enabled = !labelDroneId.Text.Equals(string.Empty);

        var sec = (DateTime.Now - s.WorkStart).TotalSeconds;
        if (_paused != DateTime.MinValue)
        {
            var msPaused = (DateTime.Now - _paused).TotalMilliseconds;
            await s.ChangeWorkTimeAsync(_startTime.AddMilliseconds(msPaused), default);
            if (msPaused > _timePausedMinutes * 1000 * 60)
            {
                _startTime = s.WorkStart;
                _lastPaused = DateTime.Now;
                _paused = DateTime.MinValue;
            }
            buttonPause.Text = $@"{(_timePausedMinutes * 60 - msPaused / 1000).ToSecTime()}";
            labelTime.BackColor = Color.LightYellow;
        }
        else
        {
            var minLast = (DateTime.Now - _lastPaused).TotalMinutes;
            var last = Math.Max(0, _timeMinWorkMinutes - minLast);
            buttonPause.Text = last > 0 ? $@"ОТДЫХ [{last:0}]" : "ОТДЫХ";
            buttonPause.Enabled = (DateTime.Now - _lastPaused).TotalMinutes > _timeMinWorkMinutes;
            labelTime.BackColor = Color.WhiteSmoke;
        }

        labelWork.Text = work.Name;
        labelUser.Text = s.User.Name;
        labelTime.Text = sec.ToSecTime();
        label1.Text = work.TimeNormalSec > 0 ? $@"НОРМАТИВ: {work.TimeNormalSec:0} сек." : string.Empty;
        labelCount.Text = work.TimeNormalSec > 0 ? $@"КОЛИЧЕСТВО: {_counts}" : string.Empty;
        _timePausedMinutes = works.Get(Core.Config.Type).TimePauseSec / 60d;
        _timeMinWorkMinutes = works.Get(Core.Config.Type).TimePauseLongSec / 60d;

        if (sec < work.TimeNormalSec * 1.0d)
            labelTime.ForeColor = Color.DarkGreen;
        else if (sec < work.TimeNormalSec * 1.5d)
            labelTime.ForeColor = Color.DarkOrange;
        else
            labelTime.ForeColor = Color.DarkRed;
    }

    private async void ButtonFinishClick(object? sender, EventArgs e)
    {
        var f = new FormYesNo(@"ВЫ ДЕЙСТВИТЕЛЬНО ХОТИТЕ ЗАКОНЧИТЬ РАБОТУ?", Color.LightYellow, Color.DarkRed, new Size(600, 400));
        if (f.ShowDialog(this) != DialogResult.Yes) return;
        var s = Core.IoC.Services.GetRequiredService<Station>();
        await s.StartWorkAsync(new Users.User(), default);
    }

    private async void ButtonPauseClick(object? sender, EventArgs e)
    {
        var s = Core.IoC.Services.GetRequiredService<Station>();

        if (_paused == DateTime.MinValue)
        {
            _paused = DateTime.Now;
        }
        else
        {
            _startTime = s.WorkStart;
            _lastPaused = DateTime.Now;
            _paused = DateTime.MinValue;
        }
        await s.GetStateAsync(default);
    }

    private async Task<string> WriteToCliAsync(string text, int waitMs = 200)
    {
        var ret = await Task.Run(async () =>
        {
            var res = await _betaflight.CliWrite(text, waitMs);
            return res;
        });

        if (ret.Any(x => x.Contains("ERROR"))) richTextBoxMain.SelectionColor = Color.Red;
        var retStr = string.Join(string.Empty, ret);
        richTextBoxMain.AppendText(retStr);
        richTextBoxMain.SelectionColor = Color.Black;
        richTextBoxMain.ScrollToCaret();
        return retStr;
    }

    private void Button1_Click(object sender, EventArgs e) => _ = WriteToCliAsync("#\r\n");
    private void Button2_Click(object sender, EventArgs e) => _ = WriteToCliAsync("help\r\n");
    private void Button3_Click(object sender, EventArgs e) => _ = WriteToCliAsync("get name\r\n");
    private void Button6_Click(object sender, EventArgs e) => _ = WriteToCliAsync("version\r\n");
    private void Button7_Click(object sender, EventArgs e) => _ = WriteToCliAsync("exit\r\n");
    private void Button8_Click(object sender, EventArgs e) => _ = WriteToCliAsync("status\r\n");
    private void Button9_Click(object sender, EventArgs e) => _ = WriteToCliAsync("bl\r\n");
    private void Button10_Click(object sender, EventArgs e) => _ = WriteToCliAsync("dump\r\n");
    private void Button11_Click(object sender, EventArgs e) => _ = WriteToCliAsync("save\r\n");
    private void Button12_Click(object sender, EventArgs e) => richTextBoxMain.Clear();
}