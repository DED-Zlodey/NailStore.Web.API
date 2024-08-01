using NetTopologySuite.Geometries;

namespace NailStore.Data.Models;

/// <summary>
/// Класс модели для города.
/// Представляет сущность города и содержит свойства, относящиеся к городу.
/// </summary>
public class City
{
    /// <summary>
    /// Уникальный идентификатор города.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор региона, к которому принадлежит город.
    /// </summary>
    public int RegionId { get; set; }

    /// <summary>
    /// Название города.
    /// </summary>
    public string NameCity { get; set; }

    /// <summary>
    /// Часовой пояс города.
    /// </summary>
    public string TimeZone { get; set; }

    /// <summary>
    /// Географические координаты города.
    /// </summary>
    public Point Coordinates { get; set; } 

    /// <summary>
    /// Регион, к которому принадлежит город.
    /// </summary>
    public Region Region { get; set; }

    /// <summary>
    /// Список услуг, предоставляемых в городе.
    /// </summary>
    public List<ServiceModel> Services { get; set; }
}