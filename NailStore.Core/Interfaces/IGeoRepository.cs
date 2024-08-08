﻿using NailStore.Core.Models;
using NailStore.Core.Models.Country;
using NailStore.Core.Models.GeoLocation;

namespace NailStore.Core.Interfaces;

public interface ICountryRepository
{
    /// <summary>
    /// Метод для асинхронной проверки существования страны в базе данных.
    /// </summary>
    /// <returns>
    /// Возвращает <c>true</c>, если страна существует, и <c>false</c> в противном случае.
    /// </returns>
    Task<bool> IsCountryExistAsync();

    Task<bool> InsertInitDataAsync(CountryDTO country);
}