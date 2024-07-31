using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NailStore.Data.Models;

namespace NailStore.Data.Configurations;

public class CategoryServiceModelConfig : IEntityTypeConfiguration<CategoryServiceModel>
{
    public void Configure(EntityTypeBuilder<CategoryServiceModel> ent)
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
    }
}