namespace CbsPromPost.Model;

public partial class SerialBetaflight
{
    public float Roll { get; set; }
    public float Pitch { get; set; }
    public float Yaw { get; set; }
    public int Motor1 { get; set; }
    public int Motor2 { get; set; }
    public int Motor3 { get; set; }
    public int Motor4 { get; set; }
    public float BatteryV { get; set; }
    public float Amperage { get; set; }

    private bool _portPause;

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
}