using System.Text.Json;
using CbsPromPost.Other;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CbsPromPost.Model;

public class Stocks
{
    public DateTime TimeStamp { get; set; } = DateTime.MinValue;
    public List<Stock> Items { get; set; } = new();
    public Action OnChange { get; set; } = delegate { };

    public Stock Get(long id)
    {
        var ret = Items.Find(x => x.Id == id);
        return ret ?? new Stock();
    }

    public async Task UpdateAsync(CancellationToken ct)
    {
        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            using var answ = await web.GetAsync("GetStocks", ct);

            if (answ.IsSuccessStatusCode)
            {
                var json = await answ.Content.ReadAsStringAsync(ct);
                var values = JsonSerializer.Deserialize<Stocks>(json);
                if (values == null) return;
                Items = values.Items.OrderByDescending(x => x.Name).Reverse().ToList();
                TimeStamp = values.TimeStamp;
                OnChange.Invoke();
            }
        }
        catch
        {
            //
        }
    }

    private static void WriteLog(LogLevel level, string log)
    {
        Core.IoC.Services.GetRequiredService<ILogger<Stocks>>().Log(level, $"*Stocks* {log}");
    }

    public class Stock
    {
        public long Id { get; set; } = -1;
        public string Name { get; set; } = string.Empty;
        public long Count { get; set; } = 0;
        public long Type { get; set; } = -1;
    }
}