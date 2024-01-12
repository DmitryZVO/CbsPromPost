using LibUsbDotNet.Main;
using Microsoft.Extensions.Logging;

namespace CbsPromPost.Model;

public partial class SerialBetaflight
{
    public const int DfuStartAddress = 0x08000000;
    public const int DfuFlashBlocks = 512;
    public const int DfuFlashSize = DfuBlockSize * DfuFlashBlocks;
    private const int DfuBlockSize = 0x800; //2048

    public async Task<List<string>> DfuExit()
    {
        var ret = new List<string>();
        await DfuSetAddressPointer(DfuStartAddress);
        for (var i = 0; i < 10; i++)
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
        var start = DateTime.Now;
        while (true)
        {
            await Task.Delay(1);
            await DfuClearStatus();
            if ((await DfuGetStatus()).BState == state) break;
            if ((DateTime.Now - start).TotalMilliseconds > timeoutMs) return -1;
        }
        return 0;
    }

    public async Task<byte[]> DfuRawBinReadAll()
    {
        using var ret = new MemoryStream();
        const int blocks = DfuFlashSize / DfuBlockSize;

        if (await DfuWaitState(DfuState.DfuIdle, 6000) < 0) return Array.Empty<byte>();
        if (await DfuSetAddressPointer(DfuStartAddress) < 0) return Array.Empty<byte>();
        if ((await DfuGetStatus()).BState == DfuState.DfuError) return Array.Empty<byte>();

        ProgressValue = 0;

        for (var page = 0; page < blocks; page++)
        {
            if ((await DfuGetStatus()).BState != DfuState.DfuIdle)
            {
                if (await DfuClearStatus() < 0) return ret.ToArray();
                if (await DfuWaitState(DfuState.DfuIdle, 6000) < 0) return ret.ToArray();
            }
            var block = DfuReadPage(page + 2);
            await DfuGetStatus();
            if (block.Length <= 0) return Array.Empty<byte>();
            ret.Write(block);

            ProgressValue = (int)(page / (double)blocks * 100d);
        }
        ProgressValue = 0;

        return ret.ToArray();
    }

    private byte[] DfuReadPage(int page)
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

        lock (this)
        {
            //var start = DateTime.Now;
            _usbDfu.ControlTransfer(ref packet, block, block.Length, out var lenRead);
            //_logger.LogWarning($"[DfuReadPage] usb_transfer_time: {(DateTime.Now - start).TotalMilliseconds:0.00}");
            if (lenRead <= 0) return Array.Empty<byte>();
        }

        return block;
    }

    public async Task<int> DfuRawBinWrite(byte[] hex, int timeotMs)
    {
        if (!_aliveDfu)
        {
            _logger.LogError("DfuRawBinWrite error: _notAliveDfu");
            return -1;
        }

        if (_usbDfu == null)
        {
            _logger.LogError("DfuRawBinWrite error: _notAliveUsb");
            return -1;
        }

        ProgressValue = 0;
        
        if (await DfuWaitState(DfuState.DfuIdle, 10000) < 0)
        {
            _logger.LogError("DfuRawBinWrite error: DfuWaitState #1, state=" + (await DfuGetStatus()).BState);
            return -2;
        }

        if (await DfuMassErase(timeotMs) < 0)
        {
            _logger.LogError("DfuRawBinWrite error: DfuMassErase<0");
            return -3;
        }

        ProgressValue += 25;

        if (await DfuWaitState(DfuState.DfuIdle, 10000) < 0)
        {
            _logger.LogError("DfuRawBinWrite error: DfuWaitState #2, state=" + (await DfuGetStatus()).BState);
            return -2;
        }

        ProgressValue += 50;

        var start = DateTime.Now;
        var blockNumber = 0;
        var seek = 0; // Смещение от начала реальной прошивки в RAW HEX файле, т.к. реальная запись только со 2 блока
        do
        {
            var blockData = Enumerable.Repeat((byte)0xFF, DfuBlockSize).ToArray();
            Array.Copy(hex, seek, blockData, 0, Math.Min(DfuBlockSize, hex.Length - seek));
            if (await DfuWrite(blockData, blockNumber + 2) <= 0)
                if (await DfuWrite(blockData, blockNumber + 2) <= 0)
                    if (await DfuWrite(blockData, blockNumber + 2) <= 0)
                    {
                        _logger.LogError("DfuRawBinWrite error: DfuWrite<0");
                        return -1;
                    }
            if ((await DfuGetStatus()).BState is not DfuState.DfuDownloadBusy)
                if ((await DfuGetStatus()).BState is not DfuState.DfuDownloadBusy)
                    if ((await DfuGetStatus()).BState is not DfuState.DfuDownloadBusy)
                    {
                        _logger.LogError("DfuRawBinWrite error: not DfuDownloadBusy");
                        return -5;
                    }

            if (await DfuWaitState(DfuState.DfuIdle, 10000) < 0)
            {
                _logger.LogError("DfuRawBinWrite error: DfuWaitState #3, state="+(await DfuGetStatus()).BState);
                return -2;
            }
            if ((DateTime.Now - start).TotalMilliseconds > timeotMs)
            {
                _logger.LogError("DfuRawBinWrite error: Timeout!");
                return -1;
            }

            ProgressValue += 5;

            blockNumber++;
            seek += DfuBlockSize;
        } while (seek <= hex.Length);

        ProgressValue = 0;
        return 0;
    }

    public async Task<int> DfuMassErase(int timeotMs)
    {
        if (!_aliveDfu)
        {
            _logger.LogError("DfuMassErase error: _notAliveDfu");
            return -1;
        }

        if (_usbDfu == null)
        {
            _logger.LogError("DfuMassErase error: _notAliveUsb");
            return -1;
        }

        ProgressValue = 0;

        if (await DfuWaitState(DfuState.DfuIdle, 10000) < 0)
        {
            _logger.LogError("DfuMassErase error: DfuWaitState #1, state=" + (await DfuGetStatus()).BState);
            return -2;
        }

        ProgressValue += 10;

        var ret = await DfuCheckProtected();
        if (ret < 0) ret = await DfuCheckProtected();
        if (ret < 0) ret = await DfuCheckProtected();

        switch (ret)
        {
            case < 0:
                _logger.LogError("DfuMassErase error: DfuCheckProtectes<0");
                return -1;
            case 1:
            {
                if (await DfuRemoveReadProtection() < 0)
                    if (await DfuRemoveReadProtection() < 0)
                        if (await DfuRemoveReadProtection() < 0)
                        {
                            _logger.LogError("DfuMassErase error: DfuRemoveReadProtection<0");
                            return -1;
                        }
                break;
            }
        }

        if (await DfuMassEraseCommand() < 0)
            if (await DfuMassEraseCommand() < 0)
                if (await DfuMassEraseCommand() < 0)
                {
                    _logger.LogError("DfuMassErase error: DfuMassEraseCommand<0");
                    return -1;
                }
        var time = (await DfuGetStatus()).BwPollTimeout;
        if (time < 0) time = (await DfuGetStatus()).BwPollTimeout;
        if (time < 0) time = (await DfuGetStatus()).BwPollTimeout;

        if (time > 0) // initiate erase command, returns 'download busy' even if invalid address or ROP
        {
            await Task.Delay(time);
        }
        else
        {
            _logger.LogError("DfuMassErase error: BwPollTimeout<0");
            return -2;
        }

        var startFlash = DateTime.Now;
        while (true)
        {
            if (await DfuClearStatus() < 0)
                if (await DfuClearStatus() < 0)
                    if (await DfuClearStatus() < 0)
                    {
                        _logger.LogError("DfuMassErase error: DfuClearStatus<0");
                        return -3;
                    }
            if ((await DfuGetStatus()).BState == DfuState.DfuIdle) break;
            if ((DateTime.Now - startFlash).TotalMilliseconds > timeotMs)
            {
                _logger.LogError("DfuMassErase error: Timeout!");
                return -1;
            }
            ProgressValue += 5;
        }

        ProgressValue = 0;
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

        var ret = 0;
        if ((await DfuGetStatus()).BState == DfuState.DfuError) ret = 1;
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

        return await Task.Run(() =>
        {
            var packet = new UsbSetupPacket
            {
                RequestType = 0x21,
                Request = 0x04, // DFU_CLRSTATUS
                Value = 0,
                Index = 0
            };

            try
            {
                lock (this)
                {
                    //var start = DateTime.Now;
                    _usbDfu.ControlTransfer(ref packet, null, 0, out var lenWrite);
                    //_logger.LogWarning($"[DfuClearStatus] async_usb_transfer_time: {(DateTime.Now - start).TotalMilliseconds:0.00}");
                    return lenWrite;
                }
            }
            catch
            {
                //
            }
            return -1;
        });
    }

    private async Task<DfuStatus> DfuGetStatus() //throws Exception
    {
        if (!_aliveDfu) return new DfuStatus();
        if (_usbDfu == null) return new DfuStatus();

        return await Task.Run(() =>
        {
            var ret = new DfuStatus();
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
                lock (this)
                {
                    //var start = DateTime.Now;
                    _usbDfu.ControlTransfer(ref packet, buffer, buffer.Length, out var lenWrite);
                    //_logger.LogWarning($"[DfuGetStatus] async_usb_transfer_time: {(DateTime.Now - start).TotalMilliseconds:0.00}");
                    if (lenWrite <= 0) return ret;

                    ret.BStatus = buffer[0]; // state during request
                    ret.BState = (DfuState)buffer[4]; // state after request
                    ret.BwPollTimeout = (buffer[3] & 0xFF) << 16;
                    ret.BwPollTimeout |= (buffer[2] & 0xFF) << 8;
                    ret.BwPollTimeout |= (buffer[1] & 0xFF);
                }
            }
            catch
            {
                //
            }
            return ret;
        });
    }

    private async Task<int> DfuWrite(IReadOnlyCollection<byte> data) //throws Exception
    {
        if (!_aliveDfu) return -1;
        if (_usbDfu == null) return -1;

        return await Task.Run(() =>
        {
            var packet = new UsbSetupPacket
            {
                RequestType = 0x21, // '2' => Class request ; '1' => to interface
                Request = 0x01, // DFU_DNLOAD
                Value = 0,
                Index = 0
            };

            try
            {
                lock (this)
                {
                    //var start = DateTime.Now;
                    _usbDfu.ControlTransfer(ref packet, data, data.Count, out var lenWrite);
                    //_logger.LogWarning($"[DfuWrite] async_usb_transfer_time: {(DateTime.Now - start).TotalMilliseconds:0.00}");
                    return lenWrite;
                }
            }
            catch
            {
                //
            }
            return -1;
        });
    }

    private async Task<int> DfuWrite(IReadOnlyCollection<byte> data, int block) //throws Exception
    {
        if (!_aliveDfu)
        {
            _logger.LogError("DfuWrite error: _notAliveDfu block=" + block.ToString("0"));
            return -1;
        }

        if (_usbDfu == null)
        {
            _logger.LogError("DfuWrite error: _notAliveUsb block="+block.ToString("0"));
            return -1;
        }

        return await Task.Run(() =>
        {
            var packet = new UsbSetupPacket
            {
                RequestType = 0x21, // '2' => Class request ; '1' => to interface
                Request = 0x01, // DFU_DNLOAD
                Value = (short)block,
                Index = 0
            };

            try
            {
                lock (this)
                {
                    //var start = DateTime.Now;
                    _usbDfu.ControlTransfer(ref packet, data, data.Count, out var lenWrite);
                    //_logger.LogWarning($"[DfuWrite] async_usb_transfer_time: {(DateTime.Now - start).TotalMilliseconds:0.00}");
                    return lenWrite;
                }
            }
            catch
            {
                //
            }
            return -1;
        });
    }
}