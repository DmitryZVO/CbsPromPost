using System.Text;
using System.Text.Json;
using CbsPromPost.Other;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CbsPromPost.Model;

public class Works
{
    public DateTime TimeStamp { get; set; } = DateTime.MinValue;
    public List<Work> Items { get; set; } = new();
    public Action OnChange { get; set; } = delegate { };

    public Work Get(long id)
    {
        var ret = Items.Find(x => x.Id == id);
        return ret ?? new Work();
    }

    public async Task UpdateAsync(CancellationToken ct)
    {
        HttpClient web = new()
        {
            BaseAddress = new Uri(Core.Config.ServerUrl),
        };

        try
        {
            using var answ = await web.GetAsync("GetWorks", ct);

            if (answ.IsSuccessStatusCode)
            {
                var json = await answ.Content.ReadAsStringAsync(ct);
                var values = JsonSerializer.Deserialize<Works>(json);
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
#pragma warning disable CA2254
        Core.IoC.Services.GetRequiredService<ILogger<Works>>().Log(level, $"*Works* {log}");
#pragma warning restore CA2254
    }

    public class Work
    {
        public long Id { get; set; } = -1;
        public long Control { get; set; } = -1;
        public string Name { get; set; } = string.Empty;
        public long TimeNormalSec { get; set; } = -1;
        public long TimePauseSec { get; set; } = -1;
        public long TimePauseLongSec { get; set; } = -1;
        public long CostRub { get; set; }
        public StocksOperations StockDec { get; set; } = new();
        public StocksOperations StockInc { get; set; } = new();
        public long State { get; set; }

        public class StocksOperations
        {
            public List<StockValue> Items { get; set; } = new();

            public static StocksOperations FromString(string str)
            {
                var items = new StocksOperations();
                var ss = str.Split("/");
                foreach (var s in ss)
                {
                    var vals = s.Split("=");
                    if (vals.Length < 2) continue;
                    if (!long.TryParse(vals[0], out var id)) continue;
                    if (!long.TryParse(vals[1], out var count)) continue;
                    items.Items.Add(new StockValue { Count = count, IdStock = id });
                }
                return items;
            }

            public override string ToString()
            {
                var ret = new StringBuilder();
                foreach (var v in Items)
                {
                    ret.Append($"{v.IdStock:0}={v.Count:0}/");
                }

                var str = ret.ToString();
                return str.Length > 0 ? ret.ToString()[..^1] : str;
            }
        }
        public class StockValue
        {
            public long IdStock { get; set; }
            public long Count { get; set; }
        }
    }
}