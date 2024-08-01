namespace NailStore.Data.Models;

/// <summary>
/// Модель страны.
/// Представляет сущность страны и содержит свойства, относящиеся к стране.
/// </summary>
public class Country
{
    /// <summary>
    /// Уникальный идентификатор страны.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название страны.
    /// Свойство обязательно для заполнения при создании нового экземпляра класса.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Список регионов, принадлежащих стране.
    /// </summary>
    public List<Region> Regions { get; set; }
}