using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace CbsPromPost.Other;

public struct ConfigDbApp
{
    public string UsbDfuVid { get; set; } = "0483";
    public string UsbDfuPid { get; set; } = "DF11";
    public string LastFirmware { get; set; } = string.Empty;

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

public ConfigDbApp()
    {
    }

    public ConfigDbApp Load()
    {
        try
        {
            using var sr = new StreamReader(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "DB\\_db.ini", FileMode.Open));
            this = JsonSerializer.Deserialize<ConfigDbApp>(sr.ReadToEnd());
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
            using var sw = new StreamWriter(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "DB\\_db.ini", FileMode.Create, FileAccess.Write));
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
