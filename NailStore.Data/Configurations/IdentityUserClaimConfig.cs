using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NailStore.Data.Configurations;

public class IdentityUserClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> ent)
    {
        ent.ToTable(name: "user_claims");
        ent.HasKey(x => x.Id);
        ent.Property(x => x.Id).HasColumnName("claim_id");
        ent.Property(x => x.UserId).HasColumnName("user_id").HasMaxLength(36).IsRequired();
        ent.Property(x => x.ClaimType).HasColumnName("claim_type");
        ent.Property(x => x.ClaimValue).HasColumnName("claim_value");
    }
}