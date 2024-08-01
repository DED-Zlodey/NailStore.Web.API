using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации для сущности утверждения пользователя в системе удостоверения личности <see cref="IdentityUserClaim"/>
/// Настраивает сопоставление столбцов, имя таблицы, первичный ключ и имена столбцов в базе данных.
/// </summary>
public class IdentityUserClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    /// <summary>
    /// Настраивает конструктор сущности для типа <see cref="IdentityUserClaim"/>
    /// </summary>
    /// <param name="ent">Конструктор сущности для типа IdentityUserClaim.</param>
    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> ent)
    {
        // Устанавливает имя таблицы в базе данных
        ent.ToTable(name: "user_claims");

        // Настраивает первичный ключ для сущности
        ent.HasKey(x => x.Id);

        // Настраивает сопоставление столбца для свойства Id
        ent.Property(x => x.Id).HasColumnName("claim_id");

        // Настраивает сопоставление столбца для свойства UserId
        ent.Property(x => x.UserId).HasColumnName("user_id").HasMaxLength(36).IsRequired();

        // Настраивает сопоставление столбца для свойства ClaimType
        ent.Property(x => x.ClaimType).HasColumnName("claim_type");

        // Настраивает сопоставление столбца для свойства ClaimValue
        ent.Property(x => x.ClaimValue).HasColumnName("claim_value");
    }
}