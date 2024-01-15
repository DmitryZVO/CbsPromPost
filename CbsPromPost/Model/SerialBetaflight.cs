using Microsoft.Extensions.Logging;
using System.IO.Ports;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace CbsPromPost.Model;

public partial class SerialBetaflight
{
    public bool IsAliveCom() => _aliveCom;
    public bool IsAliveDfu() => _aliveDfu;
    public bool FinishWork { get; set; }
    public int ProgressValue { get; private set; }
    public Action<string> OnNewCliMessage = delegate { };
    public bool PowerEnabled { get; set; } 

    private readonly ILogger<SerialBetaflight> _logger;
    private SerialPort _port;
    private string _serialString;
    private bool _aliveCom;
    private SpinWait _spinWait;
    private bool _aliveDfu;
    private readonly object _lockDfu = new();
    private readonly object _lockCom = new();
    private UsbDevice? _usbDfu;
    private StringBuilder _cliStr;

    public SerialBetaflight(ILogger<SerialBetaflight> logger)
    {
        _cliStr = new StringBuilder();
        _logger = logger;
        _serialString = string.Empty;
        _port = new SerialPort();
    }


    public async Task StartUsbAsync(int vid, int pid, CancellationToken cancellationToken = default)
    {
        await Task.Run(async () =>
        {
            var usbFinder = new UsbDeviceFinder(vid, pid);
            while (!FinishWork)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);

                lock (_lockDfu)
                {
                    _usbDfu = UsbDevice.OpenUsbDevice(usbFinder);
                    if (_usbDfu == null)
                    {
                        _aliveDfu = false;
                        continue;
                    }
                }

                _aliveDfu = true;

                var countFalse = 0;
                while (!FinishWork)
                {
                    byte cfg;
                    await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);

                    lock (_lockDfu)
                    {
                        _usbDfu.GetConfiguration(out cfg);
                    }

                    if (cfg == 0)
                    {
                        countFalse++;
                        if (countFalse >= 10) break;
                    }
                    else
                    {
                        countFalse = 0;
                    }
                }

                lock (_lockDfu)
                {
                    _usbDfu.Close();
                }

                _aliveDfu = false;
            }
        }, cancellationToken);
    }

    public async Task StartAsync(string com, CancellationToken cancellationToken = default)
    {
        await Task.Run(async () =>
        {
            _serialString = com;
            lock (_lockCom)
            {
                _port = new SerialPort(_serialString, 115200, Parity.None, 8, StopBits.One);
                _port.ReadBufferSize = 655350;
                _port.WriteBufferSize = 655350;
                _port.WriteTimeout = 1000;
                _port.ReadTimeout = 1000;
                _port.DataReceived += ComDataReceive;
            }

            while (!FinishWork)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);

                try
                {
                    lock (_lockCom)
                    {
                        _port.Open();
                    }

                    _aliveCom = true;
                    bool open;
                    do
                    {
                        lock (_lockCom)
                        {
                            open = _port.IsOpen;
                        }

                        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                    } while (open && !FinishWork);
                }
                catch
                {
                    //_logger.LogInformation("SERIAL_EXEPTION_{Serial}: {Ex}", _serialString, ex.Message);
                }

                lock (_lockCom)
                {
                    _port.Close();
                }

                _aliveCom = false;
            }

            lock (_lockCom)
            {
                _port.DataReceived -= ComDataReceive;
            }
        }, cancellationToken);
    }

    private void ComDataReceive(object sender, SerialDataReceivedEventArgs e)
    {
        byte[] buffer;
        lock (_lockCom)
        {
            buffer = new byte[_port.BytesToRead];
            if (_port.BaseStream.Read(buffer, 0, buffer.Length) <= 0) return;
        }

        var data = Encoding.ASCII.GetString(buffer);
        if (data.Equals(string.Empty)) return;
        if (data.Length > 3 && data[..3].Equals("$X>")) // это пакет Msp
        {
            UpdateValuesFromMsp(buffer);
            return;
        }
        if (_cliStr.Length == 0 && !data.Contains('#')) return; // Это остаток от пакета Msp

        _cliStr.Append(data);
        var str = _cliStr.ToString();
        if (str.Length < 6) return;
        if (!str[^6..].Equals("\r\n\r\n# ")) return; // Конец сообщения CLI
        OnNewCliMessage(str);
        _cliStr = new StringBuilder();
    }

    public void CliWrite(string text)
    {
        var buffer = Encoding.ASCII.GetBytes($"{text}\r\n");
        CliWrite(buffer);
        _spinWait.SpinOnce();
    }

    public void CliWrite(ReadOnlySpan<byte> buffer)
    {
        lock (_lockCom)
        {
            if (!_port.IsOpen) return;
            try
            {
                _port.BaseStream.Write(buffer);
                _spinWait.SpinOnce();
            }
            catch
            {
                //
            }
        }
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