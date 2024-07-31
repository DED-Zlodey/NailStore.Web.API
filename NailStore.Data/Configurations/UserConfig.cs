using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

public class UserConfig : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> ent)
    {
        ent.ToTable(name: "users")
            .HasKey(x => x.Id);
        ent.Property(x => x.UserName).HasMaxLength(50).HasColumnName("user_name").IsRequired();
        ent.Property(x => x.Email).HasColumnName("email").IsRequired();
        ent.Property(x => x.Enable).HasColumnName("enable");
        ent.Property(x => x.AccessFailedCount).HasColumnName("access_failed_count");
        ent.Property(x => x.EmailConfirmed).HasColumnName("email_confirmed");
        ent.Property(x => x.ConcurrencyStamp).HasColumnName("concurrency_stamp");
        ent.Property(x => x.Id).HasColumnName("user_id");
        ent.Property(x => x.LockoutEnabled).HasColumnName("lockout_enabled");
        ent.Property(x => x.LockoutEnd).HasColumnName("lockout_end");
        ent.Property(x => x.NormalizedEmail).HasColumnName("normalized_email");
        ent.Property(x => x.NormalizedUserName).HasColumnName("normalized_user_name").HasMaxLength(50);
        ent.Property(x => x.PasswordHash).HasColumnName("password_hash");
        ent.Property(x => x.PhoneNumber).HasColumnName("phone_number").HasMaxLength(15);
        ent.Property(x => x.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
        ent.Property(x => x.RegisterAt).HasColumnName("register_at");
        ent.Property(x => x.SecurityStamp).HasColumnName("security_stamp");
        ent.Property(x => x.TwoFactorEnabled).HasColumnName("two_factor_enabled");
        ent.Property(x => x.FirstName).HasMaxLength(50).HasColumnName("first_name");
        ent.Property(x => x.LastName).HasMaxLength(50).HasColumnName("last_name");

        //Indexes
        ent.HasIndex(x => x.Id);
        ent.HasIndex(x => x.Email).IsUnique();
        ent.HasIndex(x => x.UserName).IsUnique();
    }
}