using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NailStore.Data.Configurations;

public class IdentityUserTokenConfig :IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> ent)
    {
        ent.ToTable(name: "user_tokens");
        ent.HasKey(x => x.UserId);
        ent.Property(x => x.UserId).HasColumnName("user_id");
        ent.Property(x => x.Name).HasColumnName("name");
        ent.Property(x => x.LoginProvider).HasColumnName("login_provider");
        ent.Property(x => x.Value).HasColumnName("value");
    }
}