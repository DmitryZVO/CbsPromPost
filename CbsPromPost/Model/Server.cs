using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using CbsPromPost.Other;
using Microsoft.Extensions.DependencyInjection;
using static CbsPromPost.Model.Works;

namespace CbsPromPost.Model;

public class Server
{
    public DateTime TimeStamp { get; set; } = DateTime.MinValue;
    public int RequestInSecond { get; set; }
    public bool Alive => (DateTime.Now - _lastAlive).TotalMilliseconds < 5000;
    public double AnswerTime { get; set; }
    public static string Prefix { get; set; } = string.Empty;

    private DateTime _lastAlive = DateTime.MinValue;

    public async void StartAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            await UpdateInfoAsync(ct);
            await Task.Delay(1000, ct);
        }
    }

    public struct SystemTime
    {
        public ushort Year;
        public ushort Month;
        public ushort DayOfWeek;
        public ushort Day;
        public ushort Hour;
        public ushort Minute;
        public ushort Second;
        public ushort Millisecond;
    };

    [DllImport("kernel32.dll", EntryPoint = "SetSystemTime", SetLastError = true)]
    private static extern bool Win32SetSystemTime(ref SystemTime sysTime);

    private static void ChangeTime(DateTime time)
    {
        var st = new SystemTime
        {
            Year = (ushort)time.Year,
            Month = (ushort)time.Month,
            Day = (ushort)time.Day,
            Hour = (ushort)time.Hour,
            Minute = (ushort)time.Minute,
            Second = (ushort)time.Second,
            DayOfWeek = (ushort)time.DayOfWeek,
            Millisecond = (ushort)time.Millisecond,
        };

        Win32SetSystemTime(ref st);
    }

    private async Task UpdateInfoAsync(CancellationToken ct)
    {
        var start = DateTime.Now;
        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            using var answ = await web.GetAsync("GetServer", ct);

            if (answ.IsSuccessStatusCode)
            {
                var json = await answ.Content.ReadAsStringAsync(ct);
                var values = JsonSerializer.Deserialize<WebState>(json);
                if (values == null) return;
                _lastAlive = DateTime.Now;
                RequestInSecond = values.RequestInSecond;
                var newTime = values.TimeStamp[TimeStampsTypes.Server];
                if (Math.Abs((DateTime.Now - newTime).Seconds) > 10) // Нужно обновить время на локальной машине
                {
                    ChangeTime(newTime.AddHours(-3)); // GTM +3
                }
                TimeStamp = newTime;
                Prefix = values.Prefix;

                var users = Core.IoC.Services.GetRequiredService<Users>();
                if (values.TimeStamp.TryGetValue(TimeStampsTypes.Users, out var stampUsers) && stampUsers > users.TimeStamp) await users.UpdateAsync(ct);
                var stocks = Core.IoC.Services.GetRequiredService<Stocks>();
                if (values.TimeStamp.TryGetValue(TimeStampsTypes.Stocks, out var stampStocks) && stampStocks > stocks.TimeStamp) await stocks.UpdateAsync(ct);
                var works = Core.IoC.Services.GetRequiredService<Works>();
                if (values.TimeStamp.TryGetValue(TimeStampsTypes.Works, out var stampWorks) && stampWorks > works.TimeStamp) await works.UpdateAsync(ct);
            }
        }
        catch
        {
            //
        }
        AnswerTime = (DateTime.Now - start).TotalMilliseconds;
    }

    public static async Task<List<HistoryItem>> GetHistoryForId(string id, CancellationToken ct)
    {
        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            using var answ = await web.GetAsync($"GetHistoryForId?id={id}", ct);

            if (answ.IsSuccessStatusCode)
            {
                var json = await answ.Content.ReadAsStringAsync(ct);
                var values = JsonSerializer.Deserialize<List<HistoryItem>>(json);
                return values ?? new List<HistoryItem>();
            }
        }
        catch
        {
            //
        }
        return new List<HistoryItem>();
    }

    public static async Task<string> CheckBadDrone(string droneId, CancellationToken ct)
    {
        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            using var answ = await web.GetAsync($"CheckDrone?droneId={droneId}", ct);

            if (answ.IsSuccessStatusCode)
            {
                var text = await answ.Content.ReadAsStringAsync(ct);
                return text;
            }

            return "ОШИБКА СЕРВЕРА";
        }
        catch
        {
            //
        }
        return "ОШИБКА СЕРВЕРА";
    }

    public static async Task<bool> AddBadDrone(string droneId, string text, CancellationToken ct)
    {
        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            var jsonString = JsonSerializer.Serialize(new BadDrone { Text = text });
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            content.Headers.ContentLength = jsonString.Length;

            using var answ = await web.PostAsync($"AddBadDrone?droneId={droneId}", content, ct);

            return answ.IsSuccessStatusCode;
        }
        catch
        {
            //
        }
        return false;
    }

    public static async Task<string> RemoveBadDrone(string droneId, CancellationToken ct)
    {
        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            using var answ = await web.GetAsync($"RemoveBadDrone?droneId={droneId}", ct);

            if (answ.IsSuccessStatusCode)
            {
                var text = await answ.Content.ReadAsStringAsync(ct);
                return text;
            }

            return "ОШИБКА СЕРВЕРА";
        }
        catch
        {
            //
        }
        return "ОШИБКА СЕРВЕРА";
    }

    public enum TimeStampsTypes
    {
        Logs = -2,
        Server = -1,
        Stations = 0,
        Users = 1,
        Works = 2,
        Stocks = 3,
        Drones = 4,
        History = 5,
        HistoryBox = 6,
    }
    protected class WebState
    {
        public float Version { get; set; }
        public string VersionString { get; set; } = string.Empty;
        public Dictionary<TimeStampsTypes, DateTime> TimeStamp { get; set; } = new();
        public int RequestInSecond { get; set; }
        public string Prefix { get; set; } = string.Empty;
    }

    public class HistoryItem
    {
        public DateTime TimeStart { get; set; } = DateTime.MinValue;
        public DateTime TimeEnd { get; set; } = DateTime.MinValue;
        public Users.User User { get; set; } = new();
        public Station Station { get; set; } = new();
        public Work Work { get; set; } = new();
        public long State { get; set; } = -1;
        public string DroneId { get; set; } = string.Empty;

    }

    public class BadDrone
    {
        public string Text { get; set; } = string.Empty;
    }
}