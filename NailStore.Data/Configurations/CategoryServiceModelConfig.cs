using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

/// <summary>
/// Класс конфигурации для сущности CategoryServiceModel.
/// Определяет имя таблицы, первичный ключ, имена столбцов, типы данных и другие свойства сущности.
/// Также включает инициализацию данных для сущности.
/// </summary>
public class CategoryServiceModelConfig : IEntityTypeConfiguration<CategoryServiceModel>
{
    /// <summary>
    /// Конфигурирует сущность CategoryServiceModel для Entity Framework Core.
    /// Определяет имя таблицы, первичный ключ, имена столбцов, типы данных и другие свойства сущности.
    /// Также включает инициализацию данных для сущности.
    /// </summary>
    /// <param name="ent">Объект конфигурации для сущности CategoryServiceModel.</param>
    public void Configure(EntityTypeBuilder<CategoryServiceModel> ent)
    {
        // Определение имени таблицы и первичного ключа
        ent.ToTable("categories_services")
            .HasKey(x => x.CategoryId);

        // Определение столбца для CategoryId и установка его в качестве столбца с автоинкрементом
        ent.Property(x => x.CategoryId).HasColumnName("category_id").UseIdentityColumn();

        // Определение столбца для CategoryName и установка максимальной длины
        ent.Property(x => x.CategoryName).HasColumnName("category_name").HasMaxLength(50).IsRequired();

        // Определение столбца для Description и установка максимальной длины
        ent.Property(x => x.Description).HasColumnName("description").HasMaxLength(300).IsRequired(false);

        // Создание индекса на столбце CategoryId
        ent.HasIndex(x => x.CategoryId);

        // Инициализация данных для сущности
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
    }
}