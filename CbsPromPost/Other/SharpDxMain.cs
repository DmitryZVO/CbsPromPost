using SharpDX.Mathematics.Interop;
using SharpDX.Direct2D1;

namespace CbsPromPost.Other;

internal class SharpDxMain : SharpDx
{
    public SharpDxMain(PictureBox surfacePtr, int fpsTarget) : base(surfacePtr, fpsTarget, new Sprites(), 800)
    {
    }

    /*
    public sealed override async Task FrameUpdateAsync(Mat frame)
    {
        var temp = CreateDxBitmap(frame);
        if (temp is null) return;

        lock (this)
        {
            _frame.Dispose();
            _frame = temp;
            frame.Dispose();
        }

        FpsOcvC++;

        await RenderCallback();
    }
    */
    protected sealed override void DrawUser()
    {
        lock (this)
        {
            Rt?.DrawBitmap(FrameVideo, new RawRectangleF(0, 0, BaseWidth, BaseHeight), 1.0f, BitmapInterpolationMode.Linear);
        }
    }

    protected sealed override void DrawInfo()
    {
    }
}
