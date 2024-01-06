
using OpenCvSharp;

namespace CbsPromPost.Other;

public class WebCam
{
    public event Action<Mat> OnNewVideoFrame = delegate { };

    public async void StartAsync(int targetFps, CancellationToken cancellationToken = default)
    {
        await Task.Run(async () =>
        {
            using var capture = new VideoCapture(0, VideoCaptureAPIs.ANY)
            {
                FrameWidth = 640,
                FrameHeight = 480,
            };

            while (!cancellationToken.IsCancellationRequested)
            {
                var start = DateTime.Now;

                try
                {
                    using var grab = new Mat(capture.FrameHeight, capture.FrameWidth, MatType.CV_8UC1, Scalar.Black);
                    if (capture.Grab()) capture.Read(grab);
                    OnNewVideoFrame.Invoke(grab);
                }
                catch
                {
                    //
                }

                var lastMs = 1000d / targetFps - (DateTime.Now - start).TotalMilliseconds;
                if (lastMs <= 0) continue;

                await Task.Delay(TimeSpan.FromMilliseconds(lastMs), cancellationToken);
            }
        }, cancellationToken);
    }
}
