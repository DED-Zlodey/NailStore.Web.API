using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NailStore.Core.Interfaces;
using NailStore.Core.Models.Country;
using NailStore.Data;
using NailStore.Data.Models;
using NailStore.Repositories.Mappers;

namespace NailStore.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly ILogger<CountryRepository> _logger;
    private readonly ApplicationDbContext _context;

    public CountryRepository(ILogger<CountryRepository> logger, ApplicationDbContext context)
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

    private Country CountryMapToDTO(CountryDTO country)
    {
        var resCountry = CountryMapper.MapTo(country);
        foreach (var region in country.Regions)
        {
            var reg = CountryMapper.MapTo(region);
            foreach (var city in region.Cities)
            {
                var mapCity = CountryMapper.MapTo(city);
                reg.Cities.Add(mapCity);
            }

            resCountry.Regions.Add(reg);
        }

        return resCountry;
    }
}