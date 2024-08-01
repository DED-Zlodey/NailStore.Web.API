using Serilog;

namespace NailStore.Web.API;

public class Program
{
    public static void Main(string[] args)
    {
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
                    AppInit init = new AppInit(serviceProvider);
                    task = init.InitializeAsync();
                    task.Wait();
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.LogError(ex, " {method} Program | Main | Reason: {Message}", nameof(Main), ex.Message);
                }
            }
            host.Run();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "{method}, The following {Exception} was thrown during application startup", nameof(Main), ex.Message);
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