using System.ComponentModel;
using Microsoft.Extensions.Logging;
using System.IO.Ports;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace CbsPromPost.Model;

public class SerialBetaflight
{
    public class DfuStatus
    {
        public byte BStatus;       // state during request
        public int BwPollTimeout = -1;  // minimum time in ms before next getStatus call should be made
        public byte BState;        // state after request
    }

    private readonly int _dfuAddress = 0x08000000;
    private string _serial;
    private readonly ILogger<SerialBetaflight> _logger;
    private bool _alive;
    private readonly SemaphoreSlim _sem;
    private SerialPort _port = new();

    public bool IsAlive() => _alive;

    private bool _aliveDfu;
    public bool IsAliveDfu() => _aliveDfu;
    private UsbDevice? _usbDfu;

    public List<string> DfuExit()
    {
        var ret = new List<string>();
        DfuSetAddressPointer(_dfuAddress);
        DfuClearStatus();
        DfuWrite(Array.Empty<byte>());
        DfuGetStatus();
        ret.Add("\r\n***EXIT DFU MODE & RESTART***\r\n");
        return ret;
    }

    public SerialBetaflight(ILogger<SerialBetaflight> logger)
    {
        _sem = new SemaphoreSlim(1);
        _logger = logger;
        _serial = string.Empty;
    }

    private int DfuSetAddressPointer(long address) //throws Exception //was int
    {
        var buffer = new byte[5];
        buffer[0] = 0x21;
        buffer[1] = (byte)(address & 0xFF);
        buffer[2] = (byte)((address >> 8) & 0xFF);
        buffer[3] = (byte)((address >> 16) & 0xFF);
        buffer[4] = (byte)((address >> 24) & 0xFF);
        return DfuWrite(buffer);
    }

    private int DfuClearStatus()
    {
        if (!_aliveDfu) return -1;
        if (_usbDfu == null) return -1;

        var packet = new UsbSetupPacket
        {
            RequestType = 0x21,
            Request = 0x04, // DFU_CLRSTATUS
            Value = 0,
            Index = 0
        };

        try
        {
            _sem.Wait();
            _usbDfu.ControlTransfer(ref packet, null, 0, out var lenWrite);
            _sem.Release();
            return lenWrite;
        }
        catch
        {
            //
        }
        return -1;
    }

    private DfuStatus DfuGetStatus() //throws Exception
    {
        var ret = new DfuStatus();
        if (!_aliveDfu) return ret;
        if (_usbDfu == null) return ret;

        var buffer = new byte[6];
        var packet = new UsbSetupPacket
        {
            RequestType = 0x21 | 0x80, // IN
            Request = 0x03, // DFU_GETSTATUS
            Value = 0,
            Index = 0
        };

        try
        {
            _sem.Wait();
            _usbDfu.ControlTransfer(ref packet, buffer, buffer.Length, out var lenWrite);
            _sem.Release();

            if (lenWrite <= 0) return ret;

            ret.BStatus = buffer[0]; // state during request
            ret.BState = buffer[4]; // state after request
            ret.BwPollTimeout = (buffer[3] & 0xFF) << 16;
            ret.BwPollTimeout |= (buffer[2] & 0xFF) << 8;
            ret.BwPollTimeout |= (buffer[1] & 0xFF);
        }
        catch
        {
            //
        }

        return ret;
    }

    private int DfuWrite(IReadOnlyCollection<byte> data) //throws Exception
    {
        if (!_aliveDfu) return -1;
        if (_usbDfu == null) return -1;

        var packet = new UsbSetupPacket()
        {
            RequestType = 0x21, // '2' => Class request ; '1' => to interface
            Request = 0x01, // DFU_DNLOAD
            Value = 0,
            Index = 0
        };

        try
        {
            _sem.Wait();
            _usbDfu.ControlTransfer(ref packet, data, data.Count, out var lenWrite);
            _sem.Release();
            return lenWrite;
        }
        catch
        {
            //
        }
        return -1;
    }

    public async void StartUsbAsync(int vid, int pid, CancellationToken cancellationToken = default)
    {

        var usbFinder = new UsbDeviceFinder(vid, pid);
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);

            _usbDfu = UsbDevice.OpenUsbDevice(usbFinder);
            if (_usbDfu == null)
            {
                _aliveDfu = false;
                continue;
            }
            _aliveDfu = true;

            while (_usbDfu.IsOpen)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);
                if (DfuGetStatus().BwPollTimeout < 0) break;
            }
            _usbDfu.Close();
            

            _aliveDfu = false;
        }
    }

    public async void StartAsync(string com, CancellationToken cancellationToken = default)
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
        _sem.Dispose();
    }

    private async Task<List<string>> CliEnter()
    {
        var ret = new List<string>();
        await _sem.WaitAsync();
        _port.WriteLine("#");
        do
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            ret.Add(_port.ReadExisting());
        } while (_port.BytesToRead > 0);
        _sem.Release();
        return ret;
    }

    private async Task<List<string>> CliExit()
    {
        var ret = new List<string>();
        await _sem.WaitAsync();
        _port.WriteLine("exit");
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        if (!_port.IsOpen)
        {
            ret.Add("\r\n***RESTART***\r\n");
        }
        _sem.Release();
        return ret;
    }

    public async Task<List<string>> CliWrite(string text)
    {
        var ret = new List<string>();
        if (!_port.IsOpen) return ret;

        await _sem.WaitAsync();
        _port.WriteLine(text);
        do
        {
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            if (!_port.IsOpen)
            {
                ret.Add("\r\n***RESTART***\r\n");
                break;
            }
            ret.Add(_port.ReadExisting());
        } while (_port.BytesToRead > 0);
        _sem.Release();
        return ret;
    }
}