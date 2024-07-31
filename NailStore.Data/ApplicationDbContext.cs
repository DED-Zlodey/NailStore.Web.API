using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NailStore.Data.Models;

namespace NailStore.Data;

/// <summary>
/// Представляет контекст базы данных для приложения Nail Store.
/// Наследуется от IdentityDbContext для поддержки аутентификации и авторизации пользователей.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
{
    /// <summary>
    /// Получает или устанавливает набор DbSet для сущностей CategoryService.
    /// </summary>
    public DbSet<CategoryServiceModel> CategoriesServices { get; set; }

    /// <summary>
    /// Получает или устанавливает набор DbSet для сущностей Service.
    /// </summary>
    public DbSet<ServiceModel> Services { get; set; }

    /// <summary>
    /// Получает или устанавливает набор DbSet для сущностей ServiceDescription.
    /// </summary>
    public DbSet<ServiceDescriptionModel> DescriptionsService { get; set; }

    /// <summary>
    /// Получает или устанавливает набор DbSet для сущностей Country.
    /// </summary>
    public DbSet<Country> Countries { get; set; }

    /// <summary>
    /// Получает или устанавливает набор DbSet для сущностей CountryRegion.
    /// </summary>
    public DbSet<CountryRegion> Regions { get; set; }

    /// <summary>
    /// Получает или устанавливает набор DbSet для сущностей City.
    /// </summary>
    public DbSet<City> Cities { get; set; }
    public DbSet<CityToService> CityToService { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр класса ApplicationDbContext.
    /// </summary>
    /// <param name="options">Параметры, которые будут использоваться контекстом.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        //Database.EnsureCreated();
    }

    /// <summary>
    /// Переопределите этот метод, чтобы настроить модель для контекста, который создается.
    /// </summary>
    /// <param name="builder">Строитель, используемый для построения модели для этого контекста.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresExtension("postgis");
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}