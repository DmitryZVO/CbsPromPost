using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using CbsPromPost.Model;
using CbsPromPost.Other;
using CbsPromPost.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        var select = false;
        foreach (var p in SerialPort.GetPortNames())
        {
            comboBoxScanner.Items.Add(p);
            if (!p.Equals(Core.Config.ComScanner)) continue;

            comboBoxScanner.SelectedIndex = comboBoxScanner.Items.Count - 1;
            select = true;
        }

        if (!select) comboBoxScanner.SelectedIndex = comboBoxScanner.Items.Count > 0 ? 0 : -1;
        _comScanner = comboBoxScanner.Text;
        if (!_comScanner.Equals(string.Empty)) _scanner.BlackListSerials.RemoveAll(x => x.Equals(_comScanner));
        comboBoxScanner.SelectedIndexChanged += ComScannerChange;
    }

    private void ComScannerChange(object? sender, EventArgs e)
    {
        _comScanner = comboBoxScanner.Text;
        Core.Config.ComScanner = _comScanner;
        if (!_comScanner.Equals(string.Empty))
        {
            _scanner.BlackListSerials = SerialPort.GetPortNames().ToList();
            _scanner.BlackListSerials.RemoveAll(x => x.Equals(_comScanner));
        }
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
        _timer.Start();
    }

    private void ComReadString(string com, string barr)
    {
        Invoke(() => 
        {
            if (labelUser.Text.Equals(string.Empty)) return;
            if (!com.Equals(comboBoxScanner.Text)) return;
            labelDroneId.Text = barr;
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

        labelName.BackColor = _scanner.IsAlive()
            ? Color.LightGreen
            : Color.LightPink;

        var s = Core.IoC.Services.GetRequiredService<Station>();
        if (s.User.Name.Equals(string.Empty) && !labelUser.Text.Equals(string.Empty) | s.User.Name.Equals(string.Empty) && labelUser.Text.Equals(string.Empty)) // Выключение работы
        {
            labelDroneId.Text = string.Empty;
            labelUser.Text = string.Empty;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
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