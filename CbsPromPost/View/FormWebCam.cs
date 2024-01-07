using System.ComponentModel;
using CbsPromPost.Other;
using OpenCvSharp;

namespace CbsPromPost.View;

public partial class FormWebCam : Form
{
    private readonly WebCam _webCam;
    private readonly SharpDxMain _dx;

    public FormWebCam()
    {
        InitializeComponent();

        _webCam = new WebCam();
        _dx = new SharpDxMain(pictureBoxMain, -1);
        Shown += ShownForm;
        Closing += ClosingForm;

        pictureBoxMain.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBoxMain.DoubleClick += PictureDoubleClick;
    }

    private void PictureDoubleClick(object? sender, EventArgs e)
    {
        WindowState = WindowState != FormWindowState.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
    }

    private void ClosingForm(object? sender, CancelEventArgs e)
    {
        _webCam.OnNewVideoFrame -= NewFrame;
        _webCam.Dispose();
        _dx.Dispose();
    }

    private async void NewFrame(Mat mat)
    {
        if (mat.Empty()) return;
        await _dx.FrameUpdateAsync(mat);
        //if (labelDroneId.Text.Equals(string.Empty)) return;
        //Cv2.ImWrite($"CAPTURE\\_{DateTime.Now.Ticks:0}.jpg", mat);
    }

    private void ShownForm(object? sender, EventArgs e)
    {
        _webCam.StartAsync(-1);
        _webCam.OnNewVideoFrame += NewFrame;
    }
}