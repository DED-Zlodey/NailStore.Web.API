using Serilog;

namespace NailStore.Web.API;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        Task task;
        try
        {
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                task = AppInit.InitializeAsync(serviceProvider);
                task.Wait();
            }
        }
        catch (Exception ex)
        {
            if (logger != null)
            {
                logger.LogError(ex, "Program | Main | Reason: {ex.Message}", ex.Message);
            }
        }
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
        .UseSerilog()
           .ConfigureWebHostDefaults(webBuilder =>
           {
               webBuilder.UseStartup<StartUp>();
           });
}