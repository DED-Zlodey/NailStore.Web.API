using Microsoft.Extensions.Logging;
using NailStore.Core.Interfaces;
using NailStore.Core.Models;
using NailStore.Core.Models.Country;
using NailStore.Core.Models.GeoLocation;

namespace NailStore.Application;

public class GeoService : IGeoService
{
    private readonly ILogger<GeoService> _logger;
    private readonly IGeoRepository _repository;

    public GeoService(ILogger<GeoService> logger, IGeoRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    /// <summary>
    /// Метод для асинхронной проверки существования страны в системе.
    /// </summary>
    /// <returns>
    /// Возвращает <c>true</c>, если страна существует, и <c>false</c> в противном случае.
    /// </returns>
    public async Task<bool> IsCountryExistAsync()
    {
        // Вызов асинхронного метода из репозитория для проверки существования страны
        // Ключевое слово await используется для ожидания завершения асинхронной операции
        // Результат асинхронной операции возвращается из метода
        return await _repository.IsCountryExistAsync();
    }

    /// <summary>
    /// Метод для асинхронного добавления начальных данных (страны) в систему.
    /// </summary>
    /// <param name="country">Объект страны, который необходимо добавить в систему.</param>
    /// <returns>
    /// Возвращает <c>true</c>, если начальные данные страны успешно добавлены в систему, и <c>false</c> в противном случае.
    /// </returns>
    public async Task<bool> InsertInitDataAsync(CountryDTO country)
    {
        // Вызов асинхронного метода из репозитория для добавления начальных данных страны
        return await _repository.InsertInitDataAsync(country);
    }

    /// <summary>
    /// Метод для асинхронного получения списка городов из указанного региона.
    /// </summary>
    /// <param name="regionId">Идентификатор региона, для которого необходимо получить список городов.</param>
    /// <returns>
    /// Возвращает асинхронный результат в виде списка городов в виде объектов CityDTO.
    /// </returns>
    public async Task<List<CityDTO>> GetCitiesFromRegionId(int regionId)
    {
        // Вызов асинхронного метода из репозитория для получения списка городов из указанного региона
        return await _repository.GetCitiesFromRegionId(regionId);
    }

    /// <summary>
    /// Метод для асинхронного добавления геолокаций в систему.
    /// </summary>
    /// <param name="geolocations">Список геолокаций, которые необходимо добавить в систему. Каждый элемент списка представлен объектом GeolocationDTO.</param>
    /// <returns>
    /// Возвращает объект ResponseModelCore, который содержит заголовок ответа с кодом статуса и сообщением об ошибке (если таковые имеются).
    /// Если все геолокации успешно добавлены, то код статуса в заголовке ответа будет равен 200, а сообщение об ошибке будет пустым.
    /// </returns>
    public async Task<ResponseModelCore<string>> AddGeolocationsAsync(List<GeolocationDTO> geolocations)
    {
        var response = new ResponseModelCore<string>
        {
            Header = new()
            {
                StatusCode = 400,
                Error = "Что-то пошло не так:("
            }
        };
        foreach (var geolocation in geolocations)
        {
            if (string.IsNullOrEmpty(geolocation.Address))
            {
                _logger.LogError("{method} Отсутствует адрес для геолокации", nameof(AddGeolocationsAsync));
                response.Header.Error = "Адрес не может быть пустым";
                return response;
            }
        }

        var result = await _repository.AddGeolocationsAsync(geolocations);
        response = result;

        return response;
    }
}