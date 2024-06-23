using OpenCvSharp;
using OpenCvSharp.Extensions;
using Size = OpenCvSharp.Size;

namespace CbsPromPost.Other;

public class WebCamOtk
{
    private bool _exit;

    public event Action<Mat> OnNewVideoFrame = delegate { };
    public byte Focus { get; set; } = 255;

    public async void StartAsyncRtsp(string rtsp, int targetFps, CancellationToken cancellationToken = default)
    {
        await Task.Run(async () =>
        {
            Environment.SetEnvironmentVariable("OPENCV_VIDEOIO_MSMF_ENABLE_HW_TRANSFORMS", "0");
            Environment.SetEnvironmentVariable("MF_SOURCE_READER_D3D_MANAGER", "1");
            using var capture = new VideoCapture(rtsp);
            capture.Set(VideoCaptureProperties.AutoFocus, 0);
            var focus = capture.Get(VideoCaptureProperties.Focus);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (_exit) break;

                var start = DateTime.Now;

                if (Math.Abs(focus - Focus) > 1d)
                {
                    capture.Set(VideoCaptureProperties.Focus, Focus);
                    focus = capture.Get(VideoCaptureProperties.Focus);
                }

                try
                {
                    using var grab = new Mat(capture.FrameHeight, capture.FrameWidth, MatType.CV_8UC1, Scalar.Black);
                    if (capture.Grab())
                    {
                        capture.Read(grab);
                        using var grabNormal = grab.Resize(new Size(3840, 2160));
                        OnNewVideoFrame.Invoke(grabNormal);
                    }
                    else
                    {
                        await Task.Delay(1000, cancellationToken);
                        continue;
                    }
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

    public async void StartAsync(int camNumber, int targetFps, CancellationToken cancellationToken = default)
    {
        await Task.Run(async () =>
        {
            Environment.SetEnvironmentVariable("OPENCV_VIDEOIO_MSMF_ENABLE_HW_TRANSFORMS", "0");
            Environment.SetEnvironmentVariable("MF_SOURCE_READER_D3D_MANAGER", "1");
            using var capture = new VideoCapture(camNumber, VideoCaptureAPIs.MSMF);
            capture.FrameWidth = 3840;
            capture.FrameHeight = 2160;
            capture.Set(VideoCaptureProperties.AutoFocus, 0);
            var focus = capture.Get(VideoCaptureProperties.Focus);
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_exit) break;   

                var start = DateTime.Now;

                if (Math.Abs(focus - Focus) > 1d)
                {
                    capture.Set(VideoCaptureProperties.Focus, Focus);
                    focus = capture.Get(VideoCaptureProperties.Focus);
                }

                try
                {
                    using var grab = new Mat(capture.FrameHeight, capture.FrameWidth, MatType.CV_8UC1, Scalar.Black);
                    if (capture.Grab())
                    {
                        capture.Read(grab);
                        OnNewVideoFrame.Invoke(grab);
                    }
                    else
                    {
                        await Task.Delay(1000, cancellationToken);
                        continue;
                    }
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
    public void Dispose()
    {
        _exit = true;
    }
}
