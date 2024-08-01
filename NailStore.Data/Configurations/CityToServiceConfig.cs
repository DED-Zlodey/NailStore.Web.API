using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации для сущности связи города с услугой <see cref="CityToService"/>.
/// Настраивает сопоставление столбцов, имя таблицы и первичный ключ.
/// </summary>
public class CityToServiceConfig : IEntityTypeConfiguration<CityToService>
{
    /// <summary>
    /// Настраивает конструктор сущности для типа <see cref="CityToService"/>.
    /// </summary>
    /// <param name="ent">Конструктор сущности для типа <see cref="CityToService"/>.</param>
    public void Configure(EntityTypeBuilder<CityToService> ent)
    {
        // Устанавливает имя таблицы в базе данных
        ent.ToTable("city_to_service");

        // Настраивает сопоставление столбца для свойства CityId
        ent.Property(x => x.CityId).HasColumnName("city_id");

        // Настраивает сопоставление столбца для свойства ServiceId
        ent.Property(x => x.ServiceId).HasColumnName("service_id");

        // Настраивает составной первичный ключ для свойств CityId и ServiceId
        ent.HasKey(x => new { x.CityId, x.ServiceId });
    }
}