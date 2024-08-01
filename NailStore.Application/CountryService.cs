using Microsoft.Extensions.Logging;
using NailStore.Core.Interfaces;
using NailStore.Core.Models.Country;
using NailStore.Data.Models;

namespace NailStore.Application;

public class CountryService : ICountryService
{
    private readonly ILogger<CountryService> _logger;
    private readonly ICountryRepository _repository;

    public CountryService(ILogger<CountryService> logger, ICountryRepository repository)
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

    public async Task<bool> InsertInitDataAsync(CountryDTO country)
    {
        return await _repository.InsertInitDataAsync(country);
    }
}