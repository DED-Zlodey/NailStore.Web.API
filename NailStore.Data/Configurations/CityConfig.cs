using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

public class CityConfig : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> ent)
    {
        ent.ToTable("cities").HasKey(x => x.CityId);
        ent.Property(x => x.CityId).HasColumnName("city_id");
        ent.Property(x => x.RegionId).HasColumnName("region_id");
        ent.Property(x => x.NameCity).HasColumnName("name_city").IsRequired();
        ent.Property(x => x.TimeZone).HasColumnName("time_zone").IsRequired();
        ent.Property(x => x.Coordinates).HasColumnName("coordinates").HasColumnType("geometry (point)");

        ent.HasIndex(x => x.CityId);
        ent.HasIndex(x => x.RegionId);
    }
}