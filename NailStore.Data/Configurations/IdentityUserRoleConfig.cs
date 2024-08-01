using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации для сущности связи пользователя и роли в системе удостоверения личности <see cref="IdentityUserRole"/>
/// Настраивает сопоставление столбцов, имя таблицы, первичный ключ и имена столбцов в базе данных.
/// </summary>
public class IdentityUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    /// <summary>
    /// Настраивает конструктор сущности для типа <see cref="IdentityUserRole"/>
    /// </summary>
    /// <param name="ent">Конструктор сущности для типа IdentityUserRole.</param>
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> ent)
    {
        // Настраивает первичный ключ для сущности, как составной ключ из UserId и RoleId
        ent.HasKey(x => new { x.UserId, x.RoleId });

        // Устанавливает имя таблицы в базе данных
        ent.ToTable(name: "user_roles");

        // Настраивает сопоставление столбца для свойства UserId
        ent.Property(x => x.UserId).HasColumnName("user_id");

        // Настраивает сопоставление столбца для свойства RoleId
        ent.Property(x => x.RoleId).HasColumnName("role_id");

        // Создает индекс на составном ключе UserId и RoleId для повышения производительности запросов
        ent.HasIndex(x => new { x.UserId, x.RoleId });
    }
}