using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации для сущности <see cref="Country"/>.
/// Настраивает сопоставление столбцов, имя таблицы, первичный ключ, ограничения длины,
/// связи с другими сущностями и индексы.
/// </summary>
public class CountryConfig : IEntityTypeConfiguration<Country>
{
    /// <summary>
    /// Настраивает конструктор сущности для типа <see cref="Country"/>.
    /// </summary>
    /// <param name="ent">Конструктор сущности для типа <see cref="Country"/>.</param>
    public void Configure(EntityTypeBuilder<Country> ent)
    {
        // Устанавливает имя таблицы в базе данных
        ent.ToTable(name: "countries")
            .HasKey(x => x.CountryId);

        // Настраивает сопоставление столбца для свойства CountryId
        ent.Property(x => x.CountryId).HasColumnName("country_id");

        // Настраивает сопоставление столбца для свойства CountryName.
        // Устанавливает ограничение длины в 70 символов
        ent.Property(x => x.CountryName).HasMaxLength(70).HasColumnName("country_name");

        // Настраивает связь между сущностями Country и Region.
        // Указывает внешний ключ для связи
        ent.HasMany(x => x.Regions).WithOne(x => x.Country).HasForeignKey(x => x.RegionId);

        // Настраивает индекс на свойстве CountryId
        ent.HasIndex(x => x.CountryId);
    }
}