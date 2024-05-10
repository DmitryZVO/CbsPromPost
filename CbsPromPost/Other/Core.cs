using CbsPromPost.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CbsPromPost.Other;

internal static class Core
{
    public static ConfigApp Config = new ConfigApp().Load();
    public static ConfigDbApp ConfigDb = new ConfigDbApp().Load();
    public static IHost IoC { get; private set; } = Host.CreateDefaultBuilder(null).Build();

    public static void Start()
    {
        IoC = Host.CreateDefaultBuilder(null)
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<Server>();
                services.AddSingleton<Station>();
                services.AddSingleton<Works>();
                services.AddSingleton<Stocks>();
                services.AddSingleton<Users>();
                services.AddSingleton<RelayPower>();
            })
            .ConfigureLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(new LoggerToFilesProvider());
                builder.AddSimpleConsole(config =>
                {
                    config.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff] ";
                    config.SingleLine = true;
                });
            })
            .Build();

        IoC.Services.GetRequiredService<Server>().StartAsync();
        IoC.Services.GetRequiredService<Station>().StartAsync();
        IoC.Services.GetRequiredService<RelayPower>().StartAsync();

        Config.Save();

        if (Config.TestMode)
        {
        }
    }
}