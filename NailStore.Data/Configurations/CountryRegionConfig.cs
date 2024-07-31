using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

public class CountryRegionConfig : IEntityTypeConfiguration<CountryRegion>
{
    public void Configure(EntityTypeBuilder<CountryRegion> ent)
    {
        ent.ToTable("regions").HasKey(x => x.RegionId);
        ent.Property(x => x.CountryId).HasColumnName("country_id");
        ent.Property(x => x.RegionName).HasColumnName("region_name").IsRequired();
        ent.Property(x => x.RegionId).HasColumnName("region_id");
        //ent.Property(x => x.Cities).HasColumnName("cities");
        
        ent.HasMany(x => x.Cities).WithOne(x => x.Region).HasForeignKey(x => x.RegionId);

        ent.HasIndex(x => x.CountryId);
        ent.HasIndex(x => x.RegionId);
    }
}