using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NailStore.Data.Models;

namespace NailStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserEntity>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityUserRole<string>>(ent =>
            {
                ent.HasKey(x => new { x.UserId, x.RoleId });
                ent.ToTable(name: "user_roles");
                ent.Property(x => x.UserId).HasColumnName("user_id");
                ent.Property(x => x.RoleId).HasColumnName("role_id");
            });
            builder.Entity<UserEntity>(ent =>
            {
                ent.ToTable(name: "users");
                ent.HasKey(x => x.Id);
                ent.HasIndex(x => x.Email).IsUnique();
                ent.HasIndex(x => x.UserName).IsUnique();
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
                ent.Property(x => x.NormalizedUserName).HasColumnName("normalized_user_name");
                ent.Property(x => x.PasswordHash).HasColumnName("password_hash");
                ent.Property(x => x.PhoneNumber).HasColumnName("phone_number");
                ent.Property(x => x.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
                ent.Property(x => x.RegisterAt).HasColumnName("register_at");
                ent.Property(x => x.SecurityStamp).HasColumnName("security_stamp");
                ent.Property(x => x.TwoFactorEnabled).HasColumnName("two_factor_enabled");
                ent.Property(x => x.FirstName).HasMaxLength(50).HasColumnName("first_name");
                ent.Property(x => x.LastName).HasMaxLength(50).HasColumnName("last_name");
            });
            builder.Entity<IdentityRole>(ent =>
            {
                ent.ToTable(name: "roles");
                ent.HasKey(x => x.Id);
                ent.Property(x => x.Name).HasMaxLength(20).HasColumnName("name");
                ent.Property(x => x.Id).HasColumnName("role_id");
                ent.Property(x => x.NormalizedName).HasColumnName("normalized_name");
                ent.Property(x => x.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            });
            builder.Entity<IdentityUserClaim<string>>(ent =>
            {
                ent.ToTable(name: "user_claims");
                ent.HasKey(x => x.Id);
                ent.Property(x => x.Id).HasColumnName("claim_id");
                ent.Property(x => x.UserId).HasColumnName("user_id");
                ent.Property(x => x.ClaimType).HasColumnName("claim_type");
                ent.Property(x => x.ClaimValue).HasColumnName("claim_value");
            });
            builder.Entity<IdentityUserLogin<string>>(ent =>
            {
                ent.ToTable(name: "user_logins");
                ent.HasKey(x => x.UserId);
                ent.Property(x => x.UserId).HasColumnName("user_id");
                ent.Property(x => x.ProviderKey).HasColumnName("provider_key");
                ent.Property(x => x.ProviderDisplayName).HasColumnName("provider_display_name");
                ent.Property(x => x.LoginProvider).HasColumnName("login_provider");
            });
            builder.Entity<IdentityRoleClaim<string>>(ent =>
            {
                ent.ToTable(name: "role_claims");
                ent.HasKey(x => x.Id);
                ent.Property(x => x.Id).HasColumnName("claim_id");
                ent.Property(x => x.RoleId).HasColumnName("role_id");
                ent.Property(x => x.ClaimType).HasColumnName("claim_type");
                ent.Property(x => x.ClaimValue).HasColumnName("claim_value");
            });
            builder.Entity<IdentityUserToken<string>>(ent =>
            {
                ent.ToTable(name: "user_tokens");
                ent.HasKey(x => x.UserId);
                ent.Property(x => x.UserId).HasColumnName("user_id");
                ent.Property(x => x.Name).HasColumnName("name");
                ent.Property(x => x.LoginProvider).HasColumnName("login_provider");
                ent.Property(x => x.Value).HasColumnName("value");
            });
        }
    }
}
