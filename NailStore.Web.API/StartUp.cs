using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NailStore.Data;
using Microsoft.EntityFrameworkCore;
using NailStore.Data.Models;
using Serilog.Events;
using Serilog;
using Microsoft.AspNetCore.HttpOverrides;
using NailStore.Application.Settings;
using NailStore.Web.API.ConfigureServices;

namespace NailStore.Web.API;

public class StartUp
{
    public IConfiguration Configuration { get; }
    private readonly string _osNameAndVersion;
    public StartUp(IConfiguration configuration)
    {
        _osNameAndVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        SetSerilog();
        Configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureCustomServices.ConfigureLogging(services);
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });
        services.Configure<SrvSettings>(Configuration.GetSection("SrvSettings"));
        SrvSettings srvSettings = new SrvSettings();
        Configuration.GetSection("SrvSettings").Bind(srvSettings);
        ConfigureCustomServices.ConfigureJWT(services, srvSettings);
        ConfigureCustomServices.ConfigureDBServices(services, Configuration);
        ConfigureCustomServices.ConfigureServices(services);
        ConfigureCustomServices.ConfigureCommonCustomServices(services);
        ConfigureCustomServices.ConfigureCoockies(services);
        ConfigureCustomServices.ConfigureSwagger(services);
        services.AddControllers();
        services.AddSignalR();
        services.AddCors();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
    private void SetSerilog()
    {
        var nameDir = Directory.GetCurrentDirectory() + @"/Logs/webapi-.txt";
        if (_osNameAndVersion.Contains("Windows"))
        {
            nameDir = Directory.GetCurrentDirectory() + @"\Logs\webapi-.txt";
        }
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).Enrich.FromLogContext().CreateLogger();
            //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            //.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
            //.MinimumLevel.Override("Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager", LogEventLevel.Error)
            //.MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.DefaultActionDescriptorCollectionProvider", LogEventLevel.Error)
            //.MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Fatal)
            //.MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", LogEventLevel.Fatal)
            //.WriteTo.Console(LogEventLevel.Information, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            //.WriteTo.File(nameDir, LogEventLevel.Information, rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            //.CreateLogger();
        Log.Logger.Information("OS Version: {_osNameAndVersion}", _osNameAndVersion);
    }
}
