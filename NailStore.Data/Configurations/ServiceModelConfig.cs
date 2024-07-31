using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

public class ServiceModelConfig : IEntityTypeConfiguration<ServiceModel>
{
    public void Configure(EntityTypeBuilder<ServiceModel> ent)
    {
        ent.ToTable("services")
            .HasKey(x => x.ServiceId);
        ent.Property(x => x.ServiceId).HasColumnName("service_id").UseIdentityColumn();
        ent.Property(x => x.CategoryId).HasColumnName("category_id");
        ent.Property(x => x.ServiceName).HasColumnName("service_name").HasMaxLength(70);
        ent.Property(x => x.UserId).HasColumnName("user_id");
        ent.Property(x => x.DurationTime).HasColumnName("duration");
        ent.Property(x => x.Price).HasColumnName("price");

        ent.Property(x => x.DescriptionService).HasColumnName("description_service").HasMaxLength(300);
        ent.HasMany(x => x.ServiceDescriptions).WithOne(x => x.Service).HasForeignKey(x => x.ServiceId);
        ent.HasOne(x => x.Category).WithMany(x => x.Services).HasForeignKey(x => x.CategoryId);
        ent.HasMany(x => x.Cities).WithMany(x => x.Services).UsingEntity<CityToService>();

        // Indexes
        ent.HasIndex(x => x.ServiceId);
        ent.HasIndex(x => x.UserId);
        ent.HasIndex(x => x.CategoryId);
    }
}