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

    private DateTime _lastAlive = DateTime.MinValue;

    public async void StartAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            await UpdateInfoAsync(ct);
            await Task.Delay(1000, ct);
        }
    }

    private async Task UpdateInfoAsync(CancellationToken ct)
    {
        HttpClient web = new()
        {
            BaseAddress = new Uri(Core.Config.ServerUrl),
        };

        var start = DateTime.Now;
        try
        {
            using var answ = await web.GetAsync("GetServer", ct);

            if (answ.IsSuccessStatusCode)
            {
                var json = await answ.Content.ReadAsStringAsync(ct);
                var values = JsonSerializer.Deserialize<WebState>(json);
                if (values == null) return;
                _lastAlive = DateTime.Now;
                RequestInSecond = values.RequestInSecond;
                TimeStamp = values.TimeStamp[TimeStampsTypes.Server];

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
        HttpClient web = new()
        {
            BaseAddress = new Uri(Core.Config.ServerUrl),
        };

        try
        {
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

}