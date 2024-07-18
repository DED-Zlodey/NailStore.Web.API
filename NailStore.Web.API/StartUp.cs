using Microsoft.AspNetCore.HttpOverrides;
using NailStore.Application.Settings;
using NailStore.Web.API.ConfigureServices;
using Serilog;

namespace NailStore.Web.API;

public class StartUp
{
    public IConfiguration Configuration { get; }
    public StartUp(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureCustomServices configService = new ConfigureCustomServices();
        configService.ConfigureLogging(services);
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });
        services.Configure<SrvSettings>(Configuration.GetSection("SrvSettings"));
        SrvSettings srvSettings = new SrvSettings();
        Configuration.GetSection("SrvSettings").Bind(srvSettings);
        configService.ConfigureJWT(services, srvSettings);
        configService.ConfigureDBServices(services, Configuration);
        configService.ConfigureServices(services);
        configService.ConfigureCommonCustomServices(services);
        configService.ConfigureCoockies(services);
        configService.ConfigureSwagger(services);
        services.AddControllers();
        services.AddCors();
    }
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
