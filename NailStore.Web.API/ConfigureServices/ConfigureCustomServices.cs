using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NailStore.Application;
using NailStore.Application.Settings;
using NailStore.Core.Interfaces;
using NailStore.Data;
using NailStore.Data.Models;
using NailStore.Repositories;
using Serilog;
using System.Reflection;
using System.Text;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.HttpOverrides;

namespace NailStore.Web.API.ConfigureServices;

/// <summary>
/// 
/// </summary>
public struct ConfigureCustomServices
{
    /// <summary>
    /// Конфигурирование IdentityCore
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServicesIdentityCore(IServiceCollection services)
    {
        services.AddIdentityCore<UserEntity>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
                options.Lockout = new LockoutOptions
                {
                    AllowedForNewUsers = true,
                    MaxFailedAccessAttempts = 3,
                    DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5),
                };
            }).AddRoles<IdentityRole<Guid>>()
            .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
            .AddSignInManager<SignInManager<UserEntity>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }

    /// <summary>
    /// Конфигурирование своих сервисов
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureCommonCustomServices(IServiceCollection services)
    {
        services
            .AddScoped<IUserService, UserService>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IJWTManager, JWTManager>()
            .AddSingleton<IEmailService, EmailService>()
            .AddScoped<IProviderService<Guid>, ProviderService>()
            .AddScoped<IServiceRepository<Guid>, ServiceRepository>()
            .AddScoped<ICountryService, CountryService>()
            .AddScoped<ICountryRepository, CountryRepository>()
            .AddSingleton<SmtpClient>()
            ;
    }

    /// <summary>
    /// Конфигурирование контекста БД
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public void ConfigureDBServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreConnection"), o =>
            {
                o.UseNetTopologySuite();
            }).LogTo(Console.WriteLine, LogLevel.Information));
    }

    /// <summary>
    /// Конфигурирование Swagger`a
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme,
                securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Please enter a valid JWT token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new string[] { }
                }
            });
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "API for Web Service NailStore",
                Description = "Для сайта ухода за ногтями",
            });
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });
    }

    /// <summary>
    /// Конфигурирование кукисов
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureCoockies(IServiceCollection services)
    {
        services.AddAntiforgery(options => options.Cookie.Name = "NailStore.FormId")
            .ConfigureApplicationCookie(options => { options.Cookie.Name = "NailStoreId"; })
            .Configure<CookieTempDataProviderOptions>(options => options.Cookie.Name = "NailStore.Provider");
    }

    /// <summary>
    /// Конфигурирование JWT авторизации/аутентификации
    /// </summary>
    /// <param name="services"></param>
    /// <param name="srvSettings"></param>
    public void ConfigureJWT(IServiceCollection services, SrvSettings srvSettings)
    {
        services.AddEndpointsApiExplorer();
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(srvSettings.ServerKey!)),
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "NailStoreApi",
                ValidAudience = "NailStore.Company",
            });
        services.AddAuthorization();
    }

    /// <summary>
    /// Конфигурирование логирования
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureLogging(IServiceCollection services)
    {
        services.AddLogging(builder => builder.AddSerilog(dispose: true));
    }

    /// <summary>
    /// Конфигурирование проброса заголовков
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureForwardedHeaders(IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });
    }
}