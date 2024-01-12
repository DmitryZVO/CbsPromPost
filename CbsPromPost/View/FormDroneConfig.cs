using System.ComponentModel;
using CbsPromPost.Model;
using CbsPromPost.Other;
using OpenCvSharp;

namespace CbsPromPost.View;

public partial class FormDroneConfig : Form
{
    private readonly SerialBetaflight _betaflight;
    private readonly SharpDxDrone3d _dx3;
    private readonly SharpDxDrone2d _dx2;
    private bool[] _reverseSpin;
    private bool _reverseSpinAll;

    public FormDroneConfig(SerialBetaflight bf)
    {
        InitializeComponent();

        _reverseSpin = new[] { false, false, false, false };
        _reverseSpinAll = false;
        _betaflight = bf;
        _dx3 = new SharpDxDrone3d(pictureBox3d, 30, bf);
        _dx2 = new SharpDxDrone2d(pictureBox2d, 30, bf);

        Shown += ShownForm;
        Closing += ClosingForm;

        buttonMotors1000.Click += MotorsSet1000;
        buttonMotors1100.Click += MotorsSet1100;
        buttonMotors1250.Click += MotorsSet1250;
        buttonMotors1500.Click += MotorsSet1500;
        trackBarMotors.ValueChanged += ValuesMotorsChanged;
        trackBarD1.ValueChanged += ValueMotorD1Changed;
        trackBarD2.ValueChanged += ValueMotorD2Changed;
        trackBarD3.ValueChanged += ValueMotorD3Changed;
        trackBarD4.ValueChanged += ValueMotorD4Changed;
        buttonAcelCalibrate.Click += AccelCalibrate;
        buttonD1Inv.Click += Motor1SpinInv;
        buttonD2Inv.Click += Motor2SpinInv;
        buttonD3Inv.Click += Motor3SpinInv;
        buttonD4Inv.Click += Motor4SpinInv;
        buttonInverseAll.Click += MotorsInvAll;
        button1010.Click += MotorMinimum;
    }

    private async void MotorsInvAll(object? sender, EventArgs e)
    {
        await _betaflight.MspSetReverse(255, _reverseSpinAll);
        _reverseSpinAll = !_reverseSpinAll;
        _reverseSpin[0] = !_reverseSpin[0];
        _reverseSpin[1] = !_reverseSpin[1];
        _reverseSpin[2] = !_reverseSpin[2];
        _reverseSpin[3] = !_reverseSpin[3];
    }

    private void MotorMinimum(object? sender, EventArgs e)
    {
        _dx2.Motors.ToList().ForEach(x => x.ValuePwm = 1020);
        TrackBarsUpdate();
    }

    private async void Motor1SpinInv(object? sender, EventArgs e)
    {
        await _betaflight.MspSetReverse(0, _reverseSpin[0]);
        _reverseSpin[0] = !_reverseSpin[0];
    }

    private async void Motor2SpinInv(object? sender, EventArgs e)
    {
        await _betaflight.MspSetReverse(1, _reverseSpin[1]);
        _reverseSpin[1] = !_reverseSpin[1];
    }

    private async void Motor3SpinInv(object? sender, EventArgs e)
    {
        await _betaflight.MspSetReverse(2, _reverseSpin[2]);
        _reverseSpin[2] = !_reverseSpin[2];
    }

    private async void Motor4SpinInv(object? sender, EventArgs e)
    {
        await _betaflight.MspSetReverse(3, _reverseSpin[3]);
        _reverseSpin[3] = !_reverseSpin[3];
    }

    private void AccelCalibrate(object? sender, EventArgs e) => _betaflight.MspCalibrateAcel();

    private async Task StartAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(10, ct);
            if (!Visible) continue;

            if (IsDisposed) break;
            splitContainer1.Enabled = _betaflight.IsAlive();
            if ((trackBarD1.Value != _dx2.Motors[0].ValuePwm) |
                (trackBarD2.Value != _dx2.Motors[1].ValuePwm) |
                (trackBarD3.Value != _dx2.Motors[2].ValuePwm) |
                (trackBarD4.Value != _dx2.Motors[3].ValuePwm)) TrackBarsUpdate();
            if (!_betaflight.IsAlive()) continue;

            await _betaflight.MspUpdateAttitude(50);
            await _betaflight.MspUpdateMotors(50);
            await _betaflight.MspUpdateAnalog(50);
            await _betaflight.MspUpdateRc(50);
            await _betaflight.MspUpdateImu(50);
        }
    }

    private void ValueMotorD1Changed(object? sender, EventArgs e)
    {
        _dx2.Motors[0].ValuePwm = trackBarD1.Value;
        TrackBarsUpdate();
    }

    private void ValueMotorD2Changed(object? sender, EventArgs e)
    {
        _dx2.Motors[1].ValuePwm = trackBarD2.Value;
        TrackBarsUpdate();
    }

    private void ValueMotorD3Changed(object? sender, EventArgs e)
    {
        _dx2.Motors[2].ValuePwm = trackBarD3.Value;
        TrackBarsUpdate();
    }

    private void ValueMotorD4Changed(object? sender, EventArgs e)
    {
        _dx2.Motors[3].ValuePwm = trackBarD4.Value;
        TrackBarsUpdate();
    }

    private void ClosingForm(object? sender, CancelEventArgs e)
    {
        _reverseSpin = new[] { false, false, false, false };
        _reverseSpinAll = false;
        _dx2.Motors.ToList().ForEach(x => x.ValuePwm = 1000);
        _betaflight.MspSetMotor(1000, 1000, 1000, 1000);
        Visible = false;
        e.Cancel = true;
        //_dx3.Dispose();
        //_dx2.Dispose();
    }

    private void ShownForm(object? sender, EventArgs e)
    {
        _ = _dx3.StartAsync();
        _ = _dx2.StartAsync();
        _ = StartAsync();
    }

    private void ValuesMotorsChanged(object? sender, EventArgs e)
    {
        _dx2.Motors.ToList().ForEach(x => x.ValuePwm = trackBarMotors.Value);
        TrackBarsUpdate();
    }

    private void TrackBarsUpdate()
    {
        var updateMotors = _dx2.Motors[0].ValuePwm == _dx2.Motors[1].ValuePwm &&
                           _dx2.Motors[1].ValuePwm == _dx2.Motors[2].ValuePwm &&
                           _dx2.Motors[2].ValuePwm == _dx2.Motors[3].ValuePwm;
        trackBarD1.Value = _dx2.Motors[0].ValuePwm;
        trackBarD2.Value = _dx2.Motors[1].ValuePwm;
        trackBarD3.Value = _dx2.Motors[2].ValuePwm;
        trackBarD4.Value = _dx2.Motors[3].ValuePwm;
        if (updateMotors) trackBarMotors.Value = _dx2.Motors[0].ValuePwm;

        _betaflight.MspSetMotor(_dx2.Motors[0].ValuePwm, _dx2.Motors[1].ValuePwm, _dx2.Motors[2].ValuePwm, _dx2.Motors[3].ValuePwm);
    }
    private async void MotorsSet1000(object? sender, EventArgs e)
    {
        var max = _dx2.Motors.ToList().Max(x => x.ValuePwm);
        if (max <= 1000)
        {
            _dx2.Motors.ToList().ForEach(x => x.ValuePwm = 1000);
            TrackBarsUpdate();
        }

        for (var ii = max; ii > 1000; ii -= 10)
        {
            _dx2.Motors.ToList().ForEach(x => x.ValuePwm = ii);
            TrackBarsUpdate();
            await Task.Delay(20);
        }

        _dx2.Motors.ToList().ForEach(x => x.ValuePwm = 1000);
        TrackBarsUpdate();
    }
    private async void MotorsSet1100(object? sender, EventArgs e)
    {
        var min = _dx2.Motors.ToList().Min(x => x.ValuePwm);
        if (min >= 1100)
        {
            _dx2.Motors.ToList().ForEach(x => x.ValuePwm = 1100);
            TrackBarsUpdate();
        }

        for (var i = min; i < 1100; i += 10)
        {
            _dx2.Motors.ToList().ForEach(x => x.ValuePwm = i);
            TrackBarsUpdate();
            await Task.Delay(20);
        }
    }
    private async void MotorsSet1250(object? sender, EventArgs e)
    {
        var min = _dx2.Motors.ToList().Min(x => x.ValuePwm);
        if (min >= 1250)
        {
            _dx2.Motors.ToList().ForEach(x => x.ValuePwm = 1250);
            TrackBarsUpdate();
        }

        for (var i = min; i < 1250; i += 10)
        {
            _dx2.Motors.ToList().ForEach(x => x.ValuePwm = i);
            TrackBarsUpdate();
            await Task.Delay(20);
        }
    }
    private async void MotorsSet1500(object? sender, EventArgs e)
    {
        var min = _dx2.Motors.ToList().Min(x => x.ValuePwm);
        if (min >= 1500)
        {
            _dx2.Motors.ToList().ForEach(x => x.ValuePwm = 1500);
            TrackBarsUpdate();
        }

        for (var i = min; i < 1500; i += 10)
        {
            _dx2.Motors.ToList().ForEach(x => x.ValuePwm = i);
            TrackBarsUpdate();
            await Task.Delay(20);
        }
    }

    public void UpdateFrame(Mat mat)
    {
        using var mat2 = mat.CvtColor(ColorConversionCodes.BGR2RGB);
        _dx3.FrameUpdate(mat2);
    }
}