using TrackYourLife.Common.Infrastructure;

namespace TrackYourLifeDotnet.App;

public class Program
{
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureSerilog()
            .ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }
            );
}
