using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации сущности токена пользователя в системе удостоверения личности <see cref="IdentityUserToken"/>
/// Настраивает сопоставление столбцов, имя таблицы, первичный ключ и имена столбцов в базе данных.
/// </summary>
public class IdentityUserTokenConfig : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    /// <summary>
    /// Настраивает конструктор сущности для типа <see cref="IdentityUserToken"/>
    /// </summary>
    /// <param name="ent">Конструктор сущности для типа IdentityUserToken.</param>
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> ent)
    {
        // Устанавливает имя таблицы в базе данных
        ent.ToTable(name: "user_tokens");

        // Настраивает первичный ключ для сущности
        ent.HasKey(x => x.UserId);

        // Настраивает сопоставление столбца для свойства UserId
        ent.Property(x => x.UserId).HasColumnName("user_id");

        // Настраивает сопоставление столбца для свойства Name
        ent.Property(x => x.Name).HasColumnName("name");

        // Настраивает сопоставление столбца для свойства LoginProvider
        ent.Property(x => x.LoginProvider).HasColumnName("login_provider");

        // Настраивает сопоставление столбца для свойства Value
        ent.Property(x => x.Value).HasColumnName("value");
    }
}