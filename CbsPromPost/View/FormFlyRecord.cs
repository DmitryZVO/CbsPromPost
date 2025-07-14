using CbsPromPost.Model;
using CbsPromPost.Other;
using CbsPromPost.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.Logging;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using SharpDX;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CbsPromPost.Model.Server;
using static System.Net.Mime.MediaTypeNames;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;
using Timer = System.Windows.Forms.Timer;

namespace CbsPromPost.View;

public sealed partial class FormFlyRecord : Form
{
    private readonly Timer _timer = new();

    private readonly WebCam _webCamFpv;
    private readonly WebCam _webCamBox;
    private readonly SharpDxMain _dxFpv;
    private readonly SharpDxMain _dxBox;
    private readonly SharpDxMain _dxFullScreen;
    private long _counts;

    private int _counterClick;
    private DateTime _counterClickTime;
    private readonly SerialScanner _scanner;
    private readonly VideoRecord _record;
    private bool _reverse = false;
    private int FullScreenCamNumber = 0;
    DateTime _lastCam1Update;
    DateTime _lastCam2Update;

    private VideoWriter _wr1 = new(string.Empty, -1, 0, new OpenCvSharp.Size());
    private VideoWriter _wr2 = new(string.Empty, -1, 0, new OpenCvSharp.Size());

    public FormFlyRecord()
    {
        InitializeComponent();

        _scanner = new SerialScanner(Core.IoC.Services.GetRequiredService<ILogger<SerialScanner>>());

        labelName.Text = $@"ПОСТ №{Core.Config.PostNumber:0}";
        _timer.Interval = 100;

        _webCamFpv = new WebCam();
        _webCamBox = new WebCam();
        _dxFpv = new SharpDxMain(pictureBoxCamFpv, -1);
        _dxBox = new SharpDxMain(pictureBoxCamBox, -1);
        _dxFullScreen = new SharpDxMain(pictureBoxFullScreen, -1);
        pictureBoxCamFpv.SizeMode = PictureBoxSizeMode.CenterImage;

        var works = Core.IoC.Services.GetRequiredService<Works>();
        var work = works.Get(Core.Config.Type);
        labelWork.Text = work.Name;

        Text = $@"[КБ ЦБС] ПОСТ №{Core.Config.PostNumber:0}, полетный тест изделия, v{Core.IoC.Services.GetRequiredService<Station>().Version.ToStringF2()}";
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
        pictureBoxCamFpv.DoubleClick += FpvDoubleClick;
        pictureBoxCamBox.DoubleClick += BoxDoubleClick;
        buttonCamChange.Click += CamChanges;
        pictureBoxFullScreen.Visible = false;
        pictureBoxFullScreen.DoubleClick += DoubleClick;

        //button1.Click += TestUser;
        //button2.Click += Test1C;
    }

    /*
    private async void Test1C(object? sender, EventArgs e)
    {
        await Web1CTest(default);
    }
    */

    public class SendJson1C
    {
        public string UserId { get; set; } = string.Empty;
        public string UserText { get; set; } = string.Empty;
        public string DroneId { get; set; } = string.Empty;
        public string Drone1C { get; set; } = string.Empty;
        public string CurrentTime { get; set; } = string.Empty;
    }

    private async Task<string> Web1CSend(string dId, string d1C, CancellationToken ct)
    {
        try
        {
            var user = Core.IoC.Services.GetRequiredService<Station>().User;
            var serv = Core.IoC.Services.GetRequiredService<Server>();
            using var web = new HttpClient();
            web.DefaultRequestHeaders.Add("Accept", "application/json");
            web.DefaultRequestHeaders.Add("Content", "application/json; charset=UTF-8");
            web.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
            web.DefaultRequestHeaders.Add("PostNumber", $"{Core.IoC.Services.GetRequiredService<Station>().Number.ToString()}");
            web.DefaultRequestHeaders.Add("PostType", $"{Core.IoC.Services.GetRequiredService<Station>().Type.ToString()}");
            web.DefaultRequestHeaders.Add("PostPrefix", $"{Prefix}");
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var data = new SendJson1C { UserId = user.Id.ToString(), UserText = user.Name, DroneId = dId, Drone1C = d1C, CurrentTime = serv.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss.fff") };
            var jsonString = JsonSerializer.Serialize(data);
            var jsonStringUtf = JsonSerializer.Serialize(data, options);
            var content = new StringContent(jsonStringUtf, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            content.Headers.ContentLength = jsonString.Length;

            using var answ = await web.PostAsync($"http://10.0.1.23/bd_test_hs/hs/stepreport", content, ct);
            if (!answ.IsSuccessStatusCode)
            {
                var er = await answ.Content.ReadAsStringAsync(ct);
                Core.IoC.Services.GetRequiredService<ILogger<SerialScanner>>().LogInformation("Web1C: Error: {Txt}", er);
                return er;
            }
        }
        catch (Exception ex)
        {
            Core.IoC.Services.GetRequiredService<ILogger<SerialScanner>>().LogInformation("Web1C: Exception: {Txt}", ex.Message);
            return "Чтото пошло не так, повторите попытку!";
        }
        Core.IoC.Services.GetRequiredService<ILogger<SerialScanner>>().LogInformation("Web1C: OK![200]");
        return string.Empty;
    }

    private void TestUser(object? sender, EventArgs e)
    {
        long id = 45;
        var user = Core.IoC.Services.GetRequiredService<Users>().Items.Find(x => x.Id == id);
        if (user == null) return;

        if (user.State < 0)
        {
            new FormInfo($"ПОЛЬЗОВАТЕЛЬ {user.Name}\r\nЗАБЛОКИРОВАН!", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400))
                .Show(this);
            return;
        }

        labelDroneId.Text = "TT999999";

        _ = Core.IoC.Services.GetRequiredService<Station>().StartWorkAsync(user, default);
        Core.IoC.Services.GetRequiredService<Station>().User = user;
    }

    private new void DoubleClick(object? sender, EventArgs e)
    {
        FullScreenCamNumber = 0;
        pictureBoxFullScreen.Visible = false;
        //buttonCamChange.Visible = true;
        pictureBoxCamFpv.Visible = true;
        pictureBoxCamBox.Visible = true;
    }

    private async void OkDrone(object? sender, EventArgs e)
    {
        var answ = await Core.IoC.Services.GetRequiredService<Station>().FinishBodyAsync(labelDroneId.Text, checkBoxNotMy.Checked, default);
        _counts = await Core.IoC.Services.GetRequiredService<Station>().GetCountsFinishWorks(default);
        if (answ.Equals(string.Empty))
        {
            new FormInfo(@"РАБОТА ЗАВЕРШЕНА", Color.LightGreen, Color.DarkGreen, 3000, new Size(600, 400))
                .Show(this);
            var dId = labelDroneId.Text;
            var d1c = labelDrone1C.Text;
            labelDroneId.Text = string.Empty; // Финиш работы
            labelDrone1C.Text = string.Empty; // Финиш работы
            await Core.IoC.Services.GetRequiredService<Station>().ChangeWorkTimeAsync(DateTime.Now, default);

            if (Core.RecordState != Core.RecState.None)
            {
                RecordStop();
            }
            await Web1CSend(dId, d1c, default); // Отправляем данные о успешном полете
            return;
        }

        new FormInfo(@$"{answ}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400)).Show(this);
    }

    private void FpvDoubleClick(object? sender, EventArgs e)
    {
        FullScreenCamNumber = 1;
        pictureBoxFullScreen.Visible = true;
        //buttonCamChange.Visible = false;
        pictureBoxCamFpv.Visible = false;
        pictureBoxCamBox.Visible = false;
    }

    private void BoxDoubleClick(object? sender, EventArgs e)
    {
        FullScreenCamNumber = 2;
        pictureBoxFullScreen.Visible = true;
        //buttonCamChange.Visible = false;
        pictureBoxCamFpv.Visible = false;
        pictureBoxCamBox.Visible = false;
    }

    private async void BadDrone(object? sender, EventArgs e)
    {
        if (labelDroneId.Text.Equals(string.Empty)) return;

        if (Core.RecordState != Core.RecState.None)
        {
            RecordStop();
        }

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
        labelDrone1C.Text = string.Empty; // Финиш работы
        await Core.IoC.Services.GetRequiredService<Station>().ChangeWorkTimeAsync(DateTime.Now, default);
    }

    private void CamChanges(object? sender, EventArgs e)
    {
        if (_reverse)
        {
            _webCamFpv.OnNewVideoFrame -= NewFrameBox;
            _webCamBox.OnNewVideoFrame -= NewFrameFpv;
            _webCamFpv.OnNewVideoFrame += NewFrameFpv;
            _webCamBox.OnNewVideoFrame += NewFrameBox;
        }
        else
        {
            _webCamFpv.OnNewVideoFrame -= NewFrameFpv;
            _webCamBox.OnNewVideoFrame -= NewFrameBox;
            _webCamFpv.OnNewVideoFrame += NewFrameBox;
            _webCamBox.OnNewVideoFrame += NewFrameFpv;
        }
        _reverse = !_reverse;
    }

    private async void FormShown(object? sender, EventArgs e)
    {
        _scanner.StartAsync(Core.Config.ComScanner);
        _webCamFpv.StartAsync(0, 20);
        _webCamFpv.OnNewVideoFrame += NewFrameFpv;
        _webCamBox.StartAsync(1, 20);
        _webCamBox.OnNewVideoFrame += NewFrameBox;

        var s = Core.IoC.Services.GetRequiredService<Station>();
        labelUser.Text = s.User.Name;
        if (!s.User.Name.Equals(string.Empty))
        {
            labelTime.Text = (DateTime.Now - s.WorkStart).TotalSeconds.ToSecTime();
        }
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
            var updateFile = $"{System.Windows.Forms.Application.StartupPath}Updater\\update.zip";
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

        var upFile = $"{System.Windows.Forms.Application.StartupPath}Updater\\update.zip";
        await File.WriteAllBytesAsync(upFile, data);

        Process.Start($"{System.Windows.Forms.Application.StartupPath}Updater\\Updater.exe", new List<string>
        {
            upFile, // Архив zip с новой версией
            $"{System.Windows.Forms.Application.StartupPath}{System.Windows.Forms.Application.ProductName}.exe" // Имя запускаемого файла после распаковки
        });
        await Task.Delay(TimeSpan.FromMilliseconds(1000));
        Invoke(Close); // Закрываем головное приложение
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

            if (notOk && text.Length != 20)
            {
                new FormInfo(@$"НЕИЗВЕСТНЫЙ ШК: {text}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400))
                    .Show(this);
                return;
            }

            if (text.Length == 8 && labelDroneId.Text.Equals(string.Empty)) // Это первичное сканирование
            {
                var bad = await Server.CheckBadDrone(text, default);
                if (!bad.Equals(string.Empty))
                {
                    new FormInfo($"ИЗДЕЛИЕ {text} В БРАКЕ!\r\n{bad}", Color.LightPink, Color.DarkRed,
                        3000, new Size(600, 400)).Show(this);
                    return;
                }

                labelDroneId.Text = text;
                if (!labelDrone1C.Text.Equals(string.Empty)) RecordStart(labelDroneId.Text);

                return;
            }

            if (text.Length == 20 && labelDrone1C.Text.Equals(string.Empty)) // Это первичное сканирование
            {
                labelDrone1C.Text = text;
                if (!labelDroneId.Text.Equals(string.Empty)) RecordStart(labelDroneId.Text);

                return;
            }

            if (!labelDroneId.Text.Equals(text))
            {
                new FormInfo($"НЕ ВЕРНЫЙ ШК: [{text}]\r\nОЖИДАЕМ [{labelDroneId.Text}]", Color.LightPink, Color.DarkRed,
                    3000, new Size(600, 400)).Show(this);
                return;
            }

            var answ = await Core.IoC.Services.GetRequiredService<Station>().FinishBodyAsync(labelDroneId.Text, checkBoxNotMy.Checked, default);
            _counts = await Core.IoC.Services.GetRequiredService<Station>().GetCountsFinishWorks(default);
            if (answ.Equals(string.Empty))
            {
                new FormInfo(@"РАБОТА ЗАВЕРШЕНА", Color.LightGreen, Color.DarkGreen, 3000, new Size(600, 400))
                    .Show(this);
                var dId = labelDroneId.Text;
                var d1c = labelDrone1C.Text;

                labelDroneId.Text = string.Empty; // Финиш работы
                labelDrone1C.Text = string.Empty; // Финиш работы

                if (Core.RecordState != Core.RecState.None)
                {
                    RecordStop();
                }
                await Web1CSend(dId, d1c, default); // Отправляем данные о успешном полете
                return;
            }

            new FormInfo(@$"{answ}", Color.LightPink, Color.DarkRed, 3000, new Size(600, 400)).Show(this);
        });
    }

    private void NewFrameFpv(Mat mat)
    {
        if (mat.Empty()) return;
        mat.Rectangle(new OpenCvSharp.Point(0, 0), new OpenCvSharp.Point(mat.Width, 15), Scalar.Black, -1);
        Cv2.PutText(mat, Server.Prefix + ", " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ", " + labelDroneId.Text, new OpenCvSharp.Point(8, 13), HersheyFonts.HersheyComplexSmall, 0.8, Scalar.White);
        _lastCam1Update = DateTime.Now;
        //_record.FrameAdd(labelDroneId.Text, mat);
        if (FullScreenCamNumber == 0)
        {
            _dxFpv.NotActive = labelDroneId.Text.Equals(string.Empty);
            _dxFpv.FrameUpdate(mat);
        }
        else if (FullScreenCamNumber == 1)
        {
            _dxFullScreen.NotActive = labelDroneId.Text.Equals(string.Empty);
            _dxFullScreen.FrameUpdate(mat);
        }
        WriteCAM(1, mat);
    }

    private void NewFrameBox(Mat mat)
    {
        if (mat.Empty()) return;
        mat.Rectangle(new OpenCvSharp.Point(0, 0), new OpenCvSharp.Point(mat.Width, 15), Scalar.Black, -1);
        Cv2.PutText(mat, Server.Prefix + ", " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ", " + labelDroneId.Text, new OpenCvSharp.Point(8, 13), HersheyFonts.HersheyComplexSmall, 0.8, Scalar.White);
        _lastCam2Update = DateTime.Now;
        //_record.FrameAdd(labelDroneId.Text, mat);
        if (FullScreenCamNumber == 0)
        {
            _dxBox.NotActive = labelDroneId.Text.Equals(string.Empty);
            _dxBox.FrameUpdate(mat);
        }
        else if (FullScreenCamNumber == 2)
        {
            _dxFullScreen.NotActive = labelDroneId.Text.Equals(string.Empty);
            _dxFullScreen.FrameUpdate(mat);
        }
        WriteCAM(2, mat);
    }

    private void OnClose(object? sender, EventArgs e)
    {
        _webCamFpv.OnNewVideoFrame -= NewFrameFpv;
        _webCamFpv.Dispose();
        _webCamBox.OnNewVideoFrame -= NewFrameBox;
        _webCamBox.Dispose();
        _dxFpv.Dispose();
        _dxBox.Dispose();
        _dxFullScreen.Dispose();
        _timer.Stop();
    }

    private void WriteCAM(int camN, Mat capt)
    {
        using var frame = capt.CvtColor(ColorConversionCodes.BGRA2RGB);
        string cam = $"CAM{camN:0}";
        var date = DateTime.Now.ToString("yyyy-MM-dd") + "\\";
        string filename = $"{Core.Config.DirRecords}{date}{Core.RecordBarr}\\{Core.RecordBarr}_{Core.RecordTimeStart:yyyy-MM-dd HH-mm-ss_fff}_{cam}.avi";
        if (camN == 1)
        {
            if (Core.RecordState == Core.RecState.Record)
            {
                if (!File.Exists(filename))
                {
                    if (!_wr1.IsDisposed) _wr1.Dispose();
                    _wr1 = new VideoWriter(filename, FourCC.FromString("XVID"), 20, new OpenCvSharp.Size(640, 480));
                }
                if (!_wr1.IsDisposed) _wr1.Write(frame);
            }
            else
            {
                if (!_wr1.IsDisposed) _wr1.Dispose();
            }
        }
        else if (camN == 2)
        {
            if (Core.RecordState == Core.RecState.Record)
            {
                if (!File.Exists(filename))
                {
                    if (!_wr2.IsDisposed) _wr2.Dispose();
                    _wr2 = new VideoWriter(filename, FourCC.FromString("XVID"), 20, new OpenCvSharp.Size(640, 480));
                }
                if (!_wr2.IsDisposed) _wr2.Write(frame);
            }
            else
            {
                if (!_wr2.IsDisposed) _wr2.Dispose();
            }
        }
    }

    private void RecordStop()
    {
        Core.RecordState = Core.RecState.None;
        Core.RecordBarr = string.Empty;
        Core.RecordTimeStart = DateTime.MinValue;
    }

    private void RecordStart(string barr)
    {
        Core.RecordState = Core.RecState.Record;
        Core.RecordBarr = barr;
        Core.RecordTimeStart = DateTime.Now;
        var date = DateTime.Now.ToString("yyyy-MM-dd") + "\\";
        try
        {
            Directory.CreateDirectory(Core.Config.DirRecords + date);
        }
        catch { }
        ;
        try
        {
            Directory.CreateDirectory(Core.Config.DirRecords + date + barr);
        }
        catch { }
        ;
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
        if (s.User.Name.Equals(string.Empty) && !labelUser.Text.Equals(string.Empty) | 
            s.User.Name.Equals(string.Empty) && labelUser.Text.Equals(string.Empty)) // Выключение работы
        {
            if (!Core.Config.TestMode)
            {
                labelDroneId.Text = string.Empty;
                labelDrone1C.Text = string.Empty;
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

        buttonBadDrone.Enabled = !labelDroneId.Text.Equals(string.Empty) && !labelDrone1C.Text.Equals(string.Empty);
        buttonOkDrone.Enabled = !labelDroneId.Text.Equals(string.Empty) && !labelDrone1C.Text.Equals(string.Empty);

        var sec = (DateTime.Now - s.WorkStart).TotalSeconds;

        labelWork.Text = work.Name;
        labelUser.Text = s.User.Name;
        labelTime.Text = sec.ToSecTime();
        label1.Text = work.TimeNormalSec > 0 ? $@"НОРМАТИВ: {work.TimeNormalSec:0} сек." : string.Empty;
        labelCount.Text = work.TimeNormalSec > 0 ? $@"КОЛ: {_counts}" : string.Empty;

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