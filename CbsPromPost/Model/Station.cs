using System.ServiceProcess;
using System.Text.Json;
using CbsPromPost.Other;

namespace CbsPromPost.Model;

public class Station
{
    public DateTime TimeStamp { get; set; } = DateTime.MinValue;
    public long Number { get; set; }
    public DateTime WorkStart { get; set; } = DateTime.MinValue;
    public string Address { get; set; } = string.Empty;
    public long Type { get; set; } = -1;
    public Users.User User { get; set; } = new();
    public double Version { get; set; } = 1.05d;

    public async void StartAsync(CancellationToken ct = default)
    {
        Number = Core.Config.PostNumber;
        Address = Extensions.GetLocalIpAddress();
        Type = Core.Config.Type;
        User = new Users.User();
        TimeStamp = DateTime.Now;

        //await StartWorkAsync(new Users.User(), default);

        while (!ct.IsCancellationRequested)
        {
            await PingAsync(ct);
            await Task.Delay(1000, ct);
        }
    }

    private async Task PingAsync(CancellationToken ct)
    {
        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            using var answ = await web.GetAsync($"StationPing?id={Number}&ip={Address}&type={Type:0}&version={Version * 100d:0}", ct);
            if (answ.IsSuccessStatusCode)
            {
                var json = await answ.Content.ReadAsStringAsync(ct);
                var value = JsonSerializer.Deserialize<Station>(json);
                if (value == null) return;
                User = value.User;
                WorkStart = value.WorkStart;
            }
        }
        catch
        {
            //
        }
    }

    public async Task StartWorkAsync(Users.User user, CancellationToken ct)
    {
        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            using var answ = await web.GetAsync($"WorkStart?id={Number:0}&user={user.Id:0}", ct);
        }
        catch
        {
            //
        }
    }

    public async Task ChangeWorkTimeAsync(DateTime newTime, CancellationToken ct)
    {
        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            using var answ = await web.GetAsync($"WorkStartChange?id={Number:0}&newTimeTicks={newTime.Ticks:0}", ct);
        }
        catch
        {
            //
        }
    }

    public async Task<string> FinishBodyAsync(string text, CancellationToken ct)
    {
        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            using var answ = await web.GetAsync($"WorkFinish?stationId={Number:0}&workId={Type:0}&droneId={text}", ct);
            return answ.IsSuccessStatusCode ? string.Empty : await answ.Content.ReadAsStringAsync(ct);
        }
        catch
        {
            //
        }
        return "ПОВТОРИТЕ ПОПЫТКУ";
    }

    public async Task<long> GetCountsFinishWorks(CancellationToken ct)
    {
        try
        {
            using var web = new HttpClient();
            web.BaseAddress = new Uri(Core.Config.ServerUrl);
            using var answ = await web.GetAsync($"WorksGetCounts?userId={User.Id:0}&workId={Core.Config.Type:0}", ct);
            return answ.IsSuccessStatusCode ? long.Parse(await answ.Content.ReadAsStringAsync(ct)) : 0;
        }
        catch
        {
            //
        }
        return 0;
    }

    public static void ServiceStart(string serviceName)
    {
        var service = new ServiceController(serviceName);
        if (service.Status == ServiceControllerStatus.Running) return;

        service.Start();
    }
    public static void ServiceStop(string serviceName)
    {
        var service = new ServiceController(serviceName);
        if (service.Status == ServiceControllerStatus.Stopped) return;

        service.Stop(true);
    }
    public static ServiceControllerStatus ServiceInfo(string serviceName)
    {
        var service = new ServiceController(serviceName);
        return service.Status;
    }
}
