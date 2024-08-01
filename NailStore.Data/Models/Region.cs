namespace NailStore.Data.Models;

/// <summary>
/// Модель региона.
/// Представляет сущность региона и содержит свойства, относящиеся к региону.
/// </summary>
public class Region
{
    /// <summary>
    /// Уникальный идентификатор региона.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор страны, к которой принадлежит регион.
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// Название региона.
    /// Свойство обязательно для заполнения при создании нового экземпляра класса.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Список городов, принадлежащих региону.
    /// </summary>
    public List<City> Cities { get; set; }

    /// <summary>
    /// Страна, к которой принадлежит регион.
    /// Это свойство может быть null, поскольку регион может не принадлежать ни одной стране.
    /// </summary>
    //public Country Country { get; set; }
}