using System.IO.Ports;
using CbsPromPost.Model;
using CbsPromPost.Other;
using CbsPromPost.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;
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
    private FormWebCam _formWeb;

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

        _formWeb = new FormWebCam();
        labelName.Text = $@"ПОСТ №{Core.Config.PostNumber:0}";
        _counts = 0;
        _timer.Interval = 1000;

        var works = Core.IoC.Services.GetRequiredService<Works>();
        var work = works.Get(Core.Config.Type);
        labelWork.Text = work.Name;

        Text = $@"[КБ ЦБС] ПОСТ №{Core.Config.PostNumber:0}, прошивка и тестирование готовых изделий";
        Icon = EmbeddedResources.Get<Icon>("Sprites._user_change.ico");
        richTextBoxMain.Text = string.Empty;

        buttonPause.Click += ButtonPauseClick;
        buttonFinish.Click += ButtonFinishClick;

        labelFpl.Text = Core.Config.FileFpl;
        labelHex.Text = Core.Config.FileHex;
        labelComScanner.Text = $@"ШК СКАНЕР [{Core.Config.ComScanner}]";
        labelComBeta.Text = $@"BetaFlight [{Core.Config.ComBeta}]";
        labelDfu.Text = $@"BetaFlight DFU mode [{Core.Config.UsbDfuVid}:{Core.Config.UsbDfuPid}]";

        _timer.Tick += TimerTick;
        labelName.MouseClick += NameClick;
        Closed += OnClose;
        Shown += FormShown;
        buttonWebCam.Click += ButtonWebCam;
        buttonImpulseRC.Click += ButtonImpulseRc;
        buttonReset.Click += ButtonResetClick;
        var menu = new ContextMenuStrip();
        menu.Items.Add("ОЧИСТИТЬ", null, OnRichTextBoxClear);
        richTextBoxMain.ContextMenuStrip = menu;
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

    private void ButtonWebCam(object? sender, EventArgs e)
    {
        if (_formWeb.Visible)
        {
            _formWeb.Activate();
            return;
        }

        if (_formWeb.IsDisposed) _formWeb = new FormWebCam();
        _formWeb.Show(this);
    }

    private async void ButtonResetClick(object? sender, EventArgs e)
    {
        if (_betaflight.IsAliveDfu())
        {
            richTextBoxMain.AppendText(string.Join(string.Empty, _betaflight.DfuExit()));
            richTextBoxMain.ScrollToCaret();
            return;
        }

        await _betaflight.CliWrite("#");
        await Task.Delay(500);
        richTextBoxMain.AppendText(string.Join(string.Empty, await _betaflight.CliWrite("exit")));
    }

    private void FormShown(object? sender, EventArgs e)
    {
        _scanner.StartAsync(Core.Config.ComScanner);
        _betaflight.StartAsync(Core.Config.ComBeta);
        _betaflight.StartUsbAsync(int.Parse(Core.Config.UsbDfuVid, System.Globalization.NumberStyles.HexNumber), int.Parse(Core.Config.UsbDfuPid, System.Globalization.NumberStyles.HexNumber));

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
        _timer.Start();
        textBoxCli.KeyDown += CliKeyDown;
    }

    private async void CliKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        var txt = textBoxCli.Text;
        textBoxCli.Text = string.Empty;
        richTextBoxMain.AppendText(string.Join(string.Empty, await _betaflight.CliWrite(txt)));
        richTextBoxMain.ScrollToCaret();
    }

    private void ComReadString(string com, string text)
    {
        Invoke(async () =>
        {
            if (labelUser.Text.Equals(string.Empty)) return;

            var notOk = text.Length != 8;
            if (!notOk && text[..2] != "TT") notOk = true;
            if (!notOk && !long.TryParse(text[2..5], out var valueD)) notOk = true;
            if (!notOk && !long.TryParse(text[5..8], out var valueN)) notOk = true;

            if (notOk)
            {
                new FormInfo(@$"НЕИЗВЕСТНЫЙ ШК: {text}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400))
                    .Show(this);
                return;
            }

            if (labelDroneId.Text.Equals(string.Empty)) // Это первичное сканирование
            {

                labelDroneId.Text = text;
                File.Copy(Application.StartupPath + "\\DB\\_HEX\\" + Core.Config.FileHex,
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\_hex_{labelDroneId.Text}_{Core.Config.FileHex}");
                var fileFplOrig = Application.StartupPath + "\\DB\\_HEX\\" + Core.Config.FileFpl;
                var fileFpl = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\_fpl_{labelDroneId.Text}_{Core.Config.FileFpl}";
                var file = await File.ReadAllLinesAsync(fileFplOrig);
                for (var i = 0; i < file.Length; i++)
                {
                    if (!file[i].Contains("set name")) continue;
                    file[i] = $"set name = VT40 {text}";
                    break;
                }

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
                new FormInfo($"НЕ ВЕРНЫЙ ШК: [{text}]\r\nОЖИДАЕМ [{labelDroneId.Text}]", Color.LightPink, Color.DarkRed,
                    3000, new Size(600, 400)).Show(this);
                return;
            }

            var answ = await Core.IoC.Services.GetRequiredService<Station>()
                .FinishBodyAsync(labelDroneId.Text, default);
            if (answ.Equals(string.Empty))
            {
                _counts++;
                new FormInfo(@"РАБОТА ЗАВЕРШЕНА", Color.LightGreen, Color.DarkGreen, 3000, new Size(600, 400))
                    .Show(this);
                File.Delete(
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\_hex_{labelDroneId.Text}_{Core.Config.FileHex}");
                File.Delete(
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\_fpl_{labelDroneId.Text}_{Core.Config.FileFpl}");
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

        labelComScanner.BackColor = _scanner.IsAlive() ? Color.LightGreen : Color.LightPink;
        labelComBeta.BackColor = _betaflight.IsAlive() ? Color.LightGreen : Color.LightPink;
        labelDfu.BackColor = _betaflight.IsAliveDfu() ? Color.LightGreen : Color.LightPink;

        var s = Core.IoC.Services.GetRequiredService<Station>();
        if (s.User.Name.Equals(string.Empty) && !labelUser.Text.Equals(string.Empty) | s.User.Name.Equals(string.Empty) && labelUser.Text.Equals(string.Empty)) // Выключение работы
        {
            var hex = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\_hex_{labelDroneId.Text}_{Core.Config.FileHex}";
            if (!labelDroneId.Text.Equals(string.Empty) && File.Exists(hex)) File.Delete(hex);
            var fpl = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\_fpl_{labelDroneId.Text}_{Core.Config.FileFpl}";
            if (!labelDroneId.Text.Equals(string.Empty) && File.Exists(fpl)) File.Delete(fpl);

            labelDroneId.Text = string.Empty;
            labelUser.Text = string.Empty;
            //button1.Enabled = false;
            //button2.Enabled = false;
            //button3.Enabled = false;
            //button4.Enabled = false;
            //button5.Enabled = false;
            //button6.Enabled = false;
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
            //button1.Enabled = true;
            //button2.Enabled = true;
            //button3.Enabled = true;
            //button4.Enabled = true;
            //button5.Enabled = true;
            //button6.Enabled = true;
            buttonFinish.Enabled = true;
            _startTime = DateTime.Now;
            _lastPaused = DateTime.Now;
            _paused = DateTime.MinValue;
        }
        /*
        button1.Enabled = !labelDroneId.Text.Equals(string.Empty);
        button2.Enabled = !labelDroneId.Text.Equals(string.Empty);
        button3.Enabled = !labelDroneId.Text.Equals(string.Empty);
        button4.Enabled = !labelDroneId.Text.Equals(string.Empty);
        button5.Enabled = !labelDroneId.Text.Equals(string.Empty);
        button6.Enabled = !labelDroneId.Text.Equals(string.Empty);
        */
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