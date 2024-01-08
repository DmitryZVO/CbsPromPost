using SharpDX.Mathematics.Interop;
using SharpDX.Direct2D1;
using CbsPromPost.Model;

namespace CbsPromPost.Other;

internal class SharpDxDrone : SharpDx
{
    private readonly SerialBetaflight _betaflight;
    public SharpDxDrone(PictureBox surfacePtr, int fpsTarget, SerialBetaflight bf) : base(surfacePtr, fpsTarget, new Sprites(), 1200)
    {
        _betaflight = bf;
    }

    protected sealed override void DrawUser()
    {
        lock (this)
        {
            Rt?.Clear(new RawColor4(0, 0, 0, 1));
            Rt?.FillEllipse(new Ellipse(new RawVector2(BaseWidth * 0.98f, BaseHeight * 0.025f), 10f, 10f),
                _betaflight.IsAlive() ? Brushes.RoiGreen03 : Brushes.RoiRed03);
        }
    }

    protected sealed override void DrawInfo()
    {
    }
}
