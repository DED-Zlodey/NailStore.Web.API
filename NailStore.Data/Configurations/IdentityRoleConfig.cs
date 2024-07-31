using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NailStore.Data.Configurations;

public class IdentityRoleConfig : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> ent)
    {
        ent.ToTable(name: "roles");
        ent.HasKey(x => x.Id);
        ent.Property(x => x.Name).HasMaxLength(20).HasColumnName("name");
        ent.Property(x => x.Id).HasColumnName("role_id");
        ent.Property(x => x.NormalizedName).HasMaxLength(20).HasColumnName("normalized_name");
        ent.Property(x => x.ConcurrencyStamp).HasColumnName("concurrency_stamp");
    }
}