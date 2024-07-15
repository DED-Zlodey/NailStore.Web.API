using Serilog;

namespace NailStore.Web.API;

public class Program
{
    public static void Main(string[] args)
    {
        var nameVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .CreateLogger();
        try
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
        catch (Exception ex)
        {
            Log.Error(ex, "The following {Exception} was thrown during application startup", ex.Message);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
        .UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);
        })
           .ConfigureWebHostDefaults(webBuilder =>
           {
               webBuilder.UseStartup<StartUp>();
           });
}