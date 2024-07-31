using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

public class CityToServiceConfig : IEntityTypeConfiguration<CityToService>
{
    public void Configure(EntityTypeBuilder<CityToService> ent)
    {
        ent.ToTable("city_to_service");
        ent.Property(x => x.CityId).HasColumnName("city_id");
        ent.Property(x => x.ServiceId).HasColumnName("service_id");
        ent.HasKey(x => new { x.CityId, x.ServiceId });
    }
}