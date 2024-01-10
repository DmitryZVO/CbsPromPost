using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace CbsPromPost.Other;

public struct ConfigApp
{
    public bool TestMode { get; set; } = false;
    public bool ServiceUpdateDisable { get; set; } = true;
    public string ServerUrl { get; set; } = "http://192.168.101.98:9999/";
    public long PostNumber { get; set; } = -1;
    public long Type { get; set; } = -1;
    public string ComScanner { get; set; } = "COM1";
    public string ComBeta { get; set; } = "COM3";
    public string UsbDfuVid { get; set; } = "0483";
    public string UsbDfuPid { get; set; } = "DF11";

    public List<Firmware> Firmwares { get; set; } = new()
    {
        new Firmware { FileBin = string.Empty, FileFpl = string.Empty, Name = "_DEFAULT" } 
    };
    public class Firmware
    {
        public string Name { get; set; } = string.Empty;
        public string FileBin { get; set; } = string.Empty;
        public string FileFpl { get; set; } = string.Empty;
    }

public ConfigApp()
    {
    }

    public ConfigApp Load()
    {
        try
        {
            using var sr = new StreamReader(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "_global.ini", FileMode.Open));
            this = JsonSerializer.Deserialize<ConfigApp>(sr.ReadToEnd());
        }
        catch
        {
            Save();
        }
        return this;
    }

    public readonly bool Save()
    {
        try
        {
            using var sw = new StreamWriter(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "_global.ini", FileMode.Create, FileAccess.Write));
            sw.WriteLine(JsonSerializer.Serialize(this,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
                }));
        }
        catch
        {
            return false;
        }
        return true;
    }
}
