using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации для сущности описания услуги <see cref="ServiceDescriptionModel"/>
/// Настраивает сопоставление столбцов, имя таблицы, первичный ключ и имена столбцов в базе данных.
/// </summary>
public class ServiceDescriptionModelConfig : IEntityTypeConfiguration<ServiceDescriptionModel>
{
    /// <summary>
    /// Настраивает конструктор сущности для типа <see cref="ServiceDescriptionModel"/>
    /// </summary>
    /// <param name="ent">Конструктор сущности для типа ServiceDescriptionModel.</param>
    public void Configure(EntityTypeBuilder<ServiceDescriptionModel> ent)
    {
        // Устанавливает имя таблицы в базе данных
        ent.ToTable("descriptions_service")
            .HasKey(x => x.DescriptionId);

        // Настраивает первичный ключ для сущности
        ent.Property(x => x.DescriptionId).HasColumnName("description_id").UseIdentityColumn();

        // Настраивает сопоставление столбца для свойства ServiceId
        ent.Property(x => x.ServiceId).HasColumnName("service_id");

        // Настраивает сопоставление столбца для свойства Number
        ent.Property(x => x.Number).HasColumnName("number");

        // Настраивает сопоставление столбца для свойства Text
        ent.Property(x => x.Text).HasColumnName("text").HasMaxLength(500).IsRequired();

        // Создает индекс на столбце DescriptionId для повышения производительности запросов
        ent.HasIndex(x => x.DescriptionId);

        // Создает индекс на столбце ServiceId для повышения производительности запросов
        ent.HasIndex(x => x.ServiceId);
    }
}