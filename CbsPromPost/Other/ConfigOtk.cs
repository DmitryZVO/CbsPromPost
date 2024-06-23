using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace CbsPromPost.Other;

public struct ConfigOtk
{
    public string CamFullNumber { get; set; } = "0";
    public byte CamFullFocus { get; set; } = 0;
    public string CamUpNumber { get; set; } = "rtsp://admin:admin@192.168.101.221:554/1";
    public byte CamUpFocus { get; set; } = 0;
    public string CamLeftNumber { get; set; } = "1";
    public byte CamLeftFocus { get; set; } = 0;
    public string CamRightNumber { get; set; } = "2";
    public byte CamRightFocus { get; set; } = 0;

    public ConfigOtk()
    {
    }

    public ConfigOtk Load()
    {
        try
        {
            using var sr = new StreamReader(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "_otk.ini", FileMode.Open));
            this = JsonSerializer.Deserialize<ConfigOtk>(sr.ReadToEnd());
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
            using var sw = new StreamWriter(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "_otk.ini", FileMode.Create, FileAccess.Write));
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
