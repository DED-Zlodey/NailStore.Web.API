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
    public int CountryId { get; set; }

    /// <summary>
    /// Название страны.
    /// Свойство обязательно для заполнения при создании нового экземпляра класса.
    /// </summary>
    public required string CountryName { get; set; }

    /// <summary>
    /// Список регионов, принадлежащих стране.
    /// </summary>
    public List<Region> Regions { get; set; }
}