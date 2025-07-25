﻿using OpenCvSharp;

namespace CbsPromPost.Other;

public class WebCam
{
    private bool _exit;

    public event Action<Mat> OnNewVideoFrame = delegate { };

    public async void StartAsync(int camId, int targetFps, CancellationToken cancellationToken = default)
    {
        await Task.Run(async () =>
        {
            Environment.SetEnvironmentVariable("OPENCV_VIDEOIO_MSMF_ENABLE_HW_TRANSFORMS", "0");
            Environment.SetEnvironmentVariable("MF_SOURCE_READER_D3D_MANAGER", "1");
            using var capture = new VideoCapture(camId, VideoCaptureAPIs.MSMF) { FrameWidth = 640, FrameHeight = 480 };
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_exit) break;   

                var start = DateTime.Now;

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
