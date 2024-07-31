using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NailStore.Data.Configurations;

public class IdentityRoleClaimConfig : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> ent)
    {
        ent.ToTable(name: "role_claims");
        ent.HasKey(x => x.Id);
        ent.Property(x => x.Id).HasColumnName("claim_id");
        ent.Property(x => x.RoleId).HasColumnName("role_id");
        ent.Property(x => x.ClaimType).HasColumnName("claim_type");
        ent.Property(x => x.ClaimValue).HasColumnName("claim_value");
    }
}