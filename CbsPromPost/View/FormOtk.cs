using CbsPromPost.Model;
using CbsPromPost.Other;
using CbsPromPost.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using System.Diagnostics;
using SharpDX;
using Color = System.Drawing.Color;
using Point = SharpDX.Point;
using Size = System.Drawing.Size;
using Timer = System.Windows.Forms.Timer;
using Microsoft.VisualBasic.Devices;

namespace CbsPromPost.View;

public sealed partial class FormOtk : Form
{
    private readonly Timer _timer = new();

    private readonly WebCamOtk _webCamFull;
    private readonly WebCamOtk _webCamUp;
    private readonly WebCamOtk _webCamLeft;
    private readonly WebCamOtk _webCamRight;
    private readonly SharpDxMainOtk _dx;
    private long _counts;

    private int _counterClick;
    private DateTime _counterClickTime;

    public FormOtk()
    {
        InitializeComponent();

        labelName.Text = $@"ПОСТ №{Core.Config.PostNumber:0}";
        _timer.Interval = 100;

        _webCamFull = new WebCamOtk();
        _webCamUp = new WebCamOtk();
        _webCamLeft = new WebCamOtk();
        _webCamRight = new WebCamOtk();

        _dx = new SharpDxMainOtk(pictureBoxMain, 60);
        pictureBoxMain.SizeMode = PictureBoxSizeMode.CenterImage;

        Text = $@"[КБ ЦБС] ПОСТ №{Core.Config.PostNumber:0}, контроль качества, v{Core.IoC.Services.GetRequiredService<Station>().Version.ToStringF2()}";
        Icon = EmbeddedResources.Get<Icon>("Sprites._user_change.ico");

        buttonFinish.Click += ButtonFinishClick;

        _timer.Tick += TimerTick;
        labelName.MouseClick += NameClick;
        Closed += OnClose;
        Shown += FormShown;
        buttonBadDrone.Click += BadDrone;
        pictureBoxMain.MouseDown += PictureMouseDown;
        pictureBoxMain.MouseMove += PictureMouseMove;
        numericUpDownFocus.ValueChanged += FocusChange;
    }

    private void FocusChange(object? sender, EventArgs e)
    {
        switch (_dx.ActiveCam)
        {
            case 0: // Общий вид
                _webCamFull.Focus = (byte)numericUpDownFocus.Value;
                Core.ConfigOtk.CamFullFocus = _webCamFull.Focus;
                break;
            case 1: // Верхняя
                _webCamUp.Focus = (byte)numericUpDownFocus.Value;
                Core.ConfigOtk.CamUpFocus = _webCamUp.Focus;
                break;
            case 2: // Левая
                _webCamLeft.Focus = (byte)numericUpDownFocus.Value;
                Core.ConfigOtk.CamLeftFocus = _webCamLeft.Focus;
                break;
            case 3: // Правая
                _webCamRight.Focus = (byte)numericUpDownFocus.Value;
                Core.ConfigOtk.CamRightFocus = _webCamRight.Focus;
                break;
        }

        pictureBoxMain.Focus();
        Core.ConfigOtk.Save();
    }

    private void PictureMouseMove(object? sender, MouseEventArgs e)
    {
        _dx.MouseMove(new PointF(e.X / (float)pictureBoxMain.Width, e.Y / (float)pictureBoxMain.Height), e.Button);
    }

    private void PictureMouseDown(object? sender, EventArgs e)
    {
        var mouse = (MouseEventArgs)e;
        _dx.MouseClick(new PointF(mouse.X / (float)pictureBoxMain.Width, mouse.Y / (float)pictureBoxMain.Height),
            mouse.Button);

        switch (_dx.ActiveCam)
        {
            case 0: // Общий вид
                numericUpDownFocus.Value = _webCamFull.Focus;
                break;
            case 1: // Верхняя
                numericUpDownFocus.Value = _webCamUp.Focus;
                break;
            case 2: // Левая
                numericUpDownFocus.Value = _webCamLeft.Focus;
                break;
            case 3: // Правая
                numericUpDownFocus.Value = _webCamRight.Focus;
                break;
        }
    }

    private async void OkDrone()
    {
        var answ = await Core.IoC.Services.GetRequiredService<Station>().FinishBodyAsync(labelDroneId.Text, false, default);
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

        new FormInfo(@"ПЕРЕВЕДЕНО В БРАК", Color.Yellow, Color.DarkRed, 3000, new Size(600, 400))
            .Show(this);
        labelDroneId.Text = string.Empty; // Финиш работы
        await Core.IoC.Services.GetRequiredService<Station>().ChangeWorkTimeAsync(DateTime.Now, default);
    }

    private async void FormShown(object? sender, EventArgs e)
    {
        _ = _dx.StartAsync();

        if (Core.ConfigOtk.CamFullNumber.Contains("rtsp"))
        {
            _webCamFull.StartAsyncRtsp(Core.ConfigOtk.CamFullNumber, 5);
        }
        else
        {
            if (!byte.TryParse(Core.ConfigOtk.CamFullNumber, out var camNumber)) camNumber = 0;
            _webCamFull.StartAsync(camNumber, 5);
        }
        _webCamFull.Focus = Core.ConfigOtk.CamFullFocus;
        numericUpDownFocus.Value = _webCamFull.Focus;
        _webCamFull.OnNewVideoFrame += NewFrameFull;

        if (Core.ConfigOtk.CamUpNumber.Contains("rtsp"))
        {
            _webCamUp.StartAsyncRtsp(Core.ConfigOtk.CamUpNumber, 5);
        }
        else
        {
            if (!byte.TryParse(Core.ConfigOtk.CamUpNumber, out var camNumber)) camNumber = 0;
            _webCamUp.StartAsync(camNumber, 5);
        }
        _webCamUp.Focus = Core.ConfigOtk.CamUpFocus;
        _webCamUp.OnNewVideoFrame += NewFrameUp;

        if (Core.ConfigOtk.CamLeftNumber.Contains("rtsp"))
        {
            _webCamLeft.StartAsyncRtsp(Core.ConfigOtk.CamLeftNumber, 5);
        }
        else
        {
            if (!byte.TryParse(Core.ConfigOtk.CamLeftNumber, out var camNumber)) camNumber = 0;
            _webCamLeft.StartAsync(camNumber, 5);
        }
        _webCamLeft.Focus = Core.ConfigOtk.CamLeftFocus;
        _webCamLeft.OnNewVideoFrame += NewFrameLeft;

        if (Core.ConfigOtk.CamRightNumber.Contains("rtsp"))
        {
            _webCamRight.StartAsyncRtsp(Core.ConfigOtk.CamRightNumber, 5);
        }
        else
        {
            if (!byte.TryParse(Core.ConfigOtk.CamRightNumber, out var camNumber)) camNumber = 0;
            _webCamRight.StartAsync(camNumber, 5);
        }
        _webCamRight.Focus = Core.ConfigOtk.CamRightFocus;
        _webCamRight.OnNewVideoFrame += NewFrameRight;

        var s = Core.IoC.Services.GetRequiredService<Station>();
        labelUser.Text = s.User.Name;
        _timer.Start();
        _counts = await Core.IoC.Services.GetRequiredService<Station>().GetCountsFinishWorks(default);
        _ = StartCheckNewVersionAsync();
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

    private void NewFrameFull(Mat mat)
    {
        if (mat.Empty()) return;
        //_dx.NotActive = labelDroneId.Text.Equals(string.Empty); // КОСТЫЛЬ
        _dx.FrameCamUpdate(mat, SharpDxMainOtk.CameraType.Full);
    }

    private void NewFrameUp(Mat mat)
    {
        if (mat.Empty()) return;
        _dx.FrameCamUpdate(mat, SharpDxMainOtk.CameraType.Up);
    }

    private void NewFrameLeft(Mat mat)
    {
        if (mat.Empty()) return;
        _dx.FrameCamUpdate(mat, SharpDxMainOtk.CameraType.Left);
    }

    private void NewFrameRight(Mat mat)
    {
        if (mat.Empty()) return;
        _dx.FrameCamUpdate(mat, SharpDxMainOtk.CameraType.Right);
    }

    private void OnClose(object? sender, EventArgs e)
    {
        _webCamFull.OnNewVideoFrame -= NewFrameFull;
        _webCamFull.Dispose();
        _webCamUp.OnNewVideoFrame -= NewFrameUp;
        _webCamUp.Dispose();
        _webCamLeft.OnNewVideoFrame -= NewFrameLeft;
        _webCamLeft.Dispose();
        _webCamRight.OnNewVideoFrame -= NewFrameRight;
        _webCamRight.Dispose();
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

        if (_dx.CheckedAll)
        {
            OkDrone();
            _dx.Reset();
        }

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
            labelCount.Text = string.Empty;
            buttonBadDrone.Enabled = false;
            _counts = 0;
            return;
        }

        if (!s.User.Name.Equals(string.Empty) && labelUser.Text.Equals(string.Empty))
        {
            buttonFinish.Enabled = true;
        }

        buttonBadDrone.Enabled = !labelDroneId.Text.Equals(string.Empty);

        labelUser.Text = s.User.Name;
        labelCount.Text = work.TimeNormalSec > 0 ? $@"КОЛИЧЕСТВО: {_counts}" : string.Empty;
    }

    private async void ButtonFinishClick(object? sender, EventArgs e)
    {
        var f = new FormYesNo(@"ВЫ ДЕЙСТВИТЕЛЬНО ХОТИТЕ ЗАКОНЧИТЬ РАБОТУ?", Color.LightYellow, Color.DarkRed, new Size(600, 400));
        if (f.ShowDialog(this) != DialogResult.Yes) return;
        var s = Core.IoC.Services.GetRequiredService<Station>();
        await s.StartWorkAsync(new Users.User(), default);
    }
}