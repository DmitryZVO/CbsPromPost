using Microsoft.Extensions.Logging;
using System.IO.Ports;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace CbsPromPost.Model;

public partial class SerialBetaflight
{
    private string _serial;
    private readonly ILogger<SerialBetaflight> _logger;
    private bool _alive;
    private SerialPort _port = new();

    public int ProgressValue { get; private set; }

    public bool IsAlive() => _alive;

    private bool _aliveDfu;
    public bool IsAliveDfu() => _aliveDfu;
    private UsbDevice? _usbDfu;

    public SerialBetaflight(ILogger<SerialBetaflight> logger)
    {
        _logger = logger;
        _serial = string.Empty;
    }


    public async Task StartUsbAsync(int vid, int pid, CancellationToken cancellationToken = default)
    {

        var usbFinder = new UsbDeviceFinder(vid, pid);
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);

            lock (this)
            {
                _usbDfu = UsbDevice.OpenUsbDevice(usbFinder);
                if (_usbDfu == null)
                {
                    _aliveDfu = false;
                    continue;
                }
            }

            _aliveDfu = true;

            var errorsCount = 0;
            while (true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);

                if ((await DfuGetStatus()).BwPollTimeout < 0)
                {
                    errorsCount++;
                    if (errorsCount > 6) break;
                }
                else
                {
                    errorsCount = 0;
                }
            }

            lock (this)
            {
                _usbDfu.Close();
            }

            _aliveDfu = false;
        }
    }

    public async Task StartAsync(string com, CancellationToken cancellationToken = default)
    {
        _serial = com;
        _port = new SerialPort(_serial, 115200, Parity.None, 8, StopBits.One);

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);

            try
            {
                _port.Open();
                _alive = true;
                while (_port.IsOpen)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("SERIAL_EXEPTION_{Serial}: {Ex}", _serial, ex.Message);
            }
            _port.Close();

            _alive = false;
        }
    }

    public async Task<List<string>> CliWrite(string text, int waitMs=200)
    {
        var ret = new List<string>();
        if (!_port.IsOpen) return ret;

        ProgressValue = 0;

        _port.WriteLine(text);
        do
        {
            ret.Add(_port.ReadExisting());
            await Task.Delay(TimeSpan.FromMilliseconds(waitMs));
            ProgressValue += 20;

            if (_port.IsOpen) continue;
            ret.Add("\r\n***RESTART***\r\n");
            break;
        } while (_port is { IsOpen: true, BytesToRead: > 0 });

        ProgressValue = 0;
        return ret;
    }

    public class DfuStatus
    {
        public byte BStatus = 0xFF;       // state during request
        public int BwPollTimeout = -1;  // minimum time in ms before next getStatus call should be made
        public DfuState BState = DfuState.Unknown; // state after request
    }

    public enum DfuState : byte
    {
        Idle = 0x00,
        Detach = 0x01,
        DfuIdle = 0x02,
        DfuDownloadSync = 0x03,
        DfuDownloadBusy = 0x04,
        DfuDownloadIdle = 0x05,
        DfuManifestSync = 0x06,
        DfuManifest = 0x07,
        DfuManifestWaitReset = 0x08,
        DfuUploadIdle = 0x09,
        DfuError = 0x0A,
        DfuUploadSync = 0x91,
        DfuUploadBusy = 0x92,
        Unknown = 0xFF,
    }
}