using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации для сущности входа пользователя в системе удостоверения личности <see cref="IdentityUserLogin"/>
/// Настраивает сопоставление столбцов, имя таблицы, первичный ключ и имена столбцов в базе данных.
/// </summary>
public class IdentityUserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    /// <summary>
    /// Настраивает конструктор сущности для типа <see cref="IdentityUserLogin"/>
    /// </summary>
    /// <param name="ent">Конструктор сущности для типа IdentityUserLogin.</param>
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> ent)
    {
        // Устанавливает имя таблицы в базе данных
        ent.ToTable(name: "user_logins");

        // Настраивает первичный ключ для сущности
        ent.HasKey(x => x.UserId);

        // Настраивает сопоставление столбца для свойства UserId
        ent.Property(x => x.UserId).HasColumnName("user_id");

        // Настраивает сопоставление столбца для свойства ProviderKey
        ent.Property(x => x.ProviderKey).HasColumnName("provider_key");

        // Настраивает сопоставление столбца для свойства ProviderDisplayName
        ent.Property(x => x.ProviderDisplayName).HasColumnName("provider_display_name");

        // Настраивает сопоставление столбца для свойства LoginProvider
        ent.Property(x => x.LoginProvider).HasColumnName("login_provider");
    }
}