using System.ComponentModel;
using CbsPromPost.Model;
using CbsPromPost.Other;
using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;

namespace CbsPromPost.View;

public partial class FormDroneConfig : Form
{
    private readonly SerialBetaflight _betaflight;
    private readonly SharpDxDrone3d _dx3;
    private readonly SharpDxDrone2d _dx2;
    private bool[] _reverseSpin;
    private bool _reverseSpinAll;
    private bool _firstOpen = true;
    private bool _motorNotChecked = true;
    private readonly System.Windows.Forms.Timer _timer = new();

    public FormDroneConfig(SerialBetaflight bf)
    {
        InitializeComponent();

        _reverseSpin = new[] { false, false, false, false };
        _reverseSpinAll = false;
        _betaflight = bf;
        _dx3 = new SharpDxDrone3d(pictureBox3d, -1, bf);
        _dx2 = new SharpDxDrone2d(pictureBox2d, -1, bf);

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
        buttonPower.Click += PowerClick;
        _timer.Tick += DxRefresh;
        _timer.Interval = 100;
    }

    private void DxRefresh(object? sender, EventArgs e)
    {
        _dx2.RenderCallback();
        _dx3.RenderCallback();
    }

    private void PowerClick(object? sender, EventArgs e)
    {
        _betaflight.PowerEnabled = !_betaflight.PowerEnabled;
        Core.IoC.Services.GetRequiredService<RelayPower>().SetValues(_betaflight.PowerEnabled ? 1 : 0, _betaflight.PowerEnabled ? 1 : 0);
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

    private void AccelCalibrate(object? sender, EventArgs e) => _betaflight.MspSetCalibrateAcel();

    private async Task StartAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(50, ct);
            if (!Visible)
            {
                _firstOpen = true;
                _motorNotChecked = true;
                continue;
            }

            if (!_timer.Enabled) _timer.Start();

            Invoke(() =>
            {
                buttonPower.BackColor = _betaflight.PowerEnabled ? Color.LightGreen : Color.LightPink;
            });

            if (IsDisposed) break;

            splitContainer1.Enabled = _betaflight.IsAliveCom();
            if ((trackBarD1.Value != _dx2.Motors[0].ValuePwm) |
                (trackBarD2.Value != _dx2.Motors[1].ValuePwm) |
                (trackBarD3.Value != _dx2.Motors[2].ValuePwm) |
                (trackBarD4.Value != _dx2.Motors[3].ValuePwm)) TrackBarsUpdate();
            if (!_betaflight.IsAliveCom()) continue;

            _betaflight.MspGetAttitude();
            _betaflight.MspGetMotors();
            _betaflight.MspGetAnalog();
            //_betaflight.MspUpdateRc();
            _betaflight.MspGetImu();

            if (!_firstOpen) continue;

            _betaflight.MspSetCalibrateAcel();
            await Task.Delay(100, ct);
            _betaflight.MspSetCalibrateAcel();
            await Task.Delay(100, ct);
            _betaflight.MspSetCalibrateAcel();
            await Task.Delay(100, ct);

            _betaflight.RcPwm = new [] { 1500, 1500, 1500, 885, 1675, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500 };
            _betaflight.Roll = 0f;
            _betaflight.Pitch = 0f;
            _betaflight.Yaw = 0f;
            _betaflight.BatteryV = 0f;
            _betaflight.Amperage = 0f;
            _betaflight.GyroX = 0f;
            _betaflight.GyroY = 0f;
            _betaflight.GyroZ = 0f;
            _betaflight.AccX = 0f;
            _betaflight.AccY = 0f;
            _betaflight.AccZ = 0f;
            _betaflight.MagX = 0f;
            _betaflight.MagY = 0f;
            _betaflight.MagZ = 0f;

            _firstOpen = false;
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
        e.Cancel = true;
        var finish = false;

        if (_motorNotChecked)
        {
            if (MessageBox.Show(@"ВЫ НЕ ПРОВЕРЯЛИ ДВИГАТЕЛИ! УВЕРЕНЫ, ЧТО ХОТИТЕ ЗАВЕРШИТЬ ТЕСТЫ?", @"ВНИМАНИЕ!!!!",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3)!= DialogResult.Yes) return;
            finish = true;
        }

        if (!finish && _betaflight is { Roll: 0, Pitch: 0, Yaw: 0 })
        {
            if (MessageBox.Show(@"НЕ БЫЛО ДАННЫХ О УГЛАХ НАКЛОНА! УВЕРЕНЫ, ЧТО ХОТИТЕ ЗАВЕРШИТЬ ТЕСТЫ?", @"ВНИМАНИЕ!!!!",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3) != DialogResult.Yes) return;
            finish = true;
        }
        /*
        if (!finish && _betaflight.RcPwm.Min() == 885 && _betaflight.RcPwm.Max() == 1675 && _betaflight.RcPwm[0] == 1500)
        {
            if (MessageBox.Show(@"НЕ БЫЛО ДАННЫХ ОТ RC ПРИЕМНИКА! УВЕРЕНЫ, ЧТО ХОТИТЕ ЗАВЕРШИТЬ ТЕСТЫ?", @"ВНИМАНИЕ!!!!",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3) != DialogResult.Yes) return;
            finish = true;
        }
        */
        if (!finish && _betaflight is { GyroX: 0, GyroY: 0, GyroZ: 0, AccX: 0, AccY: 0, AccZ: 0, MagX: 0, MagY:0, MagZ: 0 })
        {
            if (MessageBox.Show(@"НЕ БЫЛО ДАННЫХ ОТ АКСЕЛЕРОМЕТРА! УВЕРЕНЫ, ЧТО ХОТИТЕ ЗАВЕРШИТЬ ТЕСТЫ?", @"ВНИМАНИЕ!!!!",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3) != DialogResult.Yes) return;
            finish = true;
        }

        if (!finish && _betaflight is { BatteryV: 0, Amperage: 0 })
        {
            if (MessageBox.Show(@"НЕ БЫЛО ДАННЫХ О НАПРЯЖЕНИИ! УВЕРЕНЫ, ЧТО ХОТИТЕ ЗАВЕРШИТЬ ТЕСТЫ?", @"ВНИМАНИЕ!!!!",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3) != DialogResult.Yes) return;
        }

        _reverseSpin = new[] { false, false, false, false };
        _reverseSpinAll = false;
        _dx2.Motors.ToList().ForEach(x => x.ValuePwm = 1000);
        _betaflight.MspSetMotor(1000, 1000, 1000, 1000);
        _timer.Stop();
        Visible = false;
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
        _motorNotChecked = false;
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
        _motorNotChecked = false;
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
        _motorNotChecked = false;
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