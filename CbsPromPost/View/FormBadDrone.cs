using System.Diagnostics;
using System.Text;
using Assimp;
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

public sealed partial class FormBadDrone : Form
{
    private readonly Timer _timer = new();

    private long _counterClick;
    private DateTime _counterClickTime = DateTime.Now;
    private FormDroneConfig _formDrone;
    private readonly WebCam _webCam;
    private readonly SharpDxMain _dx;

    private readonly SerialScanner _scanner;
    private readonly SerialBetaflight _betaflight;
    private bool _workFullFlash;

    public FormBadDrone()
    {
        InitializeComponent();

        _scanner = new SerialScanner(Core.IoC.Services.GetRequiredService<ILogger<SerialScanner>>());
        _betaflight = new SerialBetaflight(Core.IoC.Services.GetRequiredService<ILogger<SerialBetaflight>>());

        _formDrone = new FormDroneConfig(_betaflight);
        labelName.Text = $@"ПОСТ №{Core.Config.PostNumber:0}";
        _timer.Interval = 100;
        foreach (var f in Core.ConfigDb.Firmwares)
        {
            comboBoxFirmware.Items.Add(f.Name);
        }

        if (comboBoxFirmware.Items.Contains(Core.ConfigDb.LastFirmware))
        {
            comboBoxFirmware.SelectedItem = Core.ConfigDb.LastFirmware;
        }
        else
        {
            comboBoxFirmware.SelectedIndex = 0;
            Core.ConfigDb.LastFirmware = comboBoxFirmware.Items[0]?.ToString() ?? string.Empty;
            Core.Config.Save();
        }

        var works = Core.IoC.Services.GetRequiredService<Works>();
        var work = works.Get(Core.Config.Type);
        labelWork.Text = work.Name;

        Text = $@"[КБ ЦБС] ПОСТ №{Core.Config.PostNumber:0}, зона исправления брака, v{Core.IoC.Services.GetRequiredService<Station>().Version.ToStringF2()}";
        Icon = EmbeddedResources.Get<Icon>("Sprites._user_change.ico");
        richTextBoxMain.Text = string.Empty;

        labelFpl.Text = Core.ConfigDb.Firmwares.Find(x => x.Name.Equals(comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]!.ToString()))!.FileFpl;
        labelHex.Text = Core.ConfigDb.Firmwares.Find(x => x.Name.Equals(comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]!.ToString()))!.FileBin;
        labelComScanner.Text = $@"ШК СКАНЕР [{Core.Config.ComScanner}]";
        labelComBeta.Text = $@"BetaFlight [{Core.Config.ComBeta}]";
        labelDfu.Text = $@"BetaFlight DFU mode [{Core.ConfigDb.UsbDfuVid}:{Core.ConfigDb.UsbDfuPid}]";

        var menu = new ContextMenuStrip();
        menu.Items.Add("ОЧИСТИТЬ", null, OnRichTextBoxClear);
        richTextBoxMain.ContextMenuStrip = menu;

        buttonFinish.Click += ButtonFinishClick;

        _webCam = new WebCam();
        _dx = new SharpDxMain(pictureBoxMain, -1);
        pictureBoxMain.SizeMode = PictureBoxSizeMode.StretchImage;

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
        buttonCloneBarr.Click += ButtonDroneCloneBarr;
        comboBoxFirmware.SelectedValueChanged += FlashChanged;
        buttonOkDrone.Click += OkDrone;
        buttonPower.Click += PowerClick;
        labelDroneId.MouseDoubleClick += ShowIdInfo;
    }

    private async void ButtonDroneCloneBarr(object? sender, EventArgs e)
    {
        var fn = new DialogInputNumeric(Server.Prefix, 0, 999999, 0);
        if (fn.ShowDialog(this) != DialogResult.OK) return;
        var fp = new FormPrinters();
        if (fp.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            using var answ = await web.GetAsync($"PrintLabelFreeNumber?number={fn.Value:0}&count=1&printer={fp.Printer:0}");
        }
        catch
        {
            //
        }
    }

    private void ShowIdInfo(object? sender, MouseEventArgs e)
    {
        if (labelDroneId.Text.Equals(string.Empty)) return;
        new FormIdInfo(labelDroneId.Text).Show(this);
    }

    private void PowerClick(object? sender, EventArgs e)
    {
        _betaflight.PowerEnabled = !_betaflight.PowerEnabled;
        Core.IoC.Services.GetRequiredService<RelayPower>().SetValues(_betaflight.PowerEnabled ? 1 : 0, _betaflight.PowerEnabled ? 1 : 0);
    }

    private async void OkDrone(object? sender, EventArgs e)
    {
        var fprice = new FormBadPrice(labelDroneId.Text);
        if (fprice.ShowDialog(this) != DialogResult.OK) return;

        var answ = await Server.RemoveBadDrone(labelDroneId.Text, default);
        if (answ.Equals(string.Empty))
        {
            var s = Core.IoC.Services.GetRequiredService<Station>();
            var works = Core.IoC.Services.GetRequiredService<Works>();
            var wDeс = works.Get(15); // штраф за брак
            if (wDeс.Name.Equals(string.Empty)) return;
            var wInc = works.Get(14); // премия за ремонт брака
            if (wInc.Name.Equals(string.Empty)) return;
            if (fprice.Dec != 0)
            {
                foreach (var u in fprice.DecUserName)
                {
                    if (u.Equals(string.Empty)) continue;

                    var user = Core.IoC.Services.GetRequiredService<Users>().Items.Find(x => x.Name.Equals(u)); // Пользователь для штрафа
                    if (user == null) continue;

                    await Server.AddOtherPrice(wDeс, user, -fprice.Dec / (int)wDeс.CostRub,
                        default); // Корректируем штраф
                }
            }
            await Server.AddOtherPrice(wInc, s.User, fprice.Inc / (int)wInc.CostRub, default); // Корректируем премию за работу

            new FormInfo(@"РАБОТА ЗАВЕРШЕНА", Color.LightGreen, Color.DarkGreen, 3000, new Size(600, 400))
                .Show(this);
            labelDroneId.Text = string.Empty; // Финиш работы
            await Core.IoC.Services.GetRequiredService<Station>().ChangeWorkTimeAsync(DateTime.Now, default);
            _betaflight.PowerEnabled = false;
            richTextBoxInfo.Text = string.Empty;
            if (!_formDrone.Visible) return;
            _formDrone.Visible = false;
            return;
        }

        new FormInfo(@$"{answ}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400)).Show(this);
    }

    private void FlashChanged(object? sender, EventArgs e)
    {
        labelFpl.Text = Core.ConfigDb.Firmwares.Find(x => x.Name.Equals(comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]!.ToString()))!.FileFpl;
        labelHex.Text = Core.ConfigDb.Firmwares.Find(x => x.Name.Equals(comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]!.ToString()))!.FileBin;
        Core.ConfigDb.LastFirmware = comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]?.ToString() ?? string.Empty;
        Core.Config.Save();
    }

    private async void ButtonDroneConfigClick(object? sender, EventArgs e)
    {
        _betaflight.CliWrite("#\r\nexit");
        await Task.Delay(500);
        _betaflight.CliWrite("#\r\nexit");
        await Task.Delay(500);
        _betaflight.CliWrite("#\r\nexit");

        if (_formDrone.Visible)
        {
            _formDrone.Activate();
            return;
        }

        if (_formDrone.IsDisposed)
        {
            _formDrone = new FormDroneConfig(_betaflight);
            _formDrone.Show();
        }
        else
        {
            _formDrone.Visible = true;
            _formDrone.Activate();
        }
    }

    private async void ButtonFullFlashClick(object? sender, EventArgs e)
    {
        _workFullFlash = true;
        if (_formDrone.Visible)
        {
            _formDrone.Visible = false;
        }

        // Переводим в CLI
        _betaflight.CliWrite("#");
        await Task.Delay(300);
        _betaflight.CliWrite("#");
        await Task.Delay(300);
        _betaflight.CliWrite("#");
        await Task.Delay(300);

        richTextBoxMain.Clear();

        // Переводим в DFU
        _betaflight.CliWrite("#\r\nbl");
        await Task.Delay(300);
        _betaflight.CliWrite("#\r\nbl");
        await Task.Delay(300);
        _betaflight.CliWrite("#\r\nbl");
        await Task.Delay(1000);

        PrintText("СТАНДАРТИЗАЦИЯ ИЗДЕЛИЯ");
        var start = DateTime.Now;

        var pb = Application.StartupPath + "DB\\_HEX\\" + Core.ConfigDb.Firmwares.Find(x =>
            x.Name.Equals(comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]!.ToString()))!.FileBin;
        if (!File.Exists(pb))
        {
            PrintTextFinalError("СТАНДАРТИЗАЦИЯ НЕ УДАЛАСЬ! НЕ НАЙДЕН ФАЙЛ BIN");
            return;
        }

        var dataBin = await File.ReadAllBytesAsync(pb);
        var pf = Application.StartupPath + "DB\\_HEX\\" + Core.ConfigDb.Firmwares.Find(x =>
            x.Name.Equals(comboBoxFirmware.Items[comboBoxFirmware.SelectedIndex]!.ToString()))!.FileFpl;
        if (!File.Exists(pf))
        {
            PrintTextFinalError("СТАНДАРТИЗАЦИЯ НЕ УДАЛАСЬ! НЕ НАЙДЕН ФАЙЛ FPL");
            return;
        }

        var dataFpl = await File.ReadAllLinesAsync(pf);

        if (!_betaflight.IsAliveDfu())
        {
            PrintTextFinalError("СТАНДАРТИЗАЦИЯ НЕ УДАЛАСЬ! НЕ ЗАПУСТИЛСЯ РЕЖИМ DFU!");
            return;
        }

        // Очистка eeprom и запись BIN
        if (await WriteBinAsync(dataBin) == false)
        {
            PrintTextFinalError("СТАНДАРТИЗАЦИЯ НЕ УДАЛАСЬ! НЕ ПРОШЛА ОЧИСТКА EEPROM И ЗАЛИВКА BIN");
            return;
        }

        await Task.Delay(1000);

        // Переводим в DFU
        _betaflight.CliWrite("#\r\nbl");
        await Task.Delay(300);
        _betaflight.CliWrite("#\r\nbl");
        await Task.Delay(300);
        _betaflight.CliWrite("#\r\nbl");
        await Task.Delay(1000);

        // Валидация
        PrintText($"DFU: ВАЛИДАЦИЯ ПРОШИКИ, ОБЛАСТЬ 0x{SerialBetaflight.DfuStartAddress:x8} - 0x{SerialBetaflight.DfuStartAddress + SerialBetaflight.DfuFlashSize:x8} файл hex [{dataBin.Length:0} байт]");
        var flashWriteBytes = await _betaflight.DfuRawBinReadAll();
        if (flashWriteBytes.Length <= 0)
        {
            PrintText("DFU: ОШИБКА!!!");
            PrintTextFinalError("СТАНДАРТИЗАЦИЯ НЕ УДАЛАСЬ! НЕ УДАЛОСЬ ВЫЧИТАТЬ ДАННЫЕ ИЗ EEPROM!");
            return;
        }
        if (!dataBin.SequenceEqual(flashWriteBytes))
        {
            PrintText("DFU: ОШИБКА!!!");
            PrintTextFinalError("СТАНДАРТИЗАЦИЯ НЕ УДАЛАСЬ! ЗАПИСАННЫЕ ДАННЫЕ В EEPROM НЕ СОВПАДАЮТ С ФАЙЛОМ BIN!");
            return;
        }
        PrintText("DFU: УСПЕХ");

        await _betaflight.DfuExit();
        await Task.Delay(1000);

        // Заливка FPL
        if (await WriteFplAsync(dataFpl) == false)
        {
            PrintTextFinalError("СТАНДАРТИЗАЦИЯ НЕ УДАЛАСЬ! НЕ УДАЛОСЬ ЗАЛИТЬ КОНФИГУРАЦИЮ FPL");
            return;
        }

        PrintTextFinalGood($"СТАНДАРТИЗАЦИЯ УСПЕШНА! ЗАТРАЧЕНО {(DateTime.Now - start).TotalSeconds:0} СЕК.");
    }

    public void PrintText(string text)
    {
        Invoke(() =>
        {
            richTextBoxMain.AppendText(text+"\r\n");
            richTextBoxMain.ScrollToCaret();
        });
    }

    public void PrintTextFinalError(string text)
    {
        richTextBoxMain.SelectionBackColor = Color.LightPink;
        richTextBoxMain.AppendText(text+"\r\n");
        richTextBoxMain.SelectionBackColor = Color.White;
        richTextBoxMain.ScrollToCaret();
        _workFullFlash = false;
    }
    public void PrintTextFinalGood(string text)
    {
        richTextBoxMain.SelectionBackColor = Color.LightGreen;
        richTextBoxMain.AppendText(text+"\r\n");
        richTextBoxMain.SelectionBackColor = Color.White;
        richTextBoxMain.ScrollToCaret();
        _workFullFlash = false;
    }

    private async void ButtonWriteFplClick(object? sender, EventArgs e)
    {
        if (_formDrone.Visible)
        {
            _formDrone.Visible = false;
        }

        if (!_betaflight.IsAliveCom())
        {
            PrintText("Контроллер не в режиме MSP/CLI!");
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
                _betaflight.CliWrite("#");
                await Task.Delay(300);
                _betaflight.CliWrite("#");
                await Task.Delay(300);
                _betaflight.CliWrite("#");
                await Task.Delay(3000);

                PrintText($"CLI: ЗАПИСЬ FPL ЛИСТА, файл fpl [{data.Count:0} строк]");
                if (labelDroneId.Text.Equals(string.Empty)) labelDroneId.Text = @$"{Server.Prefix}000000";
                var containName = false;
                var i = 0;
                var stringWrite = new StringBuilder();
                foreach (var s in data)
                {
                    if (s.Equals(string.Empty)) continue;
                    if (s[0].Equals('#')) continue;
                    var sw = s;
                    if (sw.Contains("set name = "))
                    {
                        containName = true;
                        sw = $"set name = VT40 {labelDroneId.Text}";
                    }
                    if (sw.Contains("set pilot_name = "))
                    {
                        containName = true;
                        sw = $"set pilot_name = VT40 {labelDroneId.Text}";
                    }
                    if (sw.Contains("set craft_name = "))
                    {
                        containName = true;
                        sw = $"set craft_name = VT40 {labelDroneId.Text}";
                    }

                    stringWrite.Append(sw);
                    stringWrite.Append("\r\n");
                    i++;
                    if (i < 10) continue;

                    _betaflight.CliWrite(stringWrite.ToString());
                    stringWrite = new StringBuilder();
                    i = 0;
                    await Task.Delay(10);
                }
                _betaflight.CliWrite(stringWrite.ToString());
                await Task.Delay(10);

                if (!containName)
                {
                    _betaflight.CliWrite($"set name = VT40 {labelDroneId.Text}");
                }

                await Task.Delay(3000);

                if (labelDroneId.Text.Equals(@$"{Server.Prefix}000000")) labelDroneId.Text = string.Empty;
                var ok = !richTextBoxMain.Text.Contains("ERROR");
                PrintText(ok ? "CLI: УСПЕХ!" : "CLI: ОШИБКА!!!");
                if (!ok) return false;

                _betaflight.CliWrite("save");
                await Task.Delay(300);
                _betaflight.CliWrite("save");
                await Task.Delay(300);
                _betaflight.CliWrite("save");
                return true;
            });
        });
    }

    private async void ButtonWriteRawImageClick(object? sender, EventArgs e)
    {
        if (_formDrone.Visible)
        {
            _formDrone.Visible = false; 
        }
        if (!_betaflight.IsAliveDfu())
        {
            PrintText("Контроллер не в режиме DFU!");
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

        buttonDroneConfig.Enabled = false;
        await WriteBinAsync(data);
        buttonDroneConfig.Enabled = true;
    }

    private async Task<bool> WriteBinAsync(byte[] data)
    {
        PrintText($"DFU: ОЧИСТКА EEPROM И ЗАПИСЬ ПРОШИКИ, ОБЛАСТЬ 0x{SerialBetaflight.DfuStartAddress:x8} - 0x{SerialBetaflight.DfuStartAddress + SerialBetaflight.DfuFlashSize:x8} файл hex [{data.Length:0} байт]");
        var res = await _betaflight.DfuRawBinWrite(data, 60000);
        await _betaflight.DfuExit();
        PrintText(res >= 0 ? "DFU: УСПЕХ" : "DFU: ОШИБКА!!!");
        return res >= 0;
    }

    private async void ButtonClearFlashClick(object? sender, EventArgs e)
    {
        if (_formDrone.Visible)
        {
            _formDrone.Visible = false;
        }

        if (!_betaflight.IsAliveDfu())
        {
            PrintText("Контроллер не в режиме DFU!");
            return;
        }

        buttonDroneConfig.Enabled = false;
        PrintText($"DFU: ОЧИСТКА ПРОШИКИ, ОБЛАСТЬ 0x{SerialBetaflight.DfuStartAddress:x8} - 0x{SerialBetaflight.DfuStartAddress + SerialBetaflight.DfuFlashSize:x8} [{SerialBetaflight.DfuFlashSize:0} байт]");
        var res = await _betaflight.DfuMassErase(60000);
        PrintText(res >= 0 ? "DFU: УСПЕХ" : "DFU: ОШИБКА!!!");
        buttonDroneConfig.Enabled = true;
    }

    private async void ButtonFplReadClick(object? sender, EventArgs e)
    {
        if (_formDrone.Visible)
        {
            _formDrone.Visible = false;
        }

        if (!_betaflight.IsAliveCom())
        {
            PrintText("Контроллер не в режиме MSP/CLI!");
            return;
        }

        _betaflight.CliWrite("#");
        _betaflight.CliWrite("#");
        _betaflight.CliWrite("#");
        await Task.Delay(2000);
        richTextBoxMain.Clear();
        _betaflight.CliWrite("dump");
        await Task.Delay(2000);
        var res = richTextBoxMain.Text;
        if (res.Length <= 0) return;
        var fd = new SaveFileDialog { Title = @"FPL", FileName = "_fpl_betafly.txt" };
        fd.ShowDialog(this);
        await File.WriteAllTextAsync(fd.FileName, res.Replace("dump\r\n", string.Empty));
    }

    private async void ButtonImageBinReadClick(object? sender, EventArgs e)
    {
        if (_formDrone.Visible)
        {
            _formDrone.Visible = false;
        }

        if (!_betaflight.IsAliveDfu())
        {
            PrintText("Контроллер не в режиме DFU!");
            return;
        }

        PrintText($"DFU: СЧИТЫВАНИЕ ПРОШИВКИ, ОБЛАСТЬ 0x{SerialBetaflight.DfuStartAddress:x8} - 0x{SerialBetaflight.DfuStartAddress + SerialBetaflight.DfuFlashSize:x8} [{SerialBetaflight.DfuFlashSize:0} байт]");
        var image = await _betaflight.DfuRawBinReadAll();
        PrintText(image.Length > 0 ? "DFU: УСПЕХ!" : "DFU: ОШИБКА!!!");
        if (image.Length <= 0) return;

        var fd = new SaveFileDialog { Title = @"RAW HEX", FileName = "_raw_betafly.bin" };
        fd.ShowDialog(this);
        await File.WriteAllBytesAsync(fd.FileName, image);
    }

    private void ButtonImpulseRc(object? sender, EventArgs e)
    {
        try
        {
            Process.Start($"{Application.StartupPath}DB\\ImpulseRC_Driver_Fixer.exe");
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
            PrintText(string.Join(string.Empty, await _betaflight.DfuExit()));
            return;
        }

        _betaflight.CliWrite("#");
        await Task.Delay(500);
        _betaflight.CliWrite("exit");
    }

    private void FormShown(object? sender, EventArgs e)
    {
        _scanner.StartAsync(Core.Config.ComScanner);
        _ = _betaflight.StartAsync(Core.Config.ComBeta);
        _ = _betaflight.StartUsbAsync(int.Parse(Core.ConfigDb.UsbDfuVid, System.Globalization.NumberStyles.HexNumber), int.Parse(Core.ConfigDb.UsbDfuPid, System.Globalization.NumberStyles.HexNumber));
        _betaflight.OnNewCliMessage += OnNewCliMessage;
        _timer.Start();
        _ = StartCheckNewVersionAsync();

        _webCam.StartAsync(20);
        _webCam.OnNewVideoFrame += NewFrame;

        //new FormBadPrice("TT127127").Show();
    }

    private void NewFrame(Mat mat)
    {
        if (mat.Empty()) return;
        _dx.FrameUpdate(mat);
        if (_formDrone.Visible) _formDrone.UpdateFrame(mat);
    }

    private async Task StartCheckNewVersionAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000), ct);

            if (Core.Config.TestMode) continue;

            var updateAvailabeSize = Core.IoC.Services.GetRequiredService<Server>().UpdateVersionPostSize;
            var updateFile = $"{Application.StartupPath}Updater\\update.zip";
            var updateSize = File.Exists(updateFile) ? new FileInfo(updateFile).Length : 0;
            if (updateAvailabeSize > 0 && updateAvailabeSize != updateSize) // Доступно обновление на сервере
            {
                await UpdateProgramAsync();
            }
        }
    }

    private async Task UpdateProgramAsync()
    {
        var data = await Server.GetUpdatePostAsync(default);
        if (data.Length <= 0) return;

        var upFile = $"{Application.StartupPath}Updater\\update.zip";
        await File.WriteAllBytesAsync(upFile, data);

        Process.Start($"{Application.StartupPath}Updater\\Updater.exe", new List<string>
        {
            upFile, // Архив zip с новой версией
            $"{Application.StartupPath}{Application.ProductName}.exe" // Имя запускаемого файла после распаковки
        });
        await Task.Delay(TimeSpan.FromMilliseconds(1000));
        Invoke(Close); // Закрываем головное приложение
    }

    private async void OnNewCliMessage(string message)
    {
        await Task.Run(() =>
        {
            Invoke(() =>
            {
                if (message.Contains("ERROR")) richTextBoxMain.SelectionColor = Color.Red;
                richTextBoxMain.AppendText(message);
                richTextBoxMain.SelectionColor = Color.Black;
                richTextBoxMain.ScrollToCaret();
            });
        });
    }

    private void ProgressChange(int value)
    {
        progressBarMain.Value = Math.Min(Math.Max(progressBarMain.Minimum, value), progressBarMain.Maximum);
    }

    private void CliKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        var txt = textBoxCli.Text;
        textBoxCli.Text = string.Empty;
        _betaflight.CliWrite(txt);
    }

    private void ComReadString(string com, string text)
    {
        if (Core.IoC.Services.GetRequiredService<Station>().User.Name.Equals(string.Empty) && text.Contains("USER_"))
        {
            if (!long.TryParse(text.Replace("USER_", ""), out var id)) return;
            var user = Core.IoC.Services.GetRequiredService<Users>().Items.Find(x => x.Id == id);
            if (user == null) return;

            if (user.State < 0)
            {
                new FormInfo($"ПОЛЬЗОВАТЕЛЬ {text}\r\nЗАБЛОКИРОВАН!", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400))
                    .Show(this);
                return;
            }

            _ = Core.IoC.Services.GetRequiredService<Station>().StartWorkAsync(user, default);
            Core.IoC.Services.GetRequiredService<Station>().User = user;

            return;
        }

        Invoke(async () =>
        {
            if (labelUser.Text.Equals(string.Empty)) return;

            var notOk = text.Length != 8;
            if (!notOk && text[..2] != Server.Prefix) notOk = true;
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
                var bad = await Server.CheckBadDrone(text, default);
                richTextBoxInfo.Text = bad;
                if (bad.Equals(string.Empty))
                {
                    new FormInfo($"ИЗДЕЛИЕ\r\n{text}\r\nНЕ В БРАКЕ!!!!", Color.LightPink, Color.DarkRed,
                        3000, new Size(600, 400)).Show(this);
                    return;
                }

                labelDroneId.Text = text;
                return;
            }


            if (!labelDroneId.Text.Equals(text))
            {
                new FormInfo($"НЕ ВЕРНЫЙ ШК: [{text}]\r\nОЖИДАЕМ [{labelDroneId.Text}]", Color.LightPink, Color.DarkRed,
                    3000, new Size(600, 400)).Show(this);
                return;
            }

            var answ = await Core.IoC.Services.GetRequiredService<Station>().FinishBodyAsync(labelDroneId.Text, false, default);
            if (answ.Equals(string.Empty))
            {
                new FormInfo(@"РАБОТА ЗАВЕРШЕНА", Color.LightGreen, Color.DarkGreen, 3000, new Size(600, 400))
                    .Show(this);
                labelDroneId.Text = string.Empty; // Финиш работы
                if (!_formDrone.Visible) return;
                _formDrone.Visible = false;
                return;
            }

            new FormInfo(@$"{answ}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400)).Show(this);
        });
    }

    private void OnClose(object? sender, EventArgs e)
    {
        _betaflight.FinishWork = true;
        _timer.Stop();
    }

    private void NameClick(object? sender, MouseEventArgs e)
    {
        _counterClick++;
        _counterClickTime = DateTime.Now;
        if (_counterClick <= 6) return;

        new FormSettings().ShowDialog(this);
    }

    private void TimerTick(object? sender, EventArgs e)
    {
        if ((DateTime.Now - _counterClickTime).TotalMilliseconds > 1000) _counterClick = 0;

        labelTimer.Text = DateTime.Now.ToString("HH:mm:ss.fff");
        ProgressChange(_betaflight.ProgressValue);

        var works = Core.IoC.Services.GetRequiredService<Works>();
        var work = works.Get(Core.Config.Type);

        labelComScanner.BackColor = _scanner.IsAlive() ? Color.LightGreen : Color.LightPink;
        labelComBeta.BackColor = _betaflight.IsAliveCom() ? Color.LightGreen : Color.LightPink;
        labelDfu.BackColor = _betaflight.IsAliveDfu() ? Color.LightGreen : Color.LightPink;
        buttonPower.BackColor = _betaflight.PowerEnabled ? Color.LightGreen : Color.LightPink;

        var s = Core.IoC.Services.GetRequiredService<Station>();
        if (s.User.Name.Equals(string.Empty) && !labelUser.Text.Equals(string.Empty) | s.User.Name.Equals(string.Empty) && labelUser.Text.Equals(string.Empty)) // Выключение работы
        {
            if (!Core.Config.TestMode)
            {
                labelDroneId.Text = string.Empty;
                groupBoxButtons.Enabled = false;
                buttonDroneConfig.Enabled = true;
            }
            else
            {
                groupBoxButtons.Enabled = !_workFullFlash;
                buttonDroneConfig.Enabled = !_workFullFlash;
            }
            labelUser.Text = string.Empty;
            buttonFinish.Enabled = false;
            labelWork.Text = work.Name;
            buttonOkDrone.Enabled = false;
            return;
        }

        if (!s.User.Name.Equals(string.Empty) && labelUser.Text.Equals(string.Empty))
        {
            buttonFinish.Enabled = true;
        }

        buttonOkDrone.Enabled = !_workFullFlash && !labelDroneId.Text.Equals(string.Empty);
        if (!Core.Config.TestMode)
        {
            groupBoxButtons.Enabled = !_workFullFlash && !labelDroneId.Text.Equals(string.Empty);
            buttonDroneConfig.Enabled = !_workFullFlash;
            buttonFinish.Enabled = !_workFullFlash;
        }
        else
        {
            groupBoxButtons.Enabled = true;
        }

        labelWork.Text = work.Name;
        labelUser.Text = s.User.Name;
    }

    private async void ButtonFinishClick(object? sender, EventArgs e)
    {
        var f = new FormYesNo(@"ВЫ ДЕЙСТВИТЕЛЬНО ХОТИТЕ ЗАКОНЧИТЬ РАБОТУ?", Color.LightYellow, Color.DarkRed, new Size(600, 400));
        if (f.ShowDialog(this) != DialogResult.Yes) return;
        var s = Core.IoC.Services.GetRequiredService<Station>();
        await s.StartWorkAsync(new Users.User(), default);
        richTextBoxInfo.Text = string.Empty;
    }

    private void Button1_Click(object sender, EventArgs e) => _betaflight.CliWrite("#");
    private void Button2_Click(object sender, EventArgs e) => _betaflight.CliWrite("#\r\nhelp");
    private void Button3_Click(object sender, EventArgs e) => _betaflight.CliWrite("#\r\nget name");
    private void Button6_Click(object sender, EventArgs e) => _betaflight.CliWrite("#\r\nversion");
    private void Button7_Click(object sender, EventArgs e) => _betaflight.CliWrite("#\r\nexit");
    private void Button8_Click(object sender, EventArgs e) => _betaflight.CliWrite("#\r\nstatus");
    private void Button9_Click(object sender, EventArgs e) => _betaflight.CliWrite("#\r\nbl");
    private void Button10_Click(object sender, EventArgs e) => _betaflight.CliWrite("#\r\ndump");
    private void Button11_Click(object sender, EventArgs e) => _betaflight.CliWrite("#\r\nsave");
    private void Button12_Click(object sender, EventArgs e) => richTextBoxMain.Clear();
}