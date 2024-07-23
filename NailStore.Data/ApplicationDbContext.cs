using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NailStore.Data.Models;

namespace NailStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
    {

        public DbSet<CategoryServiceModel> CategoriesServices { get; set; }
        public DbSet<ServiceModel> Services { get; set; }
        public DbSet<ServiceDescriptionModel> DescriptionsService { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityUserRole<Guid>>(ent =>
            {
                ent.HasKey(x => new { x.UserId, x.RoleId });
                ent.ToTable(name: "user_roles");
                ent.Property(x => x.UserId).HasColumnName("user_id");
                ent.Property(x => x.RoleId).HasColumnName("role_id");
                ent.HasIndex(x => new { x.UserId, x.RoleId });
            });
            builder.Entity<UserEntity>(ent =>
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

                ent.HasMany(x => x.Services).WithOne().HasForeignKey(x => x.ServiceId);
            });
            builder.Entity<IdentityRole<Guid>>(ent =>
            {
                ent.ToTable(name: "roles");
                ent.HasKey(x => x.Id);
                ent.Property(x => x.Name).HasMaxLength(20).HasColumnName("name");
                ent.Property(x => x.Id).HasColumnName("role_id");
                ent.Property(x => x.NormalizedName).HasMaxLength(20).HasColumnName("normalized_name");
                ent.Property(x => x.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            });
            builder.Entity<IdentityUserClaim<Guid>>(ent =>
            {
                ent.ToTable(name: "user_claims");
                ent.HasKey(x => x.Id);
                ent.Property(x => x.Id).HasColumnName("claim_id");
                ent.Property(x => x.UserId).HasColumnName("user_id").HasMaxLength(36).IsRequired();
                ent.Property(x => x.ClaimType).HasColumnName("claim_type");
                ent.Property(x => x.ClaimValue).HasColumnName("claim_value");
            });
            builder.Entity<IdentityUserLogin<Guid>>(ent =>
            {
                ent.ToTable(name: "user_logins");
                ent.HasKey(x => x.UserId);
                ent.Property(x => x.UserId).HasColumnName("user_id");
                ent.Property(x => x.ProviderKey).HasColumnName("provider_key");
                ent.Property(x => x.ProviderDisplayName).HasColumnName("provider_display_name");
                ent.Property(x => x.LoginProvider).HasColumnName("login_provider");
            });
            builder.Entity<IdentityRoleClaim<Guid>>(ent =>
            {
                ent.ToTable(name: "role_claims");
                ent.HasKey(x => x.Id);
                ent.Property(x => x.Id).HasColumnName("claim_id");
                ent.Property(x => x.RoleId).HasColumnName("role_id");
                ent.Property(x => x.ClaimType).HasColumnName("claim_type");
                ent.Property(x => x.ClaimValue).HasColumnName("claim_value");
            });
            builder.Entity<IdentityUserToken<Guid>>(ent =>
            {
                ent.ToTable(name: "user_tokens");
                ent.HasKey(x => x.UserId);
                ent.Property(x => x.UserId).HasColumnName("user_id");
                ent.Property(x => x.Name).HasColumnName("name");
                ent.Property(x => x.LoginProvider).HasColumnName("login_provider");
                ent.Property(x => x.Value).HasColumnName("value");
            });

            builder.Entity<CategoryServiceModel>(ent =>
            {
                ent.ToTable("categories_services")
                    .HasKey(x => x.CategoryId);
                ent.Property(x => x.CategoryId).HasColumnName("category_id").UseIdentityColumn();
                ent.Property(x => x.CategoryName).HasColumnName("category_name").HasMaxLength(50);
                ent.Property(x => x.Description).HasColumnName("description").HasMaxLength(300);

                // Indexes
                ent.HasIndex(x => x.CategoryId);

                //InitData
                ent.HasData(new CategoryServiceModel
                {
                    CategoryId = 1,
                    CategoryName = "Комплекс",
                    Description = "Комплексные процедуры",
                });
                ent.HasData(new CategoryServiceModel
                {
                    CategoryId = 2,
                    CategoryName = "Маникюр",
                    Description = "Уход за ногтями на руках",
                });
                ent.HasData(new CategoryServiceModel
                {
                    CategoryId = 3,
                    CategoryName = "Педикюр",
                    Description = "Уход за ногтями на ногах",
                });
            });
            builder.Entity<ServiceModel>(ent =>
            {
                ent.ToTable("services")
                    .HasKey(x => x.ServiceId);
                ent.Property(x => x.ServiceId).HasColumnName("service_id").UseIdentityColumn();
                ent.Property(x => x.CategoryId).HasColumnName("category_id");
                ent.Property(x => x.ServiceName).HasColumnName("service_name").HasMaxLength(70);
                ent.Property(x => x.UserId).HasColumnName("user_id");
                ent.Property(x => x.DurationTime).HasColumnName("duration");
                ent.Property(x => x.Price).HasColumnName("price");
                ent.Property(x => x.City).HasColumnName("city");
                ent.HasMany(x => x.ServiceDescriptions).WithOne(x => x.Service).HasForeignKey(x => x.ServiceId);
                ent.HasOne(x => x.User).WithMany(x => x.Services).HasForeignKey(x => x.UserId);
                ent.HasOne(x => x.Category).WithMany(x => x.Services).HasForeignKey(x => x.CategoryId);

                // Indexes
                ent.HasIndex(x => x.ServiceId);
                ent.HasIndex(x => x.UserId);
                ent.HasIndex(x => x.CategoryId);
            });
            builder.Entity<ServiceDescriptionModel>(ent =>
            {
                ent.ToTable("descriptions_service")
                    .HasKey(x => x.DescriptionId);
                ent.Property(x => x.DescriptionId).HasColumnName("description_id").UseIdentityColumn();
                ent.Property(x => x.ServiceId).HasColumnName("service_id");
                ent.Property(x => x.Number).HasColumnName("number");
                ent.Property(x => x.Text).HasColumnName("text").HasMaxLength(500).IsRequired();

                ent.HasIndex(x => x.DescriptionId);
                ent.HasIndex(x => x.ServiceId);
            });
        }
    }
}
