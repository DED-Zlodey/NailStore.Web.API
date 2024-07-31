using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NailStore.Data.Configurations;

public class IdentityUserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> ent)
    {
        ent.ToTable(name: "user_logins");
        ent.HasKey(x => x.UserId);
        ent.Property(x => x.UserId).HasColumnName("user_id");
        ent.Property(x => x.ProviderKey).HasColumnName("provider_key");
        ent.Property(x => x.ProviderDisplayName).HasColumnName("provider_display_name");
        ent.Property(x => x.LoginProvider).HasColumnName("login_provider");
    }
}