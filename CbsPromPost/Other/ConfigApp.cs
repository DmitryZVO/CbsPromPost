﻿using System.Text.Encodings.Web;
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
    public string RelayPowerIp { get; set; } = "192.168.101.91";
    public string DirRecords { get; set; } = "C:\\#CAPTURE\\";

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
            Core.ConfigDb.Save();
        }
        catch
        {
            return false;
        }
        return true;
    }
}
