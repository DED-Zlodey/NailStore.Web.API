using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации для сущности услуги <see cref="ServiceModel"/>
/// Настраивает сопоставление столбцов, имя таблицы, первичный ключ и имена столбцов в базе данных.
/// </summary>
public class ServiceModelConfig : IEntityTypeConfiguration<ServiceModel>
{
    /// <summary>
    /// Настраивает конструктор сущности для типа <see cref="ServiceModel"/>
    /// </summary>
    /// <param name="ent">Конструктор сущности для типа ServiceModel.</param>
    public void Configure(EntityTypeBuilder<ServiceModel> ent)
    {
        // Устанавливает имя таблицы в базе данных и первичный ключ
        ent.ToTable("services")
            .HasKey(x => x.ServiceId);

        // Настраивает сопоставление столбца для свойства ServiceId
        ent.Property(x => x.ServiceId).HasColumnName("service_id").UseIdentityColumn();

        // Настраивает сопоставление столбца для свойства CategoryId
        ent.Property(x => x.CategoryId).HasColumnName("category_id");
        // Настраивает сопоставление столбца для свойства ServiceName
        ent.Property(x => x.ServiceName).HasColumnName("service_name").HasMaxLength(70);
        // Настраивает сопоставление столбца для свойства UserId
        ent.Property(x => x.UserId).HasColumnName("user_id");
        // Настраивает сопоставление столбца для свойства DurationTime
        ent.Property(x => x.DurationTime).HasColumnName("duration");
        // Настраивает сопоставление столбца для свойства Price
        ent.Property(x => x.Price).HasColumnName("price");

        // Настраивает сопоставление столбца для свойства DescriptionService
        ent.Property(x => x.DescriptionService).HasColumnName("description_service").HasMaxLength(300);

        // Настраивает отношения между сущностями
        ent.HasMany(x => x.ServiceDescriptions).WithOne(x => x.Service).HasForeignKey(x => x.ServiceId);
        ent.HasOne(x => x.Category).WithMany(x => x.Services).HasForeignKey(x => x.CategoryId);
        ent.HasMany(x => x.Cities).WithMany(x => x.Services).UsingEntity<CityToService>();

        // Настраивает индексы для свойств сущности
        ent.HasIndex(x => x.ServiceId);
        ent.HasIndex(x => x.UserId);
        ent.HasIndex(x => x.CategoryId);
    }
}