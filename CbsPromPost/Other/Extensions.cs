using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace CbsPromPost.Other;

public static class Extensions
{
    public static string ToStringF2(this double num) // Приведение double к виду "0.00"
    {
        return num.ToString("F2", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
    }
    public static string ToStringF2(this float num) // Приведение float к виду "0.00"
    {
        return num.ToString("F2", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
    }
    public static string ToSecTime(this double sec) // Приведение секунд к виду 00:00:00
    {
        if (sec <= 0) return "00:00:00";

        var second = Math.Truncate(sec % 60);
        var hour = Math.Truncate(sec / 3600d);
        var minutes = Math.Truncate(sec / 60d) - hour * 60d;

        return sec > 99 * 60 * 60 ? string.Empty : $"{hour:00}:{minutes:00}:{second:00}";
    }

    public static void SetDoubleBuffered(this Control control) // Установка и включения дабл-буфера для объекта формы
    {
        typeof(Control).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, control, new object[] { true });
    }

    public static string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "0.0.0.0";
    }
}