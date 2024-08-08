using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NailStore.Core.Interfaces;
using NailStore.Core.Models;
using NailStore.Core.Models.Country;
using NailStore.Core.Models.GeoLocation;
using NailStore.Data;
using NailStore.Data.Models;
using NailStore.Repositories.Mappers;
using NetTopologySuite.Geometries;

namespace NailStore.Repositories;

public class GeoRepository : IGeoRepository
{
    private readonly ILogger<GeoRepository> _logger;
    private readonly ApplicationDbContext _context;

    public GeoRepository(ILogger<GeoRepository> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Метод для асинхронной проверки существования страны в базе данных.
    /// </summary>
    /// <returns>
    /// Возвращает <c>true</c>, если страна существует, и <c>false</c> в противном случае.
    /// </returns>
    public async Task<bool> IsCountryExistAsync()
    {
        // Асинхронное получение одной страны из базы данных
        var country = await _context.Countries.AsNoTracking().FirstOrDefaultAsync();

        // Проверка, существует ли страна
        if (country == null)
        {
            // Если страна не существует, возвращается false
            return false;
        }

        // Если страна существует, возвращается true
        return true;
    }

    /// <summary>
    /// Метод для асинхронного добавления начальных данных (страны) в базу данных.
    /// </summary>
    /// <param name="country">Объект страны, который необходимо добавить в базу.</param>
    /// <returns>
    /// Возвращает <c>true</c>, если страна успешно добавлена в базу, и <c>false</c> в противном случае.
    /// </returns>
    public async Task<bool> InsertInitDataAsync(CountryDTO country)
    {
        try
        {
            var countryData = CountryMapToDTO(country);
            _context.Countries.Add(countryData);

            // Асинхронное сохранение изменений в базе данных
            await _context.SaveChangesAsync();

            // Запись в журнал информации о добавлении страны в базу
            _logger.LogInformation("{method} Страна добавлена в базу", nameof(InsertInitDataAsync));

            // Возврат true, если страна успешно добавлена в базу
            return true;
        }
        catch (Exception ex)
        {
            // Запись в журнал ошибок о возникшей проблеме при добавлении страны в базу
            _logger.LogError(ex, "{method} Ошибка при добавлении страны в базу", nameof(InsertInitDataAsync));

            // Возврат false, если при добавлении страны в базу возникла ошибка
            return false;
        }
    }

    /// <summary>
    /// Метод для асинхронного получения списка городов из указанного региона.
    /// </summary>
    /// <param name="regionId">Идентификатор региона, для которого необходимо получить список городов.</param>
    /// <returns>
    /// Возвращает список городов в виде объектов CityDTO, связанных с указанным регионом.
    /// </returns>
    public async Task<List<CityDTO>> GetCitiesFromRegionId(int regionId)
    {
        // Асинхронное выполнение запроса LINQ для получения городов из указанного региона
        var cities = await _context.Cities.Where(x => x.RegionId == regionId).Select(x => new CityDTO
        {
            RegionId = x.RegionId,
            Id = x.Id,
            Latitude = x.Coordinates.Coordinate.X,
            Longitude = x.Coordinates.Coordinate.Y,
            NameCity = x.NameCity,
            TimeZone = x.TimeZone,
        }).ToListAsync();

        // Возврат списка городов
        return cities;
    }

    /// <summary>
    /// Метод для асинхронного добавления списка геолокаций в базу данных.
    /// </summary>
    /// <param name="geolocations">Список геолокаций, которые необходимо добавить в базу. Каждая геолокация представлена объектом GeolocationDTO.</param>
    /// <returns>
    /// Возвращает строку с сообщением "Локации успешно добавлены", если все геолокации успешно добавлены в базу.
    /// </returns>
    public async Task<ResponseModelCore> AddGeolocationsAsync(List<GeolocationDTO> geolocations)
    {
        var response = new ResponseModelCore
        {
            Header = new()
            {
                StatusCode = 200,
                Error = string.Empty,
            },
            Body = new ()
            {
                Message = "Локации успешно добавлены",
            }
        };
        foreach (var item in geolocations)
        {
            _context.GoogleLocations.Add(new()
            {
                RegionId = item.RegionId,
                Address = item.Address,
                Coordinates = new Point(item.Lat, item.Lon),
                City = item.City,
                Country = item.Country,
                House = item.House,
                Postcode = item.Postcode,
                Street = item.Street,
            });
        }
        await _context.SaveChangesAsync();
        return response;
    }

    /// <summary>
    /// Метод для проецирования из DTO в модель данных.
    /// Он также включает в себя проецирование моделей регионов и городов.
    /// </summary>
    /// <param name="country">Объект страны в формате DTO.</param>
    /// <returns>
    /// Возвращает объект страны в модели данных, включая регионы и города.
    /// </returns>
    private Country CountryMapToDTO(CountryDTO country)
    {
        // Создание нового объекта страны в модели данных
        var resCountry = CountryMapper.MapTo(country);

        // Перебор всех регионов в объекте страны DTO
        foreach (var region in country.Regions)
        {
            // Создание нового объекта региона в модели данных
            var reg = CountryMapper.MapTo(region);

            // Перебор всех городов в текущем регионе
            foreach (var city in region.Cities)
            {
                // Создание нового объекта города в модели данных
                var mapCity = CountryMapper.MapTo(city);

                // Добавление объекта города в коллекцию городов текущего региона
                reg.Cities.Add(mapCity);
            }

            // Добавление объекта региона в коллекцию регионов объекта страны в модели данных
            resCountry.Regions.Add(reg);
        }

        // Возврат объекта страны в модели данных, включая регионы и города
        return resCountry;
    }
}