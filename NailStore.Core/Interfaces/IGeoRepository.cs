using NailStore.Core.Models;
using NailStore.Core.Models.Country;
using NailStore.Core.Models.GeoLocation;

namespace NailStore.Core.Interfaces;

public interface IGeoRepository
{
    /// <summary>
    /// Метод для асинхронной проверки существования страны в базе данных.
    /// </summary>
    /// <returns>
    /// Возвращает <c>true</c>, если страна существует, и <c>false</c> в противном случае.
    /// </returns>
    Task<bool> IsCountryExistAsync();
    /// <summary>
    /// Метод для асинхронного добавления начальных данных (страны) в базу данных.
    /// </summary>
    /// <param name="country">Объект страны, который необходимо добавить в базу.</param>
    /// <returns>
    /// Возвращает <c>true</c>, если страна успешно добавлена в базу, и <c>false</c> в противном случае.
    /// </returns>
    Task<bool> InsertInitDataAsync(CountryDTO country);
    /// <summary>
    /// Метод для асинхронного получения списка городов из указанного региона.
    /// </summary>
    /// <param name="regionId">Идентификатор региона, для которого необходимо получить список городов.</param>
    /// <returns>
    /// Возвращает список городов в виде объектов CityDTO, связанных с указанным регионом.
    /// </returns>
    Task<List<CityDTO>> GetCitiesFromRegionId(int regionId);
    /// <summary>
    /// Метод для асинхронного добавления списка геолокаций в базу данных.
    /// </summary>
    /// <param name="geolocations">Список геолокаций, которые необходимо добавить в базу. Каждая геолокация представлена объектом GeolocationDTO.</param>
    /// <returns>
    /// Возвращает строку с сообщением "Локации успешно добавлены", если все геолокации успешно добавлены в базу.
    /// </returns>
    Task<ResponseModelCore<string>> AddGeolocationsAsync(List<GeolocationDTO> geolocations);
}