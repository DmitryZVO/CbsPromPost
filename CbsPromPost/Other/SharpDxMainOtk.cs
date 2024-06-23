using System.Xml;
using CbsPromPost.Resources;
using SharpDX;
using SharpDX.Mathematics.Interop;
using SharpDX.Direct2D1;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using OpenCvSharp;
using Point = SharpDX.Point;
using Rectangle = SharpDX.Rectangle;
using RectangleF = SharpDX.RectangleF;
using Size = OpenCvSharp.Size;

namespace CbsPromPost.Other;

internal class SharpDxMainOtk : SharpDx
{
    public bool NotActive { get; set; }
    protected Bitmap FrameCamLeft; // Видеокадр с левой камеры
    protected Bitmap FrameCamRight; // Видеокадр с правой камеры
    protected Bitmap FrameCamUp; // Видеокадр с верхней камеры
    public const float VWidth = 1860f;
    public const float VHeight = 889f;
    public const int CamWidth = 3840;
    public const int CamHeight = 2160;
    public int ActiveCam { get; set; }
    private PointF _mousePos = new();
    private MouseButtons _mouseButtons = MouseButtons.None;

    private readonly RectangleF _cam0 = new(0.00f, 0.00f, 0.75f, 1.00f);
    private readonly RectangleF _cam1 = new(0.75f, 0.00f, 0.25f, 0.333f);
    private readonly RectangleF _cam2 = new(0.75f, 0.333f, 0.25f, 0.333f);
    private readonly RectangleF _cam3 = new(0.75f, 0.666f, 0.25f, 0.333f);

    private readonly RawMatrix3x2 _matCam0;
    private readonly RawMatrix3x2 _matCam1;
    private readonly RawMatrix3x2 _matCam2;
    private readonly RawMatrix3x2 _matCam3;

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
        }
    }

    public void MouseMove(PointF pos, MouseButtons button)
    {
        _mousePos = pos;
        _mouseButtons = button;
    }

    public virtual void FrameCamUpUpdate(Mat frame)
    {
        var temp = CreateDxBitmap(frame);
        if (temp is null) return;

        lock (this)
        {
            FrameCamUp.Dispose();
            FrameCamUp = temp;
        }
    }

    public virtual void FrameCamLeftUpdate(Mat frame)
    {
        var temp = CreateDxBitmap(frame);
        if (temp is null) return;

        lock (this)
        {
            FrameCamLeft.Dispose();
            FrameCamLeft = temp;
        }
    }

    public virtual void FrameCamRightUpdate(Mat frame)
    {
        var temp = CreateDxBitmap(frame);
        if (temp is null) return;

        lock (this)
        {
            FrameCamRight.Dispose();
            FrameCamRight = temp;
        }
    }

    public SharpDxMainOtk(PictureBox surfacePtr, int fpsTarget) : base(surfacePtr, fpsTarget, new Sprites(), (int)VWidth)
    {
        FrameCamLeft = new Bitmap(
            Rt,
            new Size2(CamWidth, CamHeight),
            new BitmapProperties
                { PixelFormat = PixelFormat });

        FrameCamRight = new Bitmap(
            Rt,
            new Size2(CamWidth, CamHeight),
            new BitmapProperties
                { PixelFormat = PixelFormat });

        FrameCamUp = new Bitmap(
            Rt,
            new Size2(CamWidth, CamHeight),
            new BitmapProperties
                { PixelFormat = PixelFormat });

            _matCam0 = new RawMatrix3x2(
                (VWidth * _cam0.Width) / CamWidth, // Масштаб X
                0.0f, 0.0f, // Поворот X,Y
                (VHeight * _cam0.Height) / CamHeight, // Масштаб Y
                VWidth * _cam0.X, VHeight * _cam0.Y); // Сдвиг X,Y

            _matCam1 = new RawMatrix3x2(
                (VWidth * _cam1.Width) / CamWidth, // Масштаб X
                0.0f, 0.0f, // Поворот X,Y
                (VHeight * _cam1.Height) / CamHeight, // Масштаб Y
                VWidth * _cam1.X, VHeight * _cam1.Y); // Сдвиг X,Y

            _matCam2 = new RawMatrix3x2(
                (VWidth * _cam2.Width) / CamWidth, // Масштаб X
                0.0f, 0.0f, // Поворот X,Y
                (VHeight * _cam2.Height) / CamHeight, // Масштаб Y
                VWidth * _cam2.X, VHeight * _cam2.Y); // Сдвиг X,Y

            _matCam3 = new RawMatrix3x2(
                (VWidth * _cam3.Width) / CamWidth, // Масштаб X
                0.0f, 0.0f, // Поворот X,Y
                (VHeight * _cam3.Height) / CamHeight, // Масштаб Y
                VWidth * _cam3.X, VHeight * _cam3.Y); // Сдвиг X,Y
    }

    protected sealed override void DrawUser()
    {
        lock (this)
        {
            DrawCamFull(); // Общий фон
            DrawCamUp(); // Верхняя камера
            DrawCamLeft(); // Левая камера
            DrawCamRight(); // Правая камера

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
                            TransformSet(_matCam1);
                            break;
                        case 2:
                            TransformSet(_matCam2);
                            break;
                        case 3:
                            TransformSet(_matCam3);
                            break;
                    }
                    Rt?.FillRectangle(new RawRectangleF(0, 0, CamWidth, CamHeight), Brushes.RoiYellow01);
                }
            }
        }
    }

    private RawMatrix3x2 GetClip(int camNumber)
    {
        return ActiveCam switch
        {
            1 => camNumber switch
            {
                0 => _matCam1,
                1 => _matCam0,
                2 => _matCam2,
                _ => _matCam3
            },
            2 => camNumber switch
            {
                0 => _matCam2,
                1 => _matCam1,
                2 => _matCam0,
                _ => _matCam3
            },
            3 => camNumber switch
            {
                0 => _matCam3,
                1 => _matCam1,
                2 => _matCam2,
                _ => _matCam0
            },
            _ => camNumber switch
            {
                0 => _matCam0,
                1 => _matCam1,
                2 => _matCam2,
                _ => _matCam3
            }
        };
    }

    private void DrawCamFull()
    {
        TransformSet(GetClip(0));
        Rt?.DrawBitmap(FrameVideo, 1.0f, BitmapInterpolationMode.Linear);
        Rt?.DrawRectangle(new RawRectangleF(0, 0, CamWidth, CamHeight), Brushes.SysTextBrushGreen, 5);

        Rt?.DrawText(
            "ОБЩИЙ ВИД",
            Brushes.SysText104,
            new RawRectangleF(CamWidth * 0.4f, CamHeight * 0.01f, CamWidth, CamHeight),
            Brushes.SysTextBrushYellow);
    }

    private void DrawCamUp()
    {
        TransformSet(GetClip(1));
        Rt?.DrawBitmap(FrameCamUp, 1.0f, BitmapInterpolationMode.Linear);
        Rt?.DrawRectangle(new RawRectangleF(0, 0, CamWidth, CamHeight), Brushes.SysTextBrushGreen, 5);

        Rt?.DrawText(
            "ПОЛЕТНЫЙ КОНТРОЛЛЕР",
            Brushes.SysText104,
            new RawRectangleF(CamWidth * 0.33f, CamHeight * 0.01f, CamWidth, CamHeight),
            Brushes.SysTextBrushYellow);
    }

    private void DrawCamLeft()
    {
        TransformSet(GetClip(2));
        Rt?.DrawBitmap(FrameCamLeft, 1.0f, BitmapInterpolationMode.Linear);
        Rt?.DrawRectangle(new RawRectangleF(0, 0, CamWidth, CamHeight), Brushes.SysTextBrushGreen, 5);

        Rt?.DrawText(
            "РЕГУЛЯТОР СЛЕВА",
            Brushes.SysText104,
            new RawRectangleF(CamWidth * 0.38f, CamHeight * 0.01f, CamWidth, CamHeight),
            Brushes.SysTextBrushYellow);
    }

    private void DrawCamRight()
    {
        TransformSet(GetClip(3));
        Rt?.DrawBitmap(FrameCamRight, 1.0f, BitmapInterpolationMode.Linear);
        Rt?.DrawRectangle(new RawRectangleF(0, 0, CamWidth, CamHeight), Brushes.SysTextBrushGreen, 5);


        Rt?.DrawText(
            "РЕГУЛЯТОР СПРАВА",
            Brushes.SysText104,
            new RawRectangleF(CamWidth * 0.38f, CamHeight * 0.01f, CamWidth, CamHeight),
            Brushes.SysTextBrushYellow);
    }

    protected sealed override void DrawInfo()
    {
    }
}
