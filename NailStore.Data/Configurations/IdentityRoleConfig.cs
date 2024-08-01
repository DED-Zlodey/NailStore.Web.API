using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации для сущности роли в системе удостоверения личности <see cref="IdentityRole"/>
/// Настраивает сопоставление столбцов, имя таблицы, первичный ключ и имена столбцов в базе данных.
/// </summary>
public class IdentityRoleConfig : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    /// <summary>
    /// Настраивает конструктор сущности для типа <see cref="IdentityRole"/>
    /// </summary>
    /// <param name="ent">Конструктор сущности для типа IdentityRole.</param>
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> ent)
    {
        // Устанавливает имя таблицы в базе данных
        ent.ToTable(name: "roles");

        // Настраивает первичный ключ для сущности
        ent.HasKey(x => x.Id);

        // Настраивает сопоставление столбца для свойства Name
        ent.Property(x => x.Name).HasMaxLength(20).HasColumnName("name");

        // Настраивает сопоставление столбца для свойства Id
        ent.Property(x => x.Id).HasColumnName("role_id");

        // Настраивает сопоставление столбца для свойства NormalizedName
        ent.Property(x => x.NormalizedName).HasMaxLength(20).HasColumnName("normalized_name");

        // Настраивает сопоставление столбца для свойства ConcurrencyStamp
        ent.Property(x => x.ConcurrencyStamp).HasColumnName("concurrency_stamp");
    }
}