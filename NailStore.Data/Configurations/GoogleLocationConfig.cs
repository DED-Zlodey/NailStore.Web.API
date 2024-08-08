using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

public class GoogleLocationConfig : IEntityTypeConfiguration<GoogleLocation>
{
    /// <summary>
    /// Конфигурация сущности GoogleLocation в контексте Entity Framework Core.
    /// </summary>
    public void Configure(EntityTypeBuilder<GoogleLocation> ent)
    {
        // Указывает имя таблицы в базе данных как "geolocation".
        ent.ToTable("geolocations")
            .HasKey(x => x.LocationId); // Определяет LocationId как первичный ключ.

        // Настраивает сопоставление столбцов для свойств модели.
        ent.Property(x => x.LocationId).HasColumnName("location_id");
        ent.Property(x => x.RegionId).HasColumnName("region_id");
        ent.Property(x => x.Postcode).HasColumnName("postcode");
        ent.Property(x => x.Country).HasColumnName("country_name");
        ent.Property(x => x.City).HasColumnName("name_city");
        ent.Property(x => x.Street).HasColumnName("street");
        ent.Property(x => x.House).HasColumnName("house");

        // Обязательное поле Address.
        ent.Property(x => x.Address).HasColumnName("address").IsRequired();

        // Настраивает сопоставление столбца для свойства Coordinates, указывая тип данных как geometry (point).
        ent.Property(x => x.Coordinates).HasColumnName("coordinates").HasColumnType("geography (point)");

        // Определяет индексы для свойств LocationId и RegionId.
        ent.HasIndex(x => x.LocationId);
        ent.HasIndex(x => x.RegionId);

        // Добавляет индекс на свойство Coordinates с указанием метода индексирования как GIST.
        ent.HasIndex(x => x.Coordinates).HasMethod("GIST");
    }
}