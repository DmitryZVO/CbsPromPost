using System.ComponentModel;
using CbsPromPost.Model;
using CbsPromPost.Other;
using OpenCvSharp;

namespace CbsPromPost.View;

public partial class FormDroneConfig : Form
{
    private readonly SerialBetaflight _betaflight;
    private readonly SharpDxDrone _dx;

    public FormDroneConfig(SerialBetaflight bf)
    {
        InitializeComponent();

        _betaflight = bf;
        _dx = new SharpDxDrone(pictureBoxMain, 30, bf);
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
        _dx.Dispose();
    }

    private void ShownForm(object? sender, EventArgs e)
    {
        _ = _dx.StartAsync();
    }
}