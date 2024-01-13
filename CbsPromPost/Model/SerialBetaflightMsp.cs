using Microsoft.Extensions.Logging;

namespace CbsPromPost.Model;

public partial class SerialBetaflight
{
    public float Roll { get; set; }
    public float Pitch { get; set; }
    public float Yaw { get; set; }
    public int[] MotorsPwm { get; set; } = { 1000, 1000, 1000, 1000 };
    public int[] RcPwm { get; set; } = { 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500 };
    public float BatteryV { get; set; }
    public float Amperage { get; set; }
    public float GyroX { get; set; }
    public float GyroY { get; set; }
    public float GyroZ { get; set; }
    public float AccX { get; set; }
    public float AccY { get; set; }
    public float AccZ { get; set; }
    public float MagX { get; set; }
    public float MagY { get; set; }
    public float MagZ { get; set; }

    private bool _portPause;

    public void UpdateValuesFromMsp(byte[] packet)
    {
        if (packet.Length < 8) return; // Пакет слишком мал
        var i = 8;
        switch (packet[4])
        {
            case 108: // Attitude
            {
                if (packet.Length < 15) break;
                Roll = -BitConverter.ToInt16(packet, i) / 10f; i += 2;
                Pitch = BitConverter.ToInt16(packet, i) / 10f; i += 2;
                Yaw = BitConverter.ToInt16(packet, i) / 1f;
                break;
            }
            case 102: // Imu
            {
                if (packet.Length < 27) break;
                AccX = BitConverter.ToInt16(packet, i); i += 2;
                AccY = BitConverter.ToInt16(packet, i); i += 2;
                AccZ = BitConverter.ToInt16(packet, i); i += 2;
                GyroX = BitConverter.ToInt16(packet, i); i += 2;
                GyroY = BitConverter.ToInt16(packet, i); i += 2;
                GyroZ = BitConverter.ToInt16(packet, i); i += 2;
                MagX = BitConverter.ToInt16(packet, i); i += 2;
                MagY = BitConverter.ToInt16(packet, i); i += 2;
                MagZ = BitConverter.ToInt16(packet, i);
                break;
            }
            case 104: // Motors
            {
                if (packet.Length < 25) break;
                MotorsPwm[0] = BitConverter.ToInt16(packet, i); i += 2;
                MotorsPwm[1] = BitConverter.ToInt16(packet, i); i += 2;
                MotorsPwm[2] = BitConverter.ToInt16(packet, i); i += 2;
                MotorsPwm[3] = BitConverter.ToInt16(packet, i);
                break;
            }
            case 105:
            {
                if (packet.Length < 41) break;
                RcPwm[0] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[1] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[2] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[3] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[4] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[5] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[6] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[7] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[8] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[9] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[10] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[11] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[12] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[13] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[14] = BitConverter.ToUInt16(packet, i); i += 2;
                RcPwm[15] = BitConverter.ToUInt16(packet, i);
                break;
            }
            case 110: // Analog
            {
                if (packet.Length < 18) break;
                BatteryV = packet[i] / 10f; i += 1;
                var pm = BitConverter.ToInt16(packet, i) / 10f; i += 2;
                var rssi = BitConverter.ToInt16(packet, i); i += 2;
                Amperage = BitConverter.ToInt16(packet, i) / 100f;
                break;
            }
            default:
                break;
        }
    }

    public void MspGetAttitude()
    {
        if (_portPause) return;
        const ushort lenV2 = 0;
        const ushort commandV2 = 108;
        var data = new byte[9 + lenV2];
        data[0] = 36; //'$' = 36;
        data[1] = 88; //'M' = 77;
        data[2] = 60; //'<' = 60; // '>' = 62 // '|' = 33
        data[3] = 0; // Всегда 0
        Array.Copy(BitConverter.GetBytes(commandV2), 0, data, 4, 2);
        Array.Copy(BitConverter.GetBytes(lenV2), 0, data, 6, 2);
        data[8] = MspGetCrcV2(data); // CRC v2

        CliWrite(data);
    }

    public void MspGetImu()
    {
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

        CliWrite(data);
    }

    public void MspGetMotors()
    {
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

        CliWrite(data);
    }

    public void MspGetAnalog()
    {
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

        CliWrite(data);
    }

    public void MspUpdateRc()
    {
        if (_portPause) return;

        const ushort lenV2 = 0;
        const ushort commandV2 = 105;
        var data = new byte[9 + lenV2];
        data[0] = 36;//'$' = 36;
        data[1] = 88;//'M' = 77;
        data[2] = 60;//'<' = 60; // '>' = 62 // '|' = 33
        data[3] = 0; // Всегда 0
        Array.Copy(BitConverter.GetBytes(commandV2), 0, data, 4, 2);
        Array.Copy(BitConverter.GetBytes(lenV2), 0, data, 6, 2);
        data[8] = MspGetCrcV2(data); // CRC v2

        CliWrite(data);
    }

    public void MspSetMotor(int pwm1, int pwm2, int pwm3, int pwm4)
    {
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

        CliWrite(data);
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
        if (_portPause) return;

        var p1 = MotorsPwm[0];
        var p2 = MotorsPwm[1];
        var p3 = MotorsPwm[2];
        var p4 = MotorsPwm[3];

        if (motor == 255)
        {
            MspSetMotor(1000, 1000, 1000, 1000);
        }
        else
        {
            MspSetMotor(motor == 0 ? 1000 : p1, motor == 1 ? 1000 : p2, motor == 2 ? 1000 : p3, motor == 3 ? 1000 : p4);
        }

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

        CliWrite(data);
        _portPause = false;

        MspSetMotor(p1, p2, p3, p4);
    }

    public void MspSetCalibrateAcel()
    {
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

        CliWrite(data);
    }
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