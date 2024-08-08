using NetTopologySuite.Geometries;

namespace NailStore.Data.Models;

/// <summary>
/// Представляет местоположение с географическими координатами.
/// </summary>
public class GoogleLocation
{
    /// <summary>
    /// Уникальный идентификатор местоположения.
    /// </summary>
    public long LocationId { get; set; }

    /// <summary>
    /// Идентификатор региона, в котором находится местоположение.
    /// </summary>
    public int RegionId { get; set; }

    /// <summary>
    /// Почтовый индекс местоположения.
    /// </summary>
    public string Postcode { get; set; }

    /// <summary>
    /// Страна, в которой находится местоположение.
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Город, в котором находится местоположение.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Улица, на которой находится местоположение.
    /// </summary>
    public string Street { get; set; }

    /// <summary>
    /// Номер дома местоположения.
    /// </summary>
    public string House { get; set; }

    /// <summary>
    /// Полный адрес местоположения.
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Географические координаты местоположения.
    /// </summary>
    public Point Coordinates { get; set; } 
}