using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации для сущности <see cref="IdentityRoleClaim"/>.
/// Настраивает сопоставление столбцов, имя таблицы, первичный ключ и имена столбцов в базе данных.
/// </summary>
public class IdentityRoleClaimConfig : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    /// <summary>
    /// Настраивает конструктор сущности для типа <see cref="IdentityRoleClaim"/>.
    /// </summary>
    /// <param name="ent">Конструктор сущности для типа <c>IdentityRoleClaim</c>.</param>
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> ent)
    {
        // Устанавливает имя таблицы в базе данных
        ent.ToTable(name: "role_claims");

        // Настраивает первичный ключ для сущности
        ent.HasKey(x => x.Id);

        // Настраивает сопоставление столбца для свойства Id
        ent.Property(x => x.Id).HasColumnName("claim_id");

        // Настраивает сопоставление столбца для свойства RoleId
        ent.Property(x => x.RoleId).HasColumnName("role_id");

        // Настраивает сопоставление столбца для свойства ClaimType
        ent.Property(x => x.ClaimType).HasColumnName("claim_type");

        // Настраивает сопоставление столбца для свойства ClaimValue
        ent.Property(x => x.ClaimValue).HasColumnName("claim_value");
    }
}