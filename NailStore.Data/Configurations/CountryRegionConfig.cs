using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации для сущности <see cref="Region"/>.
/// Настраивает сопоставление столбцов, имя таблицы, первичный ключ, ограничения,
/// связи с другими сущностями и индексы.
/// </summary>
public class CountryRegionConfig : IEntityTypeConfiguration<Region>
{
    /// <summary>
    /// Настраивает конструктор сущности для типа <see cref="Region"/>.
    /// </summary>
    /// <param name="ent">Конструктор сущности для типа <see cref="Region"/>.</param>
    public void Configure(EntityTypeBuilder<Region> ent)
    {
        // Устанавливает имя таблицы в базе данных и первичный ключ
        ent.ToTable("regions")
            .HasKey(x => x.RegionId);

        // Настраивает сопоставление столбца для свойства CountryId
        ent.Property(x => x.CountryId).HasColumnName("country_id");

        // Настраивает сопоставление столбца для свойства RegionName.
        // Устанавливает ограничение, что это поле не может быть пустым
        ent.Property(x => x.RegionName).HasColumnName("region_name").IsRequired();

        // Настраивает сопоставление столбца для свойства RegionId
        ent.Property(x => x.RegionId).HasColumnName("region_id");

        // Настраивает связь между сущностями Region и City.
        // Указывает, что один регион может иметь несколько городов.
        // Указывает внешний ключ для связи
        ent.HasMany(x => x.Cities).WithOne(x => x.Region).HasForeignKey(x => x.RegionId);

        // Настраивает индекс на свойстве CountryId
        ent.HasIndex(x => x.CountryId);

        // Настраивает индекс на свойстве RegionId
        ent.HasIndex(x => x.RegionId);
    }
}