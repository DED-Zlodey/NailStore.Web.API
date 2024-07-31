using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

public class ServiceDescriptionModelConfig : IEntityTypeConfiguration<ServiceDescriptionModel>
{
    public void Configure(EntityTypeBuilder<ServiceDescriptionModel> ent)
    {
        ent.ToTable("descriptions_service")
            .HasKey(x => x.DescriptionId);
        ent.Property(x => x.DescriptionId).HasColumnName("description_id").UseIdentityColumn();
        ent.Property(x => x.ServiceId).HasColumnName("service_id");
        ent.Property(x => x.Number).HasColumnName("number");
        ent.Property(x => x.Text).HasColumnName("text").HasMaxLength(500).IsRequired();

        ent.HasIndex(x => x.DescriptionId);
        ent.HasIndex(x => x.ServiceId);
    }
}