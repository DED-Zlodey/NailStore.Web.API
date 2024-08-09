using NailStore.Core.Models;
using NailStore.Core.Models.Country;
using NailStore.Core.Models.GeoLocation;

namespace NailStore.Core.Interfaces;

public interface IGeoService
{
    /// <summary>
    /// Метод для асинхронной проверки существования страны в системе.
    /// </summary>
    /// <returns>
    /// Возвращает <c>true</c>, если страна существует, и <c>false</c> в противном случае.
    /// </returns>
    Task<bool> IsCountryExistAsync();
    /// <summary>
    /// Метод для асинхронного добавления начальных данных (страны) в систему.
    /// </summary>
    /// <param name="country">Объект страны, который необходимо добавить в систему.</param>
    /// <returns>
    /// Возвращает <c>true</c>, если начальные данные страны успешно добавлены в систему, и <c>false</c> в противном случае.
    /// </returns>
    Task<bool> InsertInitDataAsync(CountryDTO country);
    /// <summary>
    /// Метод для асинхронного получения списка городов из указанного региона.
    /// </summary>
    /// <param name="regionId">Идентификатор региона, для которого необходимо получить список городов.</param>
    /// <returns>
    /// Возвращает асинхронный результат в виде списка городов в виде объектов CityDTO.
    /// </returns>
    Task<List<CityDTO>> GetCitiesFromRegionId(int regionId);
    /// <summary>
    /// Метод для асинхронного добавления геолокаций в систему.
    /// </summary>
    /// <param name="geolocations">Список геолокаций, которые необходимо добавить в систему. Каждый элемент списка представлен объектом GeolocationDTO.</param>
    /// <returns>
    /// Возвращает объект ResponseModelCore, который содержит заголовок ответа с кодом статуса и сообщением об ошибке (если таковые имеются).
    /// Если все геолокации успешно добавлены, то код статуса в заголовке ответа будет равен 200, а сообщение об ошибке будет пустым.
    /// </returns>
    Task<ResponseModelCore<string>> AddGeolocationsAsync(List<GeolocationDTO> geolocations);
}