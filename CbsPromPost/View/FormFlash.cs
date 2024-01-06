using System.IO.Ports;
using System.Text;
using System.Xml.Linq;
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

    private double _timePausedMinutes;
    private double _timeMinWorkMinutes;
    private DateTime _startTime;
    private DateTime _lastPaused;
    private DateTime _paused;
    private int _counts;
    private string _comScanner;
    private readonly WebCam _webCam;

    private int _counterClick;
    private DateTime _counterClickTime;
    private readonly SerialScanners _scanner;

    public FormFlash()
    {
        InitializeComponent();
        _scanner = new SerialScanners(Core.IoC.Services.GetRequiredService<ILogger<SerialScanners>>());

        buttonPause.Enabled = false;

        labelName.Text = $@"ПОСТ №{Core.Config.PostNumber:0}";
        _counts = 0;
        _comScanner = string.Empty;
        _timer.Interval = 1000;
        _timer.Tick += TimerTick;
        labelName.MouseClick += NameClick;
        Closed += OnClose;
        Shown += FormShown;
        var works = Core.IoC.Services.GetRequiredService<Works>();
        var work = works.Get(Core.Config.Type);
        labelWork.Text = work.Name;

        Text = $@"[КБ ЦБС] ПОСТ №{Core.Config.PostNumber:0}, прошивка и тестирование готовых изделий";
        Icon = EmbeddedResources.Get<Icon>("Sprites._user_change.ico");
        richTextBoxMain.Text = new StringBuilder()
            .Append("Задачи:\n")
            .Append("0> Сосканировать ШК изделия (для формирования FPL-листа.\n")
            .Append("1> Включить изделие от лабораторного ИП и проверить потребление тока.\n")
            .Append("2> Проверить изображение с камеры, включая шевеление передатчика.\n")
            .Append("3> Подключить контроллер через type-C.\n")
            .Append("Бла-бла-бла.\n")
            .ToString();

        buttonPause.Click += ButtonPauseClick;
        buttonFinish.Click += ButtonFinishClick;

        UpdateScanners();
        comboBoxScanner.SelectedValueChanged += ComScannerChange;
        _ = _scanner.StartAsync();
        _webCam = new WebCam();

        button1.Text = Core.Config.Button1Exe.Equals(string.Empty)? string.Empty : Core.Config.Button1Exe.Split(".")[0];
        button2.Text = Core.Config.Button2Exe.Equals(string.Empty) ? string.Empty : Core.Config.Button2Exe.Split(".")[0];
        button3.Text = Core.Config.Button3Exe.Equals(string.Empty) ? string.Empty : Core.Config.Button3Exe.Split(".")[0];
        button4.Text = Core.Config.Button4Exe.Equals(string.Empty) ? string.Empty : Core.Config.Button4Exe.Split(".")[0];
        button5.Text = Core.Config.Button5Exe.Equals(string.Empty) ? string.Empty : Core.Config.Button5Exe.Split(".")[0];
        button6.Text = Core.Config.Button6Exe.Equals(string.Empty) ? string.Empty : Core.Config.Button6Exe.Split(".")[0];
        button7.Text = Core.Config.Button7Exe.Equals(string.Empty) ? string.Empty : Core.Config.Button7Exe.Split(".")[0];
        labelHex.Text = Core.Config.FileHex;
        labelFpl.Text = Core.Config.FileFpl;
        button1.Click += Button1Click;
        button2.Click += Button2Click;
        button3.Click += Button3Click;
        button4.Click += Button4Click;
        button5.Click += Button5Click;
        button6.Click += Button6Click;
        button7.Click += Button7Click;

        pictureBoxMain.SizeMode = PictureBoxSizeMode.StretchImage;
    }

    private void Button1Click(object? sender, EventArgs e)
    {
        if (Core.Config.Button1Exe.Equals(string.Empty)) return;
        try
        {
            System.Diagnostics.Process.Start($"{Application.StartupPath}DB\\button1\\{Core.Config.Button1Exe}");
        }
        catch (Exception ex)
        {
            new FormInfo($"ОШИБКА ЗАПУСКА\r\n[{ex.Message}]", Color.LightPink, Color.DarkRed, 3000, new Size(1000, 800)).Show(this);
        }
    }

    private void Button2Click(object? sender, EventArgs e)
    {
        if (Core.Config.Button2Exe.Equals(string.Empty)) return;
        try
        {
            System.Diagnostics.Process.Start($"{Application.StartupPath}DB\\button2\\{Core.Config.Button2Exe}");
        }
        catch (Exception ex)
        {
            new FormInfo($"ОШИБКА ЗАПУСКА\r\n[{ex.Message}]", Color.LightPink, Color.DarkRed, 3000, new Size(1000, 800)).Show(this);
        }
    }
    private void Button3Click(object? sender, EventArgs e)
    {
        if (Core.Config.Button3Exe.Equals(string.Empty)) return;
        try
        {
            System.Diagnostics.Process.Start($"{Application.StartupPath}DB\\button3\\{Core.Config.Button3Exe}");
        }
        catch (Exception ex)
        {
            new FormInfo($"ОШИБКА ЗАПУСКА\r\n[{ex.Message}]", Color.LightPink, Color.DarkRed, 3000, new Size(1000, 800)).Show(this);
        }
    }
    private void Button4Click(object? sender, EventArgs e)
    {
        if (Core.Config.Button4Exe.Equals(string.Empty)) return;
        try
        {
            System.Diagnostics.Process.Start($"{Application.StartupPath}DB\\button4\\{Core.Config.Button4Exe}");
        }
        catch (Exception ex)
        {
            new FormInfo($"ОШИБКА ЗАПУСКА\r\n[{ex.Message}]", Color.LightPink, Color.DarkRed, 3000, new Size(1000, 800)).Show(this);
        }
    }
    private void Button5Click(object? sender, EventArgs e)
    {
        if (Core.Config.Button5Exe.Equals(string.Empty)) return;
        try
        {
            System.Diagnostics.Process.Start($"{Application.StartupPath}DB\\button5\\{Core.Config.Button5Exe}");
        }
        catch (Exception ex)
        {
            new FormInfo($"ОШИБКА ЗАПУСКА\r\n[{ex.Message}]", Color.LightPink, Color.DarkRed, 3000, new Size(1000, 800)).Show(this);
        }
    }
    private void Button6Click(object? sender, EventArgs e)
    {
        if (Core.Config.Button6Exe.Equals(string.Empty)) return;
        try
        {
            System.Diagnostics.Process.Start($"{Application.StartupPath}DB\\button6\\{Core.Config.Button6Exe}");
        }
        catch (Exception ex)
        {
            new FormInfo($"ОШИБКА ЗАПУСКА\r\n[{ex.Message}]", Color.LightPink, Color.DarkRed, 3000, new Size(1000, 800)).Show(this);
        }
    }
    private void Button7Click(object? sender, EventArgs e)
    {
        if (Core.Config.Button7Exe.Equals(string.Empty)) return;
        try
        {
            System.Diagnostics.Process.Start($"{Application.StartupPath}DB\\button7\\{Core.Config.Button7Exe}");
        }
        catch (Exception ex)
        {
            new FormInfo($"ОШИБКА ЗАПУСКА\r\n[{ex.Message}]", Color.LightPink, Color.DarkRed, 3000, new Size(1000, 800)).Show(this);
        }
    }

    private void UpdateScanners()
    {
        var select = false;
        var ports = SerialPort.GetPortNames().ToList();
        foreach (var p in ports)
        {
            if (comboBoxScanner.Items.Contains(p)) continue;

            comboBoxScanner.Items.Add(p);
            if (!p.Equals(Core.Config.ComScanner)) continue;

            comboBoxScanner.SelectedIndex = comboBoxScanner.Items.Count - 1;
            select = true;
        }

        var itemsRemove = new List<string>();
        foreach (var cb in comboBoxScanner.Items)
        {
            if (ports.Exists(x=>x.Equals(cb))) continue;
            itemsRemove.Add(cb.ToString()!);
        }

        foreach (var i in itemsRemove)
        {
            comboBoxScanner.Items.Remove(i);
        }

        if (!select && comboBoxScanner.Text.Equals(string.Empty)) comboBoxScanner.SelectedIndex = comboBoxScanner.Items.Count > 0 ? 0 : -1;
        _comScanner = comboBoxScanner.Text;
    }

    private void ComScannerChange(object? sender, EventArgs e)
    {
        _comScanner = comboBoxScanner.Text;
        Core.Config.ComScanner = _comScanner;
        _scanner.WhiteSerial = _comScanner;
        Core.Config.Save();
    }

    private void FormShown(object? sender, EventArgs e)
    {
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
        _scanner.OnReadValue += ComReadString;
        if (!_comScanner.Equals(string.Empty)) _scanner.WhiteSerial = _comScanner;
        _timer.Start();
        _webCam.OnNewVideoFrame += NewWebFrame;
        _webCam.StartAsync(30);
    }

    private void NewWebFrame(Mat mat)
    {
        if (mat.Empty()) return;
        Invoke(() =>
        {
            pictureBoxMain.Image?.Dispose();
            pictureBoxMain.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat);
            pictureBoxMain.Refresh();
        });
        //if (labelDroneId.Text.Equals(string.Empty)) return;
        //Cv2.ImWrite($"CAPTURE\\_{DateTime.Now.Ticks:0}.jpg", mat);
    }

    private void ComReadString(string com, string text)
    {
        Invoke(async () =>
        {
            if (labelUser.Text.Equals(string.Empty)) return;
            if (!com.Equals(comboBoxScanner.Text)) return;

            var notOk = text.Length != 8;
            if (!notOk && text[..2] != "TT") notOk = true;
            if (!notOk && !long.TryParse(text[2..5], out var valueD)) notOk = true;
            if (!notOk && !long.TryParse(text[5..8], out var valueN)) notOk = true;

            if (notOk)
            {
                new FormInfo(@$"НЕИЗВЕСТНЫЙ ШК: {text}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400)).Show(this);
                return;
            }

            if (labelDroneId.Text.Equals(string.Empty)) // Это первичное сканирование
            {

                labelDroneId.Text = text;
                File.Copy(Application.StartupPath + "\\DB\\_HEX\\" + Core.Config.FileHex, $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\_hex_{labelDroneId.Text}_{Core.Config.FileHex}");
                var fileFplOrig = Application.StartupPath + "\\DB\\_HEX\\" + Core.Config.FileFpl;
                var fileFpl = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\_fpl_{labelDroneId.Text}_{Core.Config.FileFpl}";
                var file =  await File.ReadAllLinesAsync(fileFplOrig);
                for (var i = 0; i < file.Length; i++)
                {
                    if (!file[i].Contains("set name")) continue;
                    file[i] = $"set name = VT40 {text}";
                    break;
                }
                /*
                if (File.Exists(fileFpl)) File.Delete(fileFpl);
                File.WriteAllLines(fileFpl, file);
                */
                var port = new SerialPort(Core.Config.ComBeta, 115200, Parity.None, 8, StopBits.One)
                {
                    ReadTimeout = 1000,
                    WriteTimeout = 1000,
                };
                richTextBoxMain.Text = string.Empty;
                port.Open();
                port.WriteLine("#");
                foreach (var f in file)
                {
                    richTextBoxMain.AppendText($"{f}\n");
                    port.WriteLine(f);
                    await Task.Delay(TimeSpan.FromMilliseconds(10));
                    richTextBoxMain.AppendText(port.ReadLine());
                }
                port.Close();
                port.Dispose();

                return;
            }

            if (!labelDroneId.Text.Equals(text))
            {
                new FormInfo($"НЕ ВЕРНЫЙ ШК: [{text}]\r\nОЖИДАЕМ [{labelDroneId.Text}]", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400)).Show(this);
                return;
            }

            var answ = await Core.IoC.Services.GetRequiredService<Station>().FinishBodyAsync(labelDroneId.Text, default);
            if (answ.Equals(string.Empty))
            {
                _counts++;
                new FormInfo(@"РАБОТА ЗАВЕРШЕНА", Color.LightGreen, Color.DarkGreen, 3000, new Size(600, 400)).Show(this);
                File.Delete($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\_hex_{labelDroneId.Text}_{Core.Config.FileHex}");
                File.Delete($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\_fpl_{labelDroneId.Text}_{Core.Config.FileFpl}");
                labelDroneId.Text = string.Empty; // Финиш работы
                return;
            }
            new FormInfo(@$"{answ}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400)).Show(this);
        });
    }

    private void OnClose(object? sender, EventArgs e)
    {
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

        var works = Core.IoC.Services.GetRequiredService<Works>();
        var work = works.Get(Core.Config.Type);
        UpdateScanners();

        labelName.BackColor = _scanner.IsAlive()
            ? Color.LightGreen
            : Color.LightPink;

        var s = Core.IoC.Services.GetRequiredService<Station>();
        if (s.User.Name.Equals(string.Empty) && !labelUser.Text.Equals(string.Empty) | s.User.Name.Equals(string.Empty) && labelUser.Text.Equals(string.Empty)) // Выключение работы
        {
            var hex = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\_hex_{labelDroneId.Text}_{Core.Config.FileHex}";
            if (!labelDroneId.Text.Equals(string.Empty) && File.Exists(hex)) File.Delete(hex);
            var fpl = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\_fpl_{labelDroneId.Text}_{Core.Config.FileFpl}";
            if (!labelDroneId.Text.Equals(string.Empty) && File.Exists(fpl)) File.Delete(fpl);

            labelDroneId.Text = string.Empty;
            labelUser.Text = string.Empty;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
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
            _counts = 0;
            return;
        }

        if (!s.User.Name.Equals(string.Empty) && labelUser.Text.Equals(string.Empty))
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            buttonFinish.Enabled = true;
            _startTime = DateTime.Now;
            _lastPaused = DateTime.Now;
            _paused = DateTime.MinValue;
        }

        button1.Enabled = !labelDroneId.Text.Equals(string.Empty);
        button2.Enabled = !labelDroneId.Text.Equals(string.Empty);
        button3.Enabled = !labelDroneId.Text.Equals(string.Empty);
        button4.Enabled = !labelDroneId.Text.Equals(string.Empty);
        button5.Enabled = !labelDroneId.Text.Equals(string.Empty);
        button6.Enabled = !labelDroneId.Text.Equals(string.Empty);
        button7.Enabled = !labelDroneId.Text.Equals(string.Empty);

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

}