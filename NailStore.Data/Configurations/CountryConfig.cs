using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

public class CountryConfig : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> ent)
    {
        ent.ToTable(name: "countries")
            .HasKey(x => x.CountryId);
        ent.Property(x => x.CountryId).HasColumnName("country_id");
        ent.Property(x => x.CountryName).HasMaxLength(70).HasColumnName("country_name");
        ent.HasMany(x => x.Regions).WithOne(x => x.Country).HasForeignKey(x => x.RegionId);
        //ent.Property(x => x.Regions).HasColumnName("regions");
        
        ent.HasIndex(x => x.CountryId);
    }
}