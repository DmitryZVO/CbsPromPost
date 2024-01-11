using SharpDX.Mathematics.Interop;
using SharpDX.Direct2D1;
using CbsPromPost.Model;
using CbsPromPost.Resources;
using SharpDX;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Brush = SharpDX.Direct2D1.Brush;

namespace CbsPromPost.Other;

internal class SharpDxDrone2d : SharpDx
{
    public Motor[] Motors { get; set; } = new Motor[4];

    private readonly SerialBetaflight _betaflight;
    public SharpDxDrone2d(PictureBox surfacePtr, int fpsTarget, SerialBetaflight bf) : base(surfacePtr, fpsTarget, new Sprites(), 800)
    {
        _betaflight = bf;
        Motors[0] = new Motor
        {
            Angle = 0f, 
            Bitmap = Sprites.Items["CW"], 
            Cw = true,
            Position = new RawVector2(BaseWidth * 0.735f, BaseHeight * 0.54f),
            Brush = Brushes.SysTextBrushYellow,
        };
        Motors[1] = new Motor
        {
            Angle = 0f,
            Bitmap = Sprites.Items["CCW"],
            Cw = false,
            Position = new RawVector2(BaseWidth * 0.705f, BaseHeight * 0.11f),
            Brush = Brushes.SysTextBrushGreen,
        };
        Motors[2] = new Motor
        {
            Angle = 0f,
            Bitmap = Sprites.Items["CCW"],
            Cw = false,
            Position = new RawVector2(BaseWidth * 0.27f, BaseHeight * 0.54f),
            Brush = Brushes.SysTextBrushRed,
        };
        Motors[3] = new Motor
        {
            Angle = 0f,
            Bitmap = Sprites.Items["CW"],
            Cw = true,
            Position = new RawVector2(BaseWidth * 0.30f, BaseHeight * 0.12f),
            Brush = Brushes.SysTextBrushBlue,
        };
    }

    protected sealed override void DrawUser()
    {
        MotorsUpdate();
        lock (this)
        {
            Rt?.Clear(new RawColor4(1.0f, 1.0f, 1.0f, 1.0f));
            var vt40 = Sprites.Items["VT40"];
            Rt?.DrawBitmap(vt40,
                new RawRectangleF(BaseWidth * 0.5f - vt40.Size.Width*0.3f, BaseHeight * 0.35f - vt40.Size.Height * 0.3f, BaseWidth * 0.5f + vt40.Size.Width * 0.3f,
                    BaseHeight * 0.35f + vt40.Size.Height * 0.3f), 1.0f, BitmapInterpolationMode.Linear);
            var cw = Sprites.Items["CW"];
            var ccw = Sprites.Items["CCW"];

            TransformSet(Matrix3x2.Rotation(Motors[0].Angle, Motors[0].Position));
            Rt?.DrawBitmap(cw,
                new RawRectangleF(Motors[0].Position.X - cw.Size.Width * 0.3f, Motors[0].Position.Y - cw.Size.Height * 0.3f,
                    Motors[0].Position.X + cw.Size.Width * 0.3f, Motors[0].Position.Y + cw.Size.Height * 0.3f),
                0.8f, BitmapInterpolationMode.Linear);
            TransformSet(ZeroTransform);

            TransformSet(Matrix3x2.Rotation(Motors[3].Angle, Motors[3].Position));
            Rt?.DrawBitmap(cw,
                new RawRectangleF(Motors[3].Position.X - cw.Size.Width * 0.3f, Motors[3].Position.Y - cw.Size.Height * 0.3f,
                    Motors[3].Position.X + cw.Size.Width * 0.3f, Motors[3].Position.Y + cw.Size.Height * 0.3f),
                0.8f, BitmapInterpolationMode.Linear);
            TransformSet(ZeroTransform);

            TransformSet(Matrix3x2.Rotation(Motors[1].Angle, Motors[1].Position));
            Rt?.DrawBitmap(ccw,
                new RawRectangleF(Motors[1].Position.X - ccw.Size.Width * 0.3f, Motors[1].Position.Y - ccw.Size.Height * 0.3f,
                    Motors[1].Position.X + ccw.Size.Width * 0.3f, Motors[1].Position.Y + ccw.Size.Height * 0.3f),
                0.8f, BitmapInterpolationMode.Linear);
            TransformSet(ZeroTransform);

            TransformSet(Matrix3x2.Rotation(Motors[2].Angle, Motors[2].Position));
            Rt?.DrawBitmap(ccw,
                new RawRectangleF(Motors[2].Position.X - ccw.Size.Width * 0.3f, Motors[2].Position.Y - ccw.Size.Height * 0.3f,
                    Motors[2].Position.X + ccw.Size.Width * 0.3f, Motors[2].Position.Y + ccw.Size.Height * 0.3f),
                0.8f, BitmapInterpolationMode.Linear);
            TransformSet(ZeroTransform);

            Rt?.DrawText(
                "Д1", Brushes.SysText34,
                new RawRectangleF(Motors[0].Position.X - 20, Motors[0].Position.Y - 20, BaseWidth, BaseHeight),
                Motors[0].Brush);
            Rt?.DrawText(
                Motors[0].ValuePwm.ToString("0"), Brushes.SysText20,
                new RawRectangleF(Motors[0].Position.X - 25, Motors[0].Position.Y - 85, BaseWidth, BaseHeight),
                Brushes.SysTextBrushBlack);
            Rt?.DrawText(
                "Д2", Brushes.SysText34,
                new RawRectangleF(Motors[1].Position.X - 20, Motors[1].Position.Y - 20, BaseWidth, BaseHeight),
                Motors[1].Brush);
            Rt?.DrawText(
                Motors[1].ValuePwm.ToString("0"), Brushes.SysText20,
                new RawRectangleF(Motors[1].Position.X - 25, Motors[1].Position.Y - 85, BaseWidth, BaseHeight),
                Brushes.SysTextBrushBlack);
            Rt?.DrawText(
                "Д3", Brushes.SysText34,
                new RawRectangleF(Motors[2].Position.X - 20, Motors[2].Position.Y - 20, BaseWidth, BaseHeight),
                Motors[2].Brush);
            Rt?.DrawText(
                Motors[2].ValuePwm.ToString("0"), Brushes.SysText20,
                new RawRectangleF(Motors[2].Position.X - 25, Motors[2].Position.Y - 85, BaseWidth, BaseHeight),
                Brushes.SysTextBrushBlack);
            Rt?.DrawText(
                "Д4", Brushes.SysText34,
                new RawRectangleF(Motors[3].Position.X - 20, Motors[3].Position.Y - 20, BaseWidth, BaseHeight),
                Motors[3].Brush);
            Rt?.DrawText(
                Motors[3].ValuePwm.ToString("0"), Brushes.SysText20,
                new RawRectangleF(Motors[3].Position.X - 25, Motors[3].Position.Y - 85, BaseWidth, BaseHeight),
                Brushes.SysTextBrushBlack);

            // Отрисовка моторов
            const float rtW = 20f;
            const float rtH = 100f;
            Rt?.FillRectangle(
                new RawRectangleF(Motors[0].Position.X - rtW / 2f - 70f - 2f, Motors[0].Position.Y - rtH / 2f - 2f,
                    Motors[0].Position.X + rtW / 2f - 70f + 2f, Motors[0].Position.Y + rtH / 2f + 2f),
                Brushes.RoiNone);
            Rt?.FillRectangle(
                new RawRectangleF(Motors[0].Position.X - rtW / 2f - 70f, Motors[0].Position.Y + rtH / 2f + rtH*((_betaflight.MotorsPwm[0]-1000f)/-1000f),
                    Motors[0].Position.X + rtW / 2f - 70f, Motors[0].Position.Y + rtH / 2f), Brushes.SysTextBrushGreen);

            Rt?.FillRectangle(
                new RawRectangleF(Motors[1].Position.X - rtW / 2f - 70f - 2f, Motors[1].Position.Y - rtH / 2f - 2f,
                    Motors[1].Position.X + rtW / 2f - 70f + 2f, Motors[1].Position.Y + rtH / 2f + 2f),
                Brushes.RoiNone);
            Rt?.FillRectangle(
                new RawRectangleF(Motors[1].Position.X - rtW / 2f - 70f, Motors[1].Position.Y + rtH / 2f + rtH * ((_betaflight.MotorsPwm[1] - 1000f) / -1000f),
                    Motors[1].Position.X + rtW / 2f - 70f, Motors[1].Position.Y + rtH / 2f), Brushes.SysTextBrushGreen);

            Rt?.FillRectangle(
                new RawRectangleF(Motors[2].Position.X - rtW / 2f + 70f - 2f, Motors[2].Position.Y - rtH / 2f - 2f,
                    Motors[2].Position.X + rtW / 2f + 70f + 2f, Motors[2].Position.Y + rtH / 2f + 2f),
                Brushes.RoiNone);
            Rt?.FillRectangle(
                new RawRectangleF(Motors[2].Position.X - rtW / 2f + 70f, Motors[2].Position.Y + rtH / 2f + rtH * ((_betaflight.MotorsPwm[2] - 1000f) / -1000f),
                    Motors[2].Position.X + rtW / 2f + 70f, Motors[2].Position.Y + rtH / 2f), Brushes.SysTextBrushGreen);

            Rt?.FillRectangle(
                new RawRectangleF(Motors[3].Position.X - rtW / 2f + 70f - 2f, Motors[3].Position.Y - rtH / 2f - 2f,
                    Motors[3].Position.X + rtW / 2f + 70f + 2f, Motors[3].Position.Y + rtH / 2f + 2f),
                Brushes.RoiNone);
            Rt?.FillRectangle(
                new RawRectangleF(Motors[3].Position.X - rtW / 2f + 70f, Motors[3].Position.Y + rtH / 2f + rtH * ((_betaflight.MotorsPwm[3] - 1000f) / -1000f),
                    Motors[3].Position.X + rtW / 2f + 70f, Motors[3].Position.Y + rtH / 2f), Brushes.SysTextBrushGreen);

            Rt?.DrawText(
                $"V: {_betaflight.BatteryV:00.00}", Brushes.SysText20,
                new RawRectangleF(BaseWidth * 0.46f, BaseHeight * 0.01f, BaseWidth, BaseHeight),
                Brushes.SysTextBrushDarkGreen);
            Rt?.DrawText(
                $"A: {_betaflight.Amperage:#0.00}", Brushes.SysText20,
                new RawRectangleF(BaseWidth * 0.46f, BaseHeight * 0.05f, BaseWidth, BaseHeight),
                Brushes.SysTextBrushRed);

            var posCx = BaseWidth * 0.20f;
            var posCy = BaseHeight * 0.86f;
            const float wC = 60f;
            const float hC = 100f;
            Rt?.FillRectangle(new RawRectangleF(posCx, posCy, posCx + wC * 8, posCy + hC), Brushes.RoiNone);
            for (var i = 0; i < 8; i++)
            {
                var vP = Math.Min(Math.Max(2000 - _betaflight.RcPwm[i], 0), 1000);
                var proc = (vP / 1000f) * hC;
                Rt?.FillRectangle(new RawRectangleF(
                    posCx + wC*i, posCy + proc, posCx + wC * (i+1), posCy + hC), Brushes.RoiGreen03);
                Rt?.DrawRectangle(new RawRectangleF(
                    posCx + wC * i, posCy, posCx + wC * (i + 1), posCy + hC), Brushes.SysTextBrushBlack, 2);
                Rt?.DrawText(
                    $"CH{i:00}", Brushes.SysText14,
                    new RawRectangleF(posCx + wC * i + 12f, posCy +hC - 20f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushYellow);
                Rt?.DrawText(
                    $"{_betaflight.RcPwm[i]:0000}", Brushes.SysText14,
                    new RawRectangleF(posCx + wC * i + 12f, posCy + 5f, BaseWidth, BaseHeight),
                    Brushes.SysTextBrushWhite);
            }

            if (!_betaflight.IsAlive())
            {
                Rt?.FillRectangle(new RawRectangleF(0, 0, BaseWidth, BaseHeight), Brushes.RoiNone);
            }
        }
    }

    private void MotorsUpdate()
    {
        if (!_betaflight.IsAlive())
        {
            Motors.ToList().ForEach(x => x.ValuePwm = 1000);
            Motors.ToList().ForEach(x=>x.Angle = 0f);
            return;
        }
        const float angleStep = 0.001f;
        Motors.ToList().ForEach(x => x.Angle += (x.ValuePwm - 1000) * angleStep * (x.Cw ? 1f : -1f));
        Motors.ToList().ForEach(x => x.Angle = (float)(x.Angle % (Math.PI * 2f)));
    }

    protected sealed override void DrawInfo()
    {
    }

    public class Motor
    {
        public RawVector2 Position { get; set; }
        public float Angle { get; set; }
        public bool Cw { get; set; } = true;
        public Bitmap? Bitmap { get; set; }
        public Brush? Brush { get; set; }
        public int ValuePwm { get; set; } = 1000;
    }
}