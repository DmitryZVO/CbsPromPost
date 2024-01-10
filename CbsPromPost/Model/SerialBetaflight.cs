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
    private bool _portPause = false;

    public float Roll { get; set; }
    public float Pitch { get; set; }
    public float Yaw { get; set; }
    public int Motor1 { get; set; }
    public int Motor2 { get; set; }
    public int Motor3 { get; set; }
    public int Motor4 { get; set; }
    public float BatteryV { get; set; }
    public float Amperage { get; set; }

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
            await Task.Delay(1);
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

        var ret0 = await DfuWaitState(DfuState.DfuIdle, 3000);
        if (ret0 < 0) return -2;
        var erase = await DfuMassErase(60000);
        if (erase < 0) return -2; // КОСТЫЛЬ

        _progress += 25;
        OnProgressChange(_progress);

        var wait = await DfuWaitState(DfuState.DfuIdle, 3000);
        if (wait < 0) return -2;

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

        var wait0 = await DfuWaitState(DfuState.DfuIdle, 3000);
        if (wait0 < 0) return -2;
        _progress += 25;
        OnProgressChange(_progress);

        var check = await DfuCheckProtected();
        if (check < 0) return -1;
        var check2 = await DfuCheckProtected();
        if (check2 == 1)
        {
            var check3 = await DfuRemoveReadProtection();
            if (check3 < 0) return -1;
            return -3;
        }
        _progress += 50;
        OnProgressChange(_progress);

        var erase = await DfuMassEraseCommand();
        if (erase < 0) return -1;
        var time = await DfuGetStatus(); // initiate erase command, returns 'download busy' even if invalid address or ROP
        if (time.BwPollTimeout>0) await Task.Delay(TimeSpan.FromMilliseconds(time.BwPollTimeout));
        
        var startFlash = DateTime.Now;
        while (true)
        {
            var answ = await DfuClearStatus();
            if (answ < 0) return -3;
            var waitState = await DfuGetStatus();
            if (waitState.BState == DfuState.DfuIdle) break;
            if ((DateTime.Now - startFlash).TotalMilliseconds > timeotMs) return -1;

            _progress += 5;
            OnProgressChange(_progress);
        }
        
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
        return await DfuWrite(new byte[] { 0x92 });
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

        try
        {
            var packet = new UsbSetupPacket
            {
                RequestType = 0x21, // '2' => Class request ; '1' => to interface
                Request = 0x01, // DFU_DNLOAD
                Value = 0,
                Index = 0
            };

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

        try
        {
            var packet = new UsbSetupPacket
            {
                RequestType = 0x21, // '2' => Class request ; '1' => to interface
                Request = 0x01, // DFU_DNLOAD
                Value = (short)block,
                Index = 0
            };

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

    public async Task StartUsbAsync(int vid, int pid, CancellationToken cancellationToken = default)
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
        _sem.Dispose();
    }

    public async Task MspUpdateAttitude(int msWait)
    {
        if (!_port.IsOpen) return;
        if (_portPause) return;
        const ushort lenV2 = 0;
        const ushort commandV2 = 108;
        var data = new byte[9 + lenV2];
        data[0] = 36;//'$' = 36;
        data[1] = 88;//'M' = 77;
        data[2] = 60;//'<' = 60; // '>' = 62 // '|' = 33
        data[3] = 0; // Всегда 0
        Array.Copy(BitConverter.GetBytes(commandV2), 0, data, 4, 2);
        Array.Copy(BitConverter.GetBytes(lenV2), 0, data, 6, 2);
        data[8] = MspGetCrcV2(data); // CRC v2

        _port.ReadExisting();
        _port.Write(data, 0, data.Length);

        await Task.Delay(TimeSpan.FromMilliseconds(msWait));
        using var ms = new MemoryStream();
        if (_port.BytesToRead > 0)
        {
            var read = new byte[_port.BytesToRead];
            _port.Read(read, 0, read.Length);
            ms.Write(read);
        }

        var answ = ms.ToArray();
        if (answ.Length < 15) return;
        if (answ[4] != commandV2) return;

        var i = 8;
        var angleX = BitConverter.ToInt16(answ, i); i += 2;
        var angleY = BitConverter.ToInt16(answ, i); i += 2;
        var angleZ = BitConverter.ToInt16(answ, i); i += 2;
        Roll = -angleX / 10f;
        Pitch = angleY / 10f;
        Yaw = angleZ / 1f;
    }

    public async Task MspUpdateImu(int msWait)
    {
        if (!_port.IsOpen) return;
        if (_portPause) return;
        const ushort lenV2 = 0;
        const ushort commandV2 = 102;
        var data = new byte[9 + lenV2];
        data[0] = 36;//'$' = 36;
        data[1] = 88;//'M' = 77;
        data[2] = 60;//'<' = 60; // '>' = 62 // '|' = 33
        data[3] = 0; // Всегда 0
        Array.Copy(BitConverter.GetBytes(commandV2), 0, data, 4, 2);
        Array.Copy(BitConverter.GetBytes(lenV2), 0, data, 6, 2);
        data[8] = MspGetCrcV2(data); // CRC v2

        _port.ReadExisting();
        _port.Write(data, 0, data.Length);
        
        await Task.Delay(TimeSpan.FromMilliseconds(msWait));
        using var ms = new MemoryStream();
        if (_port.BytesToRead > 0)
        {
            var read = new byte[_port.BytesToRead];
            _port.Read(read, 0, read.Length);
            ms.Write(read);
        }

        var answ = ms.ToArray();
        if (answ.Length < 27) return;
        if (answ[4] != commandV2) return;
        var i = 8;
        const float oneVal = 1f / 4096f;
        var accX = BitConverter.ToInt16(answ, i); i += 2;
        var accY = BitConverter.ToInt16(answ, i); i += 2;
        var accZ = BitConverter.ToInt16(answ, i); i += 2;
        var girX = BitConverter.ToInt16(answ, i) * oneVal; i += 2;
        var girY = BitConverter.ToInt16(answ, i) * oneVal; i += 2;
        var girZ = BitConverter.ToInt16(answ, i) * oneVal; i += 2;
    }

    public async Task MspUpdateMotors(int msWait)
    {
        if (!_port.IsOpen) return;
        if (_portPause) return;
        const ushort lenV2 = 0;
        const ushort commandV2 = 104;
        var data = new byte[9 + lenV2];
        data[0] = 36;//'$' = 36;
        data[1] = 88;//'M' = 77;
        data[2] = 60;//'<' = 60; // '>' = 62 // '|' = 33
        data[3] = 0; // Всегда 0
        Array.Copy(BitConverter.GetBytes(commandV2), 0, data, 4, 2);
        Array.Copy(BitConverter.GetBytes(lenV2), 0, data, 6, 2);
        data[8] = MspGetCrcV2(data); // CRC v2

        _port.ReadExisting();
        _port.Write(data, 0, data.Length);

        await Task.Delay(TimeSpan.FromMilliseconds(msWait));
        using var ms = new MemoryStream();
        if (_port.BytesToRead > 0)
        {
            var read = new byte[_port.BytesToRead];
            _port.Read(read, 0, read.Length);
            ms.Write(read);
        }

        var answ = ms.ToArray();
        if (answ.Length < 25) return;
        if (answ[4] != commandV2) return;
        var i = 8;
        Motor1 = BitConverter.ToInt16(answ, i); i += 2;
        Motor2 = BitConverter.ToInt16(answ, i); i += 2;
        Motor3 = BitConverter.ToInt16(answ, i); i += 2;
        Motor4 = BitConverter.ToInt16(answ, i); i += 2;
    }

    public async Task MspUpdateAnalog(int msWait)
    {
        if (!_port.IsOpen) return;
        if (_portPause) return;
        const ushort lenV2 = 0;
        const ushort commandV2 = 110;
        var data = new byte[9 + lenV2];
        data[0] = 36;//'$' = 36;
        data[1] = 88;//'M' = 77;
        data[2] = 60;//'<' = 60; // '>' = 62 // '|' = 33
        data[3] = 0; // Всегда 0
        Array.Copy(BitConverter.GetBytes(commandV2), 0, data, 4, 2);
        Array.Copy(BitConverter.GetBytes(lenV2), 0, data, 6, 2);
        data[8] = MspGetCrcV2(data); // CRC v2

        _port.ReadExisting();
        _port.Write(data, 0, data.Length);

        await Task.Delay(TimeSpan.FromMilliseconds(msWait));
        using var ms = new MemoryStream();
        if (_port.BytesToRead > 0)
        {
            var read = new byte[_port.BytesToRead];
            _port.Read(read, 0, read.Length);
            ms.Write(read);
        }

        var answ = ms.ToArray();
        if (answ.Length < 18) return;
        if (answ[4] != commandV2) return;
        var i = 8;
        BatteryV = answ[i] / 10f; i += 1;
        var pm = BitConverter.ToInt16(answ, i) / 10f; i += 2;
        var rssi = BitConverter.ToInt16(answ, i); i += 2;
        Amperage = BitConverter.ToInt16(answ, i) / 100f;
    }

    public void MspSetMotor(int pwm1, int pwm2, int pwm3, int pwm4)
    {
        if (!_port.IsOpen) return;
        if (_portPause) return;
        const ushort lenV2 = 8 * 2;
        const ushort commandV2 = 214;
        var data = new byte[9 + lenV2];
        data[0] = 36;//'$' = 36;
        data[1] = 88;//'M' = 77;
        data[2] = 60;//'<' = 60; // '>' = 62 // '|' = 33
        data[3] = 0; // Всегда 0
        Array.Copy(BitConverter.GetBytes(commandV2), 0, data, 4, 2);
        Array.Copy(BitConverter.GetBytes(lenV2), 0, data, 6, 2);

        var i = 8;
        Array.Copy(BitConverter.GetBytes((ushort)pwm1), 0, data, i, 2); i += 2;
        Array.Copy(BitConverter.GetBytes((ushort)pwm2), 0, data, i, 2); i += 2;
        Array.Copy(BitConverter.GetBytes((ushort)pwm3), 0, data, i, 2); i += 2;
        Array.Copy(BitConverter.GetBytes((ushort)pwm4), 0, data, i, 2); i += 2;
        Array.Copy(BitConverter.GetBytes((ushort)1000), 0, data, i, 2); i += 2;
        Array.Copy(BitConverter.GetBytes((ushort)1000), 0, data, i, 2); i += 2;
        Array.Copy(BitConverter.GetBytes((ushort)1000), 0, data, i, 2); i += 2;
        Array.Copy(BitConverter.GetBytes((ushort)1000), 0, data, i, 2); i += 2;
        data[i] = MspGetCrcV2(data); // CRC v2

        _port.ReadExisting();
        _port.Write(data, 0, data.Length);
    }

    private static byte MspGetCrcV2(IReadOnlyList<byte> data)
    {
        byte ck2 = 0; // initialise CRC
        for (var i = 3; i < data.Count - 1; i++)
            ck2 = Crc8DvbS2(ck2, data[i]);
        return ck2;
    }

    private static byte Crc8DvbS2(byte crc, byte a)
    {
        crc ^= a;
        for (var i = 0; i < 8; i++)
        {
            var crcShiftAndMask = (byte)((crc << 1) & 0xFF);
            crc = (0 != (crc & 0x80)) ? (byte)(crcShiftAndMask ^ 0xD5) : crcShiftAndMask;
        }
        return crc;
    }

    public async Task MspSetReverse(int motor, bool reverse)
    {
        if (!_port.IsOpen) return;
        if (_portPause) return;

        var p1 = Motor1;
        var p2 = Motor2;
        var p3 = Motor3;
        var p4 = Motor4;
        MspSetMotor(motor == 0 ? 1000 : p1, motor == 1 ? 1000 : p2, motor == 2 ? 1000 : p3, motor == 3 ? 1000 : p4);

        _portPause = true;
        await Task.Delay(400);

        const ushort lenV2 = 5;
        const ushort commandV2 = 0x3003; // CMD_DSHOT_SEND
        var data = new byte[9 + lenV2];
        data[0] = 36;//'$' = 36;
        data[1] = 88;//'M' = 77;
        data[2] = 60;//'<' = 60; // '>' = 62 // '|' = 33
        data[3] = 0; // Всегда 0
        Array.Copy(BitConverter.GetBytes(commandV2), 0, data, 4, 2);
        Array.Copy(BitConverter.GetBytes(lenV2), 0, data, 6, 2);
        var i = 8;
        data[i] = 1; i++; // DSHOT_CMD_TYPE_BLOCKING
        data[i] = (byte)motor; i++; // MOTOR_INDEX
        data[i] = 2; i++; // NUM_COMMANDS
        data[i] = (byte)(reverse ? 7 : 8); i++; // SPIN_DIRECTION
        data[i] = 12; i++; // SAVE_ESC_SETTING
        data[i] = MspGetCrcV2(data); // CRC v2

        _port.ReadExisting();
        _port.Write(data, 0, data.Length);
        _portPause = false;

        MspSetMotor(p1, p2, p3, p4);
    }

    public void MspCalibrateAcel()
    {
        if (!_port.IsOpen) return;
        if (_portPause) return;
        const ushort lenV2 = 0;
        const ushort commandV2 = 205;
        var data = new byte[9 + lenV2];
        data[0] = 36;//'$' = 36;
        data[1] = 88;//'M' = 77;
        data[2] = 60;//'<' = 60; // '>' = 62 // '|' = 33
        data[3] = 0; // Всегда 0
        Array.Copy(BitConverter.GetBytes(commandV2), 0, data, 4, 2);
        Array.Copy(BitConverter.GetBytes(lenV2), 0, data, 6, 2);
        data[8] = MspGetCrcV2(data); // CRC v2

        _port.ReadExisting();
        _port.Write(data, 0, data.Length);
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

    /*
DshotCommand.dshotCommands_e = {
    DSHOT_CMD_MOTOR_STOP: 0,
    DSHOT_CMD_BEACON1: 1,
    DSHOT_CMD_BEACON2: 2,
    DSHOT_CMD_BEACON3: 3,
    DSHOT_CMD_BEACON4: 4,
    DSHOT_CMD_BEACON5: 5,
    DSHOT_CMD_ESC_INFO: 6, // V2 includes settings
    DSHOT_CMD_SPIN_DIRECTION_1: 7,
    DSHOT_CMD_SPIN_DIRECTION_2: 8,
    DSHOT_CMD_3D_MODE_OFF: 9,
    DSHOT_CMD_3D_MODE_ON: 10,
    DSHOT_CMD_SETTINGS_REQUEST: 11, // Currently not implemented
    DSHOT_CMD_SAVE_SETTINGS: 12,
    DSHOT_CMD_SPIN_DIRECTION_NORMAL: 20,
    DSHOT_CMD_SPIN_DIRECTION_REVERSED: 21,
    DSHOT_CMD_LED0_ON: 22, // BLHeli32 only
    DSHOT_CMD_LED1_ON: 23, // BLHeli32 only
    DSHOT_CMD_LED2_ON: 24, // BLHeli32 only
    DSHOT_CMD_LED3_ON: 25, // BLHeli32 only
    DSHOT_CMD_LED0_OFF: 26, // BLHeli32 only
    DSHOT_CMD_LED1_OFF: 27, // BLHeli32 only
    DSHOT_CMD_LED2_OFF: 28, // BLHeli32 only
    DSHOT_CMD_LED3_OFF: 29, // BLHeli32 only
    DSHOT_CMD_AUDIO_STREAM_MODE_ON_OFF: 30, // KISS audio Stream mode on/Off
    DSHOT_CMD_SILENT_MODE_ON_OFF: 31, // KISS silent Mode on/Off
    DSHOT_CMD_MAX: 47,
};     */
    /*
const MSPCodes = {
    MSP_API_VERSION:                1,
    MSP_FC_VARIANT:                 2,
    MSP_FC_VERSION:                 3,
    MSP_BOARD_INFO:                 4,
    MSP_BUILD_INFO:                 5,

    MSP_BATTERY_CONFIG:             32,
    MSP_SET_BATTERY_CONFIG:         33,
    MSP_MODE_RANGES:                34,
    MSP_SET_MODE_RANGE:             35,
    MSP_FEATURE_CONFIG:             36,
    MSP_SET_FEATURE_CONFIG:         37,
    MSP_BOARD_ALIGNMENT_CONFIG:     38,
    MSP_SET_BOARD_ALIGNMENT_CONFIG: 39,
    MSP_CURRENT_METER_CONFIG:       40,
    MSP_SET_CURRENT_METER_CONFIG:   41,
    MSP_MIXER_CONFIG:               42,
    MSP_SET_MIXER_CONFIG:           43,
    MSP_RX_CONFIG:                  44,
    MSP_SET_RX_CONFIG:              45,
    MSP_LED_COLORS:                 46,
    MSP_SET_LED_COLORS:             47,
    MSP_LED_STRIP_CONFIG:           48,
    MSP_SET_LED_STRIP_CONFIG:       49,
    MSP_RSSI_CONFIG:                50,
    MSP_SET_RSSI_CONFIG:            51,
    MSP_ADJUSTMENT_RANGES:          52,
    MSP_SET_ADJUSTMENT_RANGE:       53,
    MSP_CF_SERIAL_CONFIG:           54,
    MSP_SET_CF_SERIAL_CONFIG:       55,
    MSP_VOLTAGE_METER_CONFIG:       56,
    MSP_SET_VOLTAGE_METER_CONFIG:   57,
    MSP_SONAR:                      58,
    MSP_PID_CONTROLLER:             59,
    MSP_SET_PID_CONTROLLER:         60,
    MSP_ARMING_CONFIG:              61,
    MSP_SET_ARMING_CONFIG:          62,
    MSP_RX_MAP:                     64,
    MSP_SET_RX_MAP:                 65,
    MSP_SET_REBOOT:                 68,
    MSP_DATAFLASH_SUMMARY:          70,
    MSP_DATAFLASH_READ:             71,
    MSP_DATAFLASH_ERASE:            72,
    MSP_LOOP_TIME:                  73,
    MSP_SET_LOOP_TIME:              74,
    MSP_FAILSAFE_CONFIG:            75,
    MSP_SET_FAILSAFE_CONFIG:        76,
    MSP_RXFAIL_CONFIG:              77,
    MSP_SET_RXFAIL_CONFIG:          78,
    MSP_SDCARD_SUMMARY:             79,
    MSP_BLACKBOX_CONFIG:            80,
    MSP_SET_BLACKBOX_CONFIG:        81,
    MSP_TRANSPONDER_CONFIG:         82,
    MSP_SET_TRANSPONDER_CONFIG:     83,
    MSP_OSD_CONFIG:                 84,
    MSP_SET_OSD_CONFIG:             85,
    MSP_OSD_CHAR_READ:              86,
    MSP_OSD_CHAR_WRITE:             87,
    MSP_VTX_CONFIG:                 88,
    MSP_SET_VTX_CONFIG:             89,
    MSP_ADVANCED_CONFIG:            90,
    MSP_SET_ADVANCED_CONFIG:        91,
    MSP_FILTER_CONFIG:              92,
    MSP_SET_FILTER_CONFIG:          93,
    MSP_PID_ADVANCED:               94,
    MSP_SET_PID_ADVANCED:           95,
    MSP_SENSOR_CONFIG:              96,
    MSP_SET_SENSOR_CONFIG:          97,
    MSP_ARMING_DISABLE:             99,
    MSP_STATUS:                     101,
    MSP_RAW_IMU:                    102,
    MSP_SERVO:                      103,
    MSP_MOTOR:                      104,
    MSP_RC:                         105,
    MSP_RAW_GPS:                    106,
    MSP_COMP_GPS:                   107,
    MSP_ATTITUDE:                   108,
    MSP_ALTITUDE:                   109,
    MSP_ANALOG:                     110,
    MSP_RC_TUNING:                  111,
    MSP_PID:                        112,
    MSP_BOXNAMES:                   116,
    MSP_PIDNAMES:                   117,
    MSP_BOXIDS:                     119,
    MSP_SERVO_CONFIGURATIONS:       120,
    MSP_MOTOR_3D_CONFIG:            124,
    MSP_RC_DEADBAND:                125,
    MSP_SENSOR_ALIGNMENT:           126,
    MSP_LED_STRIP_MODECOLOR:        127,

    MSP_VOLTAGE_METERS:             128,
    MSP_CURRENT_METERS:             129,
    MSP_BATTERY_STATE:              130,
    MSP_MOTOR_CONFIG:               131,
    MSP_GPS_CONFIG:                 132,
    MSP_GPS_RESCUE:                 135,

    MSP_VTXTABLE_BAND:              137,
    MSP_VTXTABLE_POWERLEVEL:        138,

    MSP_MOTOR_TELEMETRY:            139,

    MSP_SIMPLIFIED_TUNING:          140,
    MSP_SET_SIMPLIFIED_TUNING:      141,

    MSP_CALCULATE_SIMPLIFIED_GYRO:  143,
    MSP_CALCULATE_SIMPLIFIED_DTERM: 144,

    MSP_STATUS_EX:                  150,

    MSP_UID:                        160,
    MSP_GPS_SV_INFO:                164,

    MSP_DISPLAYPORT:                182,

    MSP_COPY_PROFILE:               183,

    MSP_BEEPER_CONFIG:              184,
    MSP_SET_BEEPER_CONFIG:          185,

    MSP_SET_OSD_CANVAS:             188,
    MSP_OSD_CANVAS:                 189,

    MSP_SET_RAW_RC:                 200,
    MSP_SET_PID:                    202,
    MSP_SET_RC_TUNING:              204,
    MSP_ACC_CALIBRATION:            205,
    MSP_MAG_CALIBRATION:            206,
    MSP_RESET_CONF:                 208,
    MSP_SELECT_SETTING:             210,
    MSP_SET_SERVO_CONFIGURATION:    212,
    MSP_SET_MOTOR:                  214,
    MSP_SET_MOTOR_3D_CONFIG:        217,
    MSP_SET_RC_DEADBAND:            218,
    MSP_SET_RESET_CURR_PID:         219,
    MSP_SET_SENSOR_ALIGNMENT:       220,
    MSP_SET_LED_STRIP_MODECOLOR:    221,
    MSP_SET_MOTOR_CONFIG:           222,
    MSP_SET_GPS_CONFIG:             223,
    MSP_SET_GPS_RESCUE:             225,

    MSP_SET_VTXTABLE_BAND:          227,
    MSP_SET_VTXTABLE_POWERLEVEL:    228,

    MSP_MULTIPLE_MSP:               230,

    MSP_MODE_RANGES_EXTRA:          238,
    MSP_SET_ACC_TRIM:               239,
    MSP_ACC_TRIM:                   240,
    MSP_SERVO_MIX_RULES:            241,
    MSP_SET_RTC:                    246,

    MSP_EEPROM_WRITE:               250,
    MSP_DEBUG:                      254,

    // MSPv2 Common
    MSP2_COMMON_SERIAL_CONFIG:      0x1009,
    MSP2_COMMON_SET_SERIAL_CONFIG:  0x100A,

    // MSPv2 Betaflight specific
    MSP2_BETAFLIGHT_BIND:           0x3000,
    MSP2_MOTOR_OUTPUT_REORDERING:   0x3001,
    MSP2_SET_MOTOR_OUTPUT_REORDERING:    0x3002,
    MSP2_SEND_DSHOT_COMMAND:        0x3003,
    MSP2_GET_VTX_DEVICE_STATUS:     0x3004,
    MSP2_GET_OSD_WARNINGS:          0x3005,
    MSP2_GET_TEXT:                  0x3006,
    MSP2_SET_TEXT:                  0x3007,

    // MSP2_GET_TEXT and MSP2_SET_TEXT variable types
    PILOT_NAME:                     1,
    CRAFT_NAME:                     2,
    PID_PROFILE_NAME:               3,
    RATE_PROFILE_NAME:              4,
};      */
}