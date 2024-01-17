using CbsPromPost.Resources;
using SharpDX.Mathematics.Interop;
using SharpDX.Direct2D1;

namespace CbsPromPost.Other;

internal class SharpDxMain : SharpDx
{
    public bool NotActive { get; set; }

    public SharpDxMain(PictureBox surfacePtr, int fpsTarget) : base(surfacePtr, fpsTarget, new Sprites(), 800)
    {
    }

    protected sealed override void DrawUser()
    {
        lock (this)
        {
            Rt?.DrawBitmap(FrameVideo, new RawRectangleF(0, 0, BaseWidth, BaseHeight), 1.0f, BitmapInterpolationMode.Linear);

            if (NotActive)
            {
                Rt?.FillRectangle(new RawRectangleF(0, 0, BaseWidth, BaseHeight), Brushes.SysTextBrushGray);
            }
        }
    }

    protected sealed override void DrawInfo()
    {
    }
}
