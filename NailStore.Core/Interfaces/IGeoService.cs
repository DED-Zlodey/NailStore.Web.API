using NailStore.Core.Models.Country;

namespace NailStore.Core.Interfaces;

public interface ICountryService
{
    /// <summary>
    /// Метод для асинхронной проверки существования страны в системе.
    /// </summary>
    /// <returns>
    /// Возвращает <c>true</c>, если страна существует, и <c>false</c> в противном случае.
    /// </returns>
    Task<bool> IsCountryExistAsync();

    Task<bool> InsertInitDataAsync(CountryDTO country);
}