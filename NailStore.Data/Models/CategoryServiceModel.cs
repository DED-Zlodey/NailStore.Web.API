namespace NailStore.Data.Models;

public class CategoryServiceModel
{
    /// <summary>
    /// Идентификатор категории
    /// </summary>
    public int CategoryId { get; set; }
    /// <summary>
    /// Название категории
    /// </summary>
    public string CategoryName { get; set; }
    /// <summary>
    /// Краткое описание категории
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Навигационное свойство списка сервисов, которые принадлежат категории
    /// </summary>
    public List<ServiceModel> Services { get; set; }
}
