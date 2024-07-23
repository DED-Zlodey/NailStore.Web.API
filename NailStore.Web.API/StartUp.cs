using NailStore.Application.Settings;
using NailStore.Web.API.ConfigureServices;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace NailStore.Web.API;

/// <summary>
/// 
/// </summary>
public class StartUp
{
    /// <summary>
    /// 
    /// </summary>
    public IConfiguration Configuration { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public StartUp(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    /// <summary>
    /// Конфигурирование и добавление сервисов
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureCustomServices configService = new ConfigureCustomServices();
        configService.ConfigureLogging(services);
        configService.ConfigureForwardedHeaders(services);
        services.Configure<SrvSettings>(Configuration.GetSection("SrvSettings"));
        SrvSettings srvSettings = new SrvSettings();
        Configuration.GetSection("SrvSettings").Bind(srvSettings);
        configService.ConfigureJWT(services, srvSettings);
        configService.ConfigureDBServices(services, Configuration);
        configService.ConfigureServicesIdentityCore(services);
        configService.ConfigureCommonCustomServices(services);
        configService.ConfigureCoockies(services);
        configService.ConfigureSwagger(services);
        services.AddControllers();
        services.AddCors();
    }
    /// <summary>
    /// Конфигурирование конвейера обработки запроса
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSerilogRequestLogging(options => options.EnrichDiagnosticContext = RequestEnricher.LogAdditionalInfo);
        SrvSettings srvSettings = new SrvSettings();
        Configuration.GetSection("SrvSettings").Bind(srvSettings);
        app.UseForwardedHeaders();
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.DocExpansion(DocExpansion.None);
                options.RoutePrefix = string.Empty;
            });
        }
        app.UseSwagger();
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseCors(builder => builder
                        .WithOrigins(srvSettings.CorsHosts!)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
