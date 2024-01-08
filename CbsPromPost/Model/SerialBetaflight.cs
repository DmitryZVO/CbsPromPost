using Microsoft.Extensions.Logging;
using System.IO.Ports;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace CbsPromPost.Model;

public class SerialBetaflight
{
    public const int DfuStartAddress = 0x08000000;
    public const int DfuFlashBlocks = 512;
    public const int DfuFlashSize = DfuBlockSize * DfuFlashBlocks;
    private const int DfuBlockSize = 0x800; //2048
    private string _serial;
    private readonly ILogger<SerialBetaflight> _logger;
    private bool _alive;
    private readonly SemaphoreSlim _sem;
    private SerialPort _port = new();
    private int _progress = 0; // 0..100;

    public Action<int> OnProgressChange = delegate { };

    public bool IsAlive() => _alive;

    private bool _aliveDfu;
    public bool IsAliveDfu() => _aliveDfu;
    private UsbDevice? _usbDfu;

    public SerialBetaflight(ILogger<SerialBetaflight> logger)
    {
        _sem = new SemaphoreSlim(1);
        _logger = logger;
        _serial = string.Empty;
    }

    public async Task<List<string>> DfuExit()
    {
        var ret = new List<string>();
        await DfuSetAddressPointer(DfuStartAddress);
        for (var i = 0; i < 5; i++)
        {
            await DfuClearStatus();
            await DfuWrite(Array.Empty<byte>());
            await DfuGetStatus();
            await Task.Delay(200);
            if (!_aliveDfu) break;
        }
        ret.Add("\r\n***EXIT DFU MODE & RESTART***\r\n");
        return ret;
    }

    public async Task<int> DfuWaitState(DfuState state, long timeoutMs)
    {
        var now = DateTime.Now;
        DfuStatus currState;
        do
        {
            await DfuClearStatus();
            currState = await DfuGetStatus();
            if ((DateTime.Now - now).TotalMilliseconds > timeoutMs) return -1;
        } while (currState.BState != state);

        return 0;
    }

    public async Task<byte[]> DfuRawBinReadAll()
    {
        using var ret = new MemoryStream();
        const int blocks = DfuFlashSize / DfuBlockSize;

        if (await DfuWaitState(DfuState.DfuIdle, 3000)<0) return Array.Empty<byte>();
        if (await DfuSetAddressPointer(DfuStartAddress) < 0) return Array.Empty<byte>();
        if ((await DfuGetStatus()).BState == DfuState.DfuError) return Array.Empty<byte>();

        _progress = 0;
        OnProgressChange.Invoke(_progress);

        for (var page = 0; page < blocks; page++)
        {
            if ((await DfuGetStatus()).BState != DfuState.DfuIdle)
            {
                if (await DfuClearStatus() < 0) return ret.ToArray();
                if (await DfuWaitState(DfuState.DfuIdle, 3000) < 0) return ret.ToArray();
            }
            var block = await DfuReadPage(page + 2);
            await DfuGetStatus();
            if (block.Length <= 0) return Array.Empty<byte>();
            ret.Write(block);

            _progress = (int)(page / (double)blocks * 100d);
            OnProgressChange.Invoke(_progress);
        }
        _progress = 0;
        OnProgressChange.Invoke(_progress);

        return ret.ToArray();
    }

    private async Task<byte[]> DfuReadPage(int page)
    {
        if (!_aliveDfu) return Array.Empty<byte>();
        if (_usbDfu == null) return Array.Empty<byte>();

        var block = new byte[DfuBlockSize];

        var packet = new UsbSetupPacket
        {
            RequestType = 0x21 | 0x80, // IN
            Request = 0x02, // DFU_CLRSTATUS
            Value = (short)page,
            Index = 0,
        };

        try
        {
            await _sem.WaitAsync();
            _usbDfu.ControlTransfer(ref packet, block, block.Length, out var lenRead);
            _sem.Release();
            if (lenRead<=0) return Array.Empty<byte>();
        }
        catch
        {
            //
        }
        return block;
    }

    public async Task<int> DfuRawBinWrite(byte[] hex, int timeotMs)
    {
        if (!_aliveDfu) return -1;
        if (_usbDfu == null) return -1;

        _progress = 0;
        OnProgressChange(_progress);

        if (await DfuWaitState(DfuState.DfuIdle, 3000) < 0) return -2;
        if (await DfuMassErase(30000) < 0) return -2;

        _progress += 25;
        OnProgressChange(_progress);

        if (await DfuWaitState(DfuState.DfuIdle, 3000) < 0) return -2;

        _progress += 50;
        OnProgressChange(_progress);

        var start = DateTime.Now;
        var blockNumber = 0;
        var seek = 0; // Смещение от начала реальной прошивки в RAW HEX файле, т.к. реальная запись только со 2 блока
        do
        {
            var blockData = Enumerable.Repeat((byte)0xFF, DfuBlockSize).ToArray();
            Array.Copy(hex, seek, blockData, 0, Math.Min(DfuBlockSize, hex.Length - seek));
            if (await DfuWrite(blockData, blockNumber + 2) <= 0) return -1;
            if ((await DfuGetStatus()).BState is not DfuState.DfuDownloadBusy) return -1;
            if (await DfuWaitState(DfuState.DfuIdle, 3000) < 0) return -2;
            if ((DateTime.Now - start).TotalMilliseconds > timeotMs) return -1;

            _progress += 5;
            OnProgressChange(_progress);

            blockNumber++;
            seek += DfuBlockSize;
        } while (seek <= hex.Length);

        _progress = 0;
        OnProgressChange(_progress);
        return 0;
    }

    public async Task<int> DfuMassErase(int timeotMs)
    {
        if (!_aliveDfu) return -1;
        if (_usbDfu == null) return -1;

        _progress = 0;
        OnProgressChange(_progress);

        if (await DfuWaitState(DfuState.DfuIdle, 3000) < 0) return -2;

        _progress += 25;
        OnProgressChange(_progress);

        var check = await DfuCheckProtected();
        if (check < 0) return -1;
        if (await DfuCheckProtected() == 1)
        {
            if (await DfuRemoveReadProtection() < 0) return -1;
            return -3;
        }

        _progress += 50;
        OnProgressChange(_progress);

        if (await DfuMassEraseCommand() < 0) return -1;
        await DfuGetStatus(); // initiate erase command, returns 'download busy' even if invalid address or ROP
        DfuStatus waitState;
        var start = DateTime.Now;
        do
        {
            await Task.Delay(300);
            await DfuClearStatus();
            waitState = await DfuGetStatus();
            if ((DateTime.Now - start).TotalMilliseconds > timeotMs) return -1;

            _progress += 5;
            OnProgressChange(_progress);

        } while (waitState.BState != DfuState.DfuIdle);

        _progress = 0;

        OnProgressChange(_progress);
        return 0;
    }

    private async Task<int> DfuRemoveReadProtection()
    {
        if (await DfuUnProtectCommand() < 0) return -1;
        if ((await DfuGetStatus()).BState != DfuState.DfuDownloadBusy) return -1;
        return 0;
    }

    private async Task<int> DfuMassEraseCommand()
    {
        return await DfuWrite(new byte[] { 0x41 });
    }

    private async Task<int> DfuUnProtectCommand()
    {
        return await DfuWrite(new byte[] {0x92});
    }

    private async Task<int> DfuCheckProtected() // -1=ERROR, 0=ОК, 1=Protected
    {

        if (await DfuWaitState(DfuState.DfuIdle, 3000) < 0) return -1;

        if (await DfuSetAddressPointer(DfuStartAddress) < 0) return -1;

        await DfuGetStatus(); // to execute
        var state = await DfuGetStatus();   // to verify

        var ret = 0;
        if (state.BState == DfuState.DfuError) ret = 1;
        if (await DfuWaitState(DfuState.DfuIdle, 3000) < 0) return -1;

        return ret;
    }

    private async Task<int> DfuSetAddressPointer(long address) //throws Exception //was int
    {
        var buffer = new byte[5];
        buffer[0] = 0x21;
        buffer[1] = (byte)(address & 0xFF);
        buffer[2] = (byte)((address >> 8) & 0xFF);
        buffer[3] = (byte)((address >> 16) & 0xFF);
        buffer[4] = (byte)((address >> 24) & 0xFF);
        return await DfuWrite(buffer);
    }

    private async Task<int> DfuClearStatus()
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
            await _sem.WaitAsync();
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

    private async Task<DfuStatus> DfuGetStatus() //throws Exception
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
            await _sem.WaitAsync();
            _usbDfu.ControlTransfer(ref packet, buffer, buffer.Length, out var lenWrite);
            _sem.Release();

            if (lenWrite <= 0) return ret;

            ret.BStatus = buffer[0]; // state during request
            ret.BState = (DfuState)buffer[4]; // state after request
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

    private async Task<int> DfuWrite(IReadOnlyCollection<byte> data) //throws Exception
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
            await _sem.WaitAsync();
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

    private async Task<int> DfuWrite(IReadOnlyCollection<byte> data, int block) //throws Exception
    {
        if (!_aliveDfu) return -1;
        if (_usbDfu == null) return -1;

        var packet = new UsbSetupPacket()
        {
            RequestType = 0x21, // '2' => Class request ; '1' => to interface
            Request = 0x01, // DFU_DNLOAD
            Value = (short)block,
            Index = 0
        };

        try
        {
            await _sem.WaitAsync();
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
                var state = await DfuGetStatus();
                if (state.BwPollTimeout < 0) break;
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
    public async Task<List<string>> CliWrite(string text, int waitMs=200)
    {
        var ret = new List<string>();
        if (!_port.IsOpen) return ret;

        _progress = 0;
        OnProgressChange.Invoke(_progress);

        await _sem.WaitAsync();
        _port.WriteLine(text);
        do
        {
            ret.Add(_port.ReadExisting());
            await Task.Delay(TimeSpan.FromMilliseconds(waitMs));
            _progress += 20;
            OnProgressChange.Invoke(_progress);

            if (_port.IsOpen) continue;
            ret.Add("\r\n***RESTART***\r\n");
            break;
        } while (_port is { IsOpen: true, BytesToRead: > 0 });
        _sem.Release();

        _progress = 0;
        OnProgressChange.Invoke(_progress);
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