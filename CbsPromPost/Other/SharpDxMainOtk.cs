using CbsPromPost.Resources;
using SharpDX;
using SharpDX.Mathematics.Interop;
using SharpDX.Direct2D1;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using OpenCvSharp;
using RectangleF = SharpDX.RectangleF;
using OpenCvSharp.Extensions;

namespace CbsPromPost.Other;

internal class SharpDxMainOtk : SharpDx
{
    public bool NotActive { get; set; }

    public const float VWidth = 1860f;
    public const float VHeight = 889f;
    public const int CamWidth = 1920;
    public const int CamHeight = 1080;
    public int ActiveCam { get; set; }

    public bool CheckedAll { get; private set; }

    private PointF _mousePos;
    private MouseButtons _mouseButtons = MouseButtons.None;

    private readonly RectangleF _cam0 = new(0.00f, 0.00f, 0.75f, 1.00f);
    private readonly RectangleF _cam1 = new(0.75f, 0.00f, 0.25f, 0.333f);
    private readonly RectangleF _cam2 = new(0.75f, 0.333f, 0.25f, 0.333f);
    private readonly RectangleF _cam3 = new(0.75f, 0.666f, 0.25f, 0.333f);

    private const float MaskAlpha = 0.3f;

    private readonly List<CameraOtk> _cameras;

    private int CamGetForPos(PointF pos)
    {
        if (pos.X > _cam1.X && pos.Y > _cam1.Y && pos.X < _cam1.X + _cam1.Width && pos.Y < _cam1.Y + _cam1.Height) // Окно 1
        {
            return 1;
        }
        if (pos.X > _cam2.X && pos.Y > _cam2.Y && pos.X < _cam2.X + _cam2.Width && pos.Y < _cam2.Y + _cam2.Height) // Окно 2
        {
            return 2;
        }
        if (pos.X > _cam3.X && pos.Y > _cam3.Y && pos.X < _cam3.X + _cam3.Width && pos.Y < _cam3.Y + _cam3.Height) // Окно 3
        {
            return 3;
        }
        return 0;
    }

    public void MouseClick(PointF pos, MouseButtons button)
    {
        _mouseButtons = button;
        if (button == MouseButtons.Left)
        {
            var cam = CamGetForPos(pos);
            ActiveCam = cam switch
            {
                // Окно 1
                1 => ActiveCam == 1 ? 0 : 1,
                // Окно 2
                2 => ActiveCam == 2 ? 0 : 2,
                // Окно 3
                3 => ActiveCam == 3 ? 0 : 3,
                _ => ActiveCam
            };
            lock (this)
            {
                _cameras.ForEach(x => x.Zones.FindAll(y => y.LightState == LightStates.Select).ForEach(z =>
                    z.State = z.State == ZoneState.NotChecked ? ZoneState.Checked : ZoneState.NotChecked));
                if (_cameras.Find(x => x.Zones.Exists(y => y.State != ZoneState.Checked)) == null) CheckedAll = true;
            }
        }
    }

    public void Reset()
    {
        lock (this)
        {
            _cameras.ForEach(x => x.Zones.ForEach(y => y.State = ZoneState.NotChecked));
            CheckedAll = false;
        }
    }

    public void MouseMove(PointF pos, MouseButtons button)
    {
        _mousePos = pos;
        _mouseButtons = button;
    }

    public virtual void FrameCamUpdate(Mat frame, CameraType type)
    {
        var temp = CreateDxBitmap(frame);
        if (temp is null) return;

        lock (this)
        {
            var cam = _cameras.Find(x => x.Type == type)!;
            cam.FrameVideo.Dispose();
            cam.FrameVideo = temp;
        }
    }

    public SharpDxMainOtk(PictureBox surfacePtr, int fpsTarget) : base(surfacePtr, fpsTarget, new Sprites(), (int)VWidth)
    {
        _cameras = new List<CameraOtk>
        {
            new(this, CameraType.Full, "ОБЩИЙ ВИД"),
            new(this, CameraType.Up, "ПОЛЕТНЫЙ КОНТРОЛЛЕР"),
            new(this, CameraType.Left, "РЕГУЛЯТОР СЛЕВА"),
            new(this, CameraType.Right, "РЕГУЛЯТОР СПРАВА")
        };

        _cameras[0].SetMask(@$"{Application.StartupPath}\\OTK\\OtkFull.png");
        _cameras[1].SetMask(@$"{Application.StartupPath}\\OTK\\OtkUp.png");
        _cameras[2].SetMask(@$"{Application.StartupPath}\\OTK\\OtkLeft.png");
        _cameras[3].SetMask(@$"{Application.StartupPath}\\OTK\\OtkRight.png");

        _cameras[0].Matrix3X2 = new RawMatrix3x2(
            (VWidth * _cam0.Width) / CamWidth, // Масштаб X
            0.0f, 0.0f, // Поворот X,Y
            (VHeight * _cam0.Height) / CamHeight, // Масштаб Y
            VWidth * _cam0.X, VHeight * _cam0.Y); // Сдвиг X,Y

        _cameras[1].Matrix3X2 = new RawMatrix3x2(
            (VWidth * _cam1.Width) / CamWidth, // Масштаб X
            0.0f, 0.0f, // Поворот X,Y
            (VHeight * _cam1.Height) / CamHeight, // Масштаб Y
            VWidth * _cam1.X, VHeight * _cam1.Y); // Сдвиг X,Y

        _cameras[2].Matrix3X2 = new RawMatrix3x2(
            (VWidth * _cam2.Width) / CamWidth, // Масштаб X
            0.0f, 0.0f, // Поворот X,Y
            (VHeight * _cam2.Height) / CamHeight, // Масштаб Y
            VWidth * _cam2.X, VHeight * _cam2.Y); // Сдвиг X,Y

        _cameras[3].Matrix3X2 = new RawMatrix3x2(
            (VWidth * _cam3.Width) / CamWidth, // Масштаб X
            0.0f, 0.0f, // Поворот X,Y
            (VHeight * _cam3.Height) / CamHeight, // Масштаб Y
            VWidth * _cam3.X, VHeight * _cam3.Y); // Сдвиг X,Y
    }

    private Bitmap GetMask(string file)
    {
        using var bm = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(file);
        return CreateDxBitmap(bm)!;
    }

    private static List<RawRectangleF> GetRectsZones(string file)
    {
        var ret = new List<RawRectangleF>();

        using var bm = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(file);
        using var mat = bm.ToMat();
        using var mask = mat.Canny(0, 255);
        mask.FindContours(out var counters, out var _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        foreach (var c in counters)
        {
            var rect = Cv2.BoundingRect(c); // ограничивающий прямоугольник
            ret.Add(new RawRectangleF(rect.Left / (float)CamWidth, rect.Top / (float)CamHeight, rect.Right / (float)CamWidth,
                rect.Bottom / (float)CamHeight));
        }

        return ret;
    }

    protected sealed override void DrawUser()
    {
        lock (this)
        {
            _cameras.ForEach(x => x.CheckLight()); // Проверка выбора
            _cameras.ForEach(x=>x.Draw()); // отрисовка камер


            if (NotActive)
            {
                Rt?.FillRectangle(new RawRectangleF(0, 0, BaseWidth, BaseHeight), Brushes.SysTextBrushGray);
            }
            else
            {
                var activeCam = CamGetForPos(_mousePos);
                if (activeCam > 0)
                {
                    switch (activeCam)
                    {
                        case 1:
                            TransformSet(_cameras[1].Matrix3X2);
                            break;
                        case 2:
                            TransformSet(_cameras[2].Matrix3X2);
                            break;
                        case 3:
                            TransformSet(_cameras[3].Matrix3X2);
                            break;
                    }

                    Rt?.FillRectangle(new RawRectangleF(0, 0, CamWidth, CamHeight), Brushes.RoiYellow01);
                }
            }
        }
    }

    private void DrawLens(Bitmap frame)
    {
        if (_mouseButtons != MouseButtons.Right) return; // Не зажата правая кнопка
        if (CamGetForPos(_mousePos) != 0) return;

        var posX = CamWidth / _cam0.Width * _mousePos.X;
        var posY = CamHeight / _cam0.Height * _mousePos.Y;
        const float sizeOrig = 150f;
        const float sizeShow = 600f;
        var rectShow = new RawRectangleF(posX - sizeShow, posY - sizeShow, posX + sizeShow, posY + sizeShow);
        Rt?.DrawBitmap(frame, rectShow, 1.0f,
            BitmapInterpolationMode.Linear,
            new RawRectangleF(posX - sizeOrig, posY - sizeOrig, posX + sizeOrig, posY + sizeOrig));
        Rt?.DrawRectangle(rectShow, Brushes.SysTextBrushYellow, 10f);
    }

    private RawMatrix3x2 GetClip(int camNumber)
    {
        lock (this)
        {
            return ActiveCam switch
            {
                1 => camNumber switch
                {
                    0 => _cameras[1].Matrix3X2,
                    1 => _cameras[0].Matrix3X2,
                    2 => _cameras[2].Matrix3X2,
                    _ => _cameras[3].Matrix3X2,
                },
                2 => camNumber switch
                {
                    0 => _cameras[2].Matrix3X2,
                    1 => _cameras[1].Matrix3X2,
                    2 => _cameras[0].Matrix3X2,
                    _ => _cameras[3].Matrix3X2,
                },
                3 => camNumber switch
                {
                    0 => _cameras[3].Matrix3X2,
                    1 => _cameras[1].Matrix3X2,
                    2 => _cameras[2].Matrix3X2,
                    _ => _cameras[0].Matrix3X2,
                },
                _ => camNumber switch
                {
                    0 => _cameras[0].Matrix3X2,
                    1 => _cameras[1].Matrix3X2,
                    2 => _cameras[2].Matrix3X2,
                    _ => _cameras[3].Matrix3X2,
                }
            };
        }
    }

    protected sealed override void DrawInfo()
    {
    }

    public class CameraOtk
    {
        public RawMatrix3x2 Matrix3X2 { get; set; } = new();
        public Bitmap FrameVideo { get; set; }
        public Bitmap FrameMask { get; set; }
        public CameraType Type { get; set; }
        public string Name { get; set; }
        public List<Zone> Zones { get; set; }

        private readonly SharpDxMainOtk _dx;

        public CameraOtk(SharpDxMainOtk dx, CameraType type, string name)
        {
            _dx = dx;
            Type = type;
            Name = name;
            Zones = new List<Zone>();

            FrameVideo = new Bitmap(_dx.Rt, new Size2(CamWidth, CamHeight), new BitmapProperties { PixelFormat = _dx.PixelFormat });
            FrameMask = new Bitmap(_dx.Rt, new Size2(CamWidth, CamHeight), new BitmapProperties { PixelFormat = _dx.PixelFormat });
        }

        public void SetMask(string file)
        {
            lock (_dx)
            {
                foreach (var z in GetRectsZones(file))
                {
                    Zones.Add(new Zone { RectN = z, Rect = new RawRectangleF(z.Left * CamWidth, z.Top * CamHeight, z.Right * CamWidth, z.Bottom * CamHeight), State = ZoneState.NotChecked });
                }

                FrameMask = _dx.GetMask(file);
            }
        }

        public void CheckLight()
        {
            lock (_dx)
            {
                if (_dx.CamGetForPos(_dx._mousePos) != 0 | !_dx.GetClip(0).Equals(Matrix3X2))
                {
                    Zones.ForEach(x=>x.LightState = LightStates.NotSelect);
                    return;
                }

                var posX = CamWidth / _dx._cam0.Width * _dx._mousePos.X;
                var posY = CamHeight / _dx._cam0.Height * _dx._mousePos.Y;

                foreach (var z in Zones)
                {
                    z.LightState = (posX >= z.Rect.Left & posY >= z.Rect.Top & posX <= z.Rect.Right & posY <= z.Rect.Bottom) ? LightStates.Select : LightStates.NotSelect;
                }
            }
        }

        public void Draw()
        {
            lock (_dx)
            {
                _dx.TransformSet(_dx.GetClip((int)Type));
                _dx.Rt?.DrawBitmap(FrameVideo, 1.0f, BitmapInterpolationMode.Linear);
                _dx.Rt?.DrawBitmap(FrameMask, MaskAlpha, BitmapInterpolationMode.Linear);
                _dx.Rt?.DrawRectangle(new RawRectangleF(0, 0, CamWidth, CamHeight), _dx.Brushes.SysTextBrushGreen, 5);

                foreach (var z in Zones)
                {
                    if (z.LightState == LightStates.Select)
                    {
                        _dx.Rt?.FillRectangle(z.Rect, _dx.Brushes.RoiGray01);
                    }
                    _dx.Rt?.DrawRectangle(z.Rect, z.State == ZoneState.Checked ? _dx.Brushes.SysTextBrushGreen : _dx.Brushes.SysTextBrushRed, 10f);
                }

                _dx.Rt?.DrawText(
                    Name,
                    _dx.Brushes.SysText74,
                    new RawRectangleF(CamWidth * 0.50f - Name.Length * 25.0f, CamHeight * 0.01f, CamWidth, CamHeight),
                    _dx.Brushes.SysTextBrushYellow);

                if (_dx.GetClip(0).Equals(Matrix3X2)) _dx.DrawLens(FrameVideo);
            }
        }
    }

    public class Zone
    {
        public RawRectangleF Rect { get; set; }
        public RawRectangleF RectN { get; set; }
        public ZoneState State { get; set; }
        public LightStates LightState { get; set; }
    }

    public enum CameraType
    {
        Full = 0,
        Up = 1,
        Left = 2,
        Right = 3,
    }
    public enum ZoneState
    {
        NotChecked = 0,
        Checked = 1,
    }
    public enum LightStates
    {
        NotSelect = 0,
        Select = 1,
    }
}
