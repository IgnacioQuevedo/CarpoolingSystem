using StatisticsServerAPI.Services;
using WebApiRabbitMQ;

namespace StatisticsServerAPI;

public class Program
{

    //private static MQUserService mqUserService;
    
    public static void Main(string[] args)
    {
        //mqUserService = new MQUserService();
        CreateHostBuilder(args).Build().Run();
    }
    
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}