using System.Text.Json;
using CbsPromPost.Other;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CbsPromPost.Model;

public class Users
{
    public DateTime TimeStamp { get; set; } = DateTime.MinValue;
    public List<User> Items { get; set; } = new();
    public Action OnChange { get; set; } = delegate { };

    public async Task UpdateAsync(CancellationToken ct)
    {
        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            using var answ = await web.GetAsync("GetUsers", ct);

            if (answ.IsSuccessStatusCode)
            {
                var json = await answ.Content.ReadAsStringAsync(ct);
                var values = JsonSerializer.Deserialize<Users>(json);
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
        Core.IoC.Services.GetRequiredService<ILogger<Users>>().Log(level, $"*Users* {log}");
#pragma warning restore CA2254
    }

    public class User
    {
        public long Id { get; set; } = -1;
        public string Name { get; set; } = string.Empty;
        public long State { get; set; } = -1;
    }
}