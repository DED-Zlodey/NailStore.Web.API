using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NailStore.Data.Configurations;

public class IdentityUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> ent)
    {
        ent.HasKey(x => new { x.UserId, x.RoleId });
        ent.ToTable(name: "user_roles");
        ent.Property(x => x.UserId).HasColumnName("user_id");
        ent.Property(x => x.RoleId).HasColumnName("role_id");
        ent.HasIndex(x => new { x.UserId, x.RoleId });
    }
}