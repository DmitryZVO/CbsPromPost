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