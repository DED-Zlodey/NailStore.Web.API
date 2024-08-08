using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации для сущности города <see cref="City"/>.
/// Настраивает сопоставление столбцов, первичный ключ, обязательность свойств и индексы.
/// </summary>
public class CityConfig : IEntityTypeConfiguration<City>
{
    /// <summary>
    /// Конфигурирует сущность типа <see cref="City"/> для Entity Framework Core..
    /// </summary>
    /// <param name="ent">Конструктор сущности для типа <see cref="City"/>.</param>
    public void Configure(EntityTypeBuilder<City> ent)
    {
        // Устанавливает имя таблицы и первичный ключ для сущности City
        ent.ToTable("cities").HasKey(x => x.Id);

        // Настраивает сопоставление столбца для свойства CityId
        ent.Property(x => x.Id).HasColumnName("city_id");

        // Настраивает сопоставление столбца для свойства RegionId
        ent.Property(x => x.RegionId).HasColumnName("region_id");

        // Настраивает сопоставление столбца для свойства NameCity, делая его обязательным
        ent.Property(x => x.NameCity).HasColumnName("name_city").IsRequired();

        // Настраивает сопоставление столбца для свойства TimeZone, делая его обязательным
        ent.Property(x => x.TimeZone).HasColumnName("time_zone").IsRequired();

        // Настраивает сопоставление столбца для свойства Coordinates, указывая тип данных как geometry (point)
        ent.Property(x => x.Coordinates).HasColumnName("coordinates").HasColumnType("geography (point)");

        // Добавляет индекс на свойство CityId
        ent.HasIndex(x => x.Id);

        // Добавляет индекс на свойство RegionId
        ent.HasIndex(x => x.RegionId);
        // Добавляет индекс на свойство Coordinates
        ent.HasIndex(x => x.Coordinates).HasMethod("GIST");
    }
}