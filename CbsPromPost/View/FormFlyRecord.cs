using CbsPromPost.Model;
using CbsPromPost.Other;
using CbsPromPost.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using Size = System.Drawing.Size;
using Timer = System.Windows.Forms.Timer;

namespace CbsPromPost.View;

public sealed partial class FormFlyRecord : Form
{
    private readonly Timer _timer = new();

    private readonly WebCam _webCam;
    private readonly SharpDxMain _dx;
    private long _counts;

    private int _counterClick;
    private DateTime _counterClickTime;
    private readonly SerialScanner _scanner;
    private readonly VideoRecord _record;

    public FormFlyRecord()
    {
        InitializeComponent();

        _scanner = new SerialScanner(Core.IoC.Services.GetRequiredService<ILogger<SerialScanner>>());

        labelName.Text = $@"ПОСТ №{Core.Config.PostNumber:0}";
        _timer.Interval = 100;

        _webCam = new WebCam();
        _dx = new SharpDxMain(pictureBoxMain, -1);
        pictureBoxMain.SizeMode = PictureBoxSizeMode.CenterImage;

        var works = Core.IoC.Services.GetRequiredService<Works>();
        var work = works.Get(Core.Config.Type);
        labelWork.Text = work.Name;

        Text = $@"[КБ ЦБС] ПОСТ №{Core.Config.PostNumber:0}, полетные тесты и финализация изделия";
        Icon = EmbeddedResources.Get<Icon>("Sprites._user_change.ico");

        buttonFinish.Click += ButtonFinishClick;

        _timer.Tick += TimerTick;
        _record = new VideoRecord();
        _scanner.OnReadValue += ComReadString;
        labelName.MouseClick += NameClick;
        Closed += OnClose;
        Shown += FormShown;
        buttonBadDrone.Click += BadDrone;
        buttonOkDrone.Click += OkDrone;
    }

    private async void OkDrone(object? sender, EventArgs e)
    {
        var answ = await Core.IoC.Services.GetRequiredService<Station>().FinishBodyAsync(labelDroneId.Text, default);
        _counts = await Core.IoC.Services.GetRequiredService<Station>().GetCountsFinishWorks(default);
        if (answ.Equals(string.Empty))
        {
            new FormInfo(@"РАБОТА ЗАВЕРШЕНА", Color.LightGreen, Color.DarkGreen, 3000, new Size(600, 400))
                .Show(this);
            labelDroneId.Text = string.Empty; // Финиш работы
            await Core.IoC.Services.GetRequiredService<Station>().ChangeWorkTimeAsync(DateTime.Now, default);
            return;
        }

        new FormInfo(@$"{answ}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400)).Show(this);
    }

    private async void BadDrone(object? sender, EventArgs e)
    {
        if (labelDroneId.Text.Equals(string.Empty)) return;

        var ft = new FormTextWrite(labelDroneId.Text);
        ft.ShowDialog(this);
        if (ft.Result.Equals(string.Empty))
        {
            return;
        }

        var ret = await Server.AddBadDrone(labelDroneId.Text, ft.Result, default);
        if (!ret)
        {
            new FormInfo(@"НЕ УДАЛОСЬ ПЕРЕВЕСТИ В БРАК", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400))
                .Show(this);
            return;
        }

        //await Core.IoC.Services.GetRequiredService<Station>().FinishBodyAsync(labelDroneId.Text, default);
        new FormInfo(@"ПЕРЕВЕДЕНО В БРАК", Color.Yellow, Color.DarkRed, 3000, new Size(600, 400))
            .Show(this);
        labelDroneId.Text = string.Empty; // Финиш работы
        await Core.IoC.Services.GetRequiredService<Station>().ChangeWorkTimeAsync(DateTime.Now, default);
        return;
    }

    private async void FormShown(object? sender, EventArgs e)
    {
        _scanner.StartAsync(Core.Config.ComScanner);
        _webCam.StartAsync(20);
        _webCam.OnNewVideoFrame += NewFrame;

        var s = Core.IoC.Services.GetRequiredService<Station>();
        labelUser.Text = s.User.Name;
        if (!s.User.Name.Equals(string.Empty))
        {
            labelTime.Text = (DateTime.Now - s.WorkStart).TotalSeconds.ToSecTime();
        }
        _timer.Start();
        _counts = await Core.IoC.Services.GetRequiredService<Station>().GetCountsFinishWorks(default);
    }

    private void ComReadString(string com, string text)
    {
        if (Core.IoC.Services.GetRequiredService<Station>().User.Name.Equals(string.Empty) && text.Contains("USER_"))
        {
            if (!long.TryParse(text.Replace("USER_", ""), out var id)) return;
            var user = Core.IoC.Services.GetRequiredService<Users>().Items.Find(x => x.Id == id);
            if (user == null) return;
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
                if (!bad.Equals(string.Empty))
                {
                    new FormInfo($"ИЗДЕЛИЕ {text} В БРАКЕ!\r\n{bad}", Color.LightPink, Color.DarkRed,
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

            var answ = await Core.IoC.Services.GetRequiredService<Station>().FinishBodyAsync(labelDroneId.Text, default);
            _counts = await Core.IoC.Services.GetRequiredService<Station>().GetCountsFinishWorks(default);
            if (answ.Equals(string.Empty))
            {
                new FormInfo(@"РАБОТА ЗАВЕРШЕНА", Color.LightGreen, Color.DarkGreen, 3000, new Size(600, 400))
                    .Show(this);
                labelDroneId.Text = string.Empty; // Финиш работы
                return;
            }

            new FormInfo(@$"{answ}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400)).Show(this);
        });
    }

    private void NewFrame(Mat mat)
    {
        if (mat.Empty()) return;
        _dx.NotActive = labelDroneId.Text.Equals(string.Empty);
        _record.FrameAdd(labelDroneId.Text, mat);
        _dx.FrameUpdate(mat);
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

    private void TimerTick(object? sender, EventArgs e)
    {
        if ((DateTime.Now - _counterClickTime).TotalMilliseconds > 1000) _counterClick = 0;

        labelName.BackColor = _scanner.IsAlive()
            ? Color.LightGreen
            : Color.LightPink;
        _record.ChangeWriting(!labelDroneId.Text.Equals(string.Empty));

        var works = Core.IoC.Services.GetRequiredService<Works>();
        var work = works.Get(Core.Config.Type);

        var s = Core.IoC.Services.GetRequiredService<Station>();
        if (s.User.Name.Equals(string.Empty) && !labelUser.Text.Equals(string.Empty) | s.User.Name.Equals(string.Empty) && labelUser.Text.Equals(string.Empty)) // Выключение работы
        {
            if (!Core.Config.TestMode)
            {
                labelDroneId.Text = string.Empty;
            }
            labelUser.Text = string.Empty;
            buttonFinish.Enabled = false;
            labelTime.Text = @"РАБОТА НЕ ВЕДЕТСЯ";
            labelTime.ForeColor = Color.DarkRed;
            labelWork.Text = work.Name;
            label1.Text = string.Empty;
            labelCount.Text = string.Empty;
            buttonBadDrone.Enabled = false;
            buttonOkDrone.Enabled = false;
            _counts = 0;
            return;
        }

        if (!s.User.Name.Equals(string.Empty) && labelUser.Text.Equals(string.Empty))
        {
            buttonFinish.Enabled = true;
        }

        buttonBadDrone.Enabled = !labelDroneId.Text.Equals(string.Empty);
        buttonOkDrone.Enabled = !labelDroneId.Text.Equals(string.Empty);

        var sec = (DateTime.Now - s.WorkStart).TotalSeconds;

        labelWork.Text = work.Name;
        labelUser.Text = s.User.Name;
        labelTime.Text = sec.ToSecTime();
        label1.Text = work.TimeNormalSec > 0 ? $@"НОРМАТИВ: {work.TimeNormalSec:0} сек." : string.Empty;
        labelCount.Text = work.TimeNormalSec > 0 ? $@"КОЛИЧЕСТВО: {_counts}" : string.Empty;

        /*
        if (sec < work.TimeNormalSec * 1.0d)
            labelTime.ForeColor = Color.DarkGreen;
        else if (sec < work.TimeNormalSec * 1.5d)
            labelTime.ForeColor = Color.DarkOrange;
        else
            labelTime.ForeColor = Color.DarkRed;
        */
    }

    private async void ButtonFinishClick(object? sender, EventArgs e)
    {
        var f = new FormYesNo(@"ВЫ ДЕЙСТВИТЕЛЬНО ХОТИТЕ ЗАКОНЧИТЬ РАБОТУ?", Color.LightYellow, Color.DarkRed, new Size(600, 400));
        if (f.ShowDialog(this) != DialogResult.Yes) return;
        var s = Core.IoC.Services.GetRequiredService<Station>();
        await s.StartWorkAsync(new Users.User(), default);
    }
}