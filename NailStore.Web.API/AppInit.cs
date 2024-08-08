using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NailStore.Application.Settings;
using NailStore.Data.Models;
using System.Text;
using System.Text.Json;
using NailStore.Core.Interfaces;
using NailStore.Core.Models.Country;

namespace NailStore.Web.API;

/// <summary>
/// Структура для инициализации БД начальными данными.
/// </summary>
public struct AppInit
{
    private readonly ILogger<AppInit> _logger;
    private readonly SrvSettings _srvSettings;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IGeoService _geoService;

    /// <summary>
    /// Конструктор для инициализации структуры AppInit.
    /// Инициализирует необходимые зависимости и конфигурационные параметры.
    /// </summary>
    /// <param name="service">Поставщик услуг приложения для доступа к сервисам приложения.</param>
    public AppInit(IServiceProvider service)
    {
        _logger = service.GetRequiredService<ILogger<AppInit>>();
        _srvSettings = service.GetRequiredService<IOptions<SrvSettings>>().Value;
        _roleManager = service.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        _userManager = service.GetRequiredService<UserManager<UserEntity>>();
        _geoService = service.GetRequiredService<IGeoService>();
    }

    /// <summary>
    /// Инициализация БД начальными данными
    /// </summary>
    public async Task InitializeAsync()
    {
        if (ValidationSectionAdmin(_srvSettings))
        {
            if (await _roleManager.FindByNameAsync("Admin") == null)
            {
                var res = await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
                if (res.Succeeded)
                {
                    _logger.LogInformation("{InitializeAsync}: Создана роль администратора: \"Admin\"",
                        nameof(InitializeAsync));
                }
                else
                {
                    _logger.LogError("{InitializeAsync}: Не удалось создать роль администратора. Reason: {errorString}",
                        nameof(InitializeAsync), GetIdentityErrorString(res.Errors));
                }
            }

            if (await _roleManager.FindByNameAsync("User") == null)
            {
                var res = await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));
                if (res.Succeeded)
                {
                    _logger.LogInformation("{InitializeAsync}: Создана роль пользователя: \"User\"",
                        nameof(InitializeAsync));
                }
                else
                {
                    _logger.LogError("{InitializeAsync}: Не удалось создать роль пользователя. Reason: {errorString}",
                        nameof(InitializeAsync), GetIdentityErrorString(res.Errors));
                }
            }

            if (await _roleManager.FindByNameAsync("Master") == null)
            {
                var res = await _roleManager.CreateAsync(new IdentityRole<Guid>("Master"));
                if (res.Succeeded)
                {
                    _logger.LogInformation("{InitializeAsync}: Создана роль пользователя: \"Master\"",
                        nameof(InitializeAsync));
                }
                else
                {
                    _logger.LogError("{InitializeAsync}: Не удалось создать роль пользователя. Reason: {errorString}",
                        nameof(InitializeAsync), GetIdentityErrorString(res.Errors));
                }
            }

            if (await _userManager.FindByEmailAsync(_srvSettings.Admin!.Email) == null)
            {
                var user = new UserEntity
                {
                    UserName = _srvSettings.Admin.UserName,
                    Email = _srvSettings.Admin.Email,
                    EmailConfirmed = true,
                    RegisterAt = DateTime.Now.ToUniversalTime(),
                    PhoneNumberConfirmed = true,
                    Enable = true,
                };
                IdentityResult result = await _userManager.CreateAsync(user, _srvSettings.Admin.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("{InitializeAsync}: Создан пользователь: {Email}", nameof(InitializeAsync),
                        _srvSettings.Admin!.Email);
                    var res = await _userManager.AddToRoleAsync(user, "Admin");
                    if (res.Succeeded)
                    {
                        _logger.LogInformation("{InitializeAsync}: Роль: \"Admin\" присвоена пользователю: {Email}",
                            nameof(InitializeAsync), _srvSettings.Admin!.Email);
                    }
                    else
                    {
                        _logger.LogError(
                            "{InitializeAsync}: Не удалось присвоить роль: \"Admin\", пользовтаелю: {Email}. Reason: {errorString}",
                            nameof(InitializeAsync), _srvSettings.Admin!.Email, GetIdentityErrorString(result.Errors));
                    }
                }
                else
                {
                    _logger.LogError(
                        "{InitializeAsync}: Не удалось создать пользователя: {Email}. Reason: {errorString}",
                        nameof(InitializeAsync), _srvSettings.Admin!.Email, GetIdentityErrorString(result.Errors));
                }
            }
        }
        else
        {
            _logger.LogError("{InitializeAsync}: Ошибки в конфигурационном файле, в секции {Admin}",
                nameof(InitializeAsync), _srvSettings.Admin);
        }

        if (!await _geoService.IsCountryExistAsync())
        {
            var jsonCountry =
                await GetStringJsonFromJSONFileAsync($"{AppDomain.CurrentDomain.BaseDirectory}Data\\Cities.json");
            if (!string.IsNullOrEmpty(jsonCountry))
            {
                var country = GetObjectFromJSON<CountryDTO>(jsonCountry);
                var res = await _geoService.InsertInitDataAsync(country);
            }   
        }
        else
        {
            _logger.LogInformation("{mathod}: Страна уже существует в БД", nameof(InitializeAsync));
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Валидация секции Admin из файла конфигурации
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    private bool ValidationSectionAdmin(SrvSettings settings)
    {
        if (settings == null)
            return false;
        if (settings.Admin == null)
            return false;
        if (string.IsNullOrEmpty(settings.Admin.Password))
            return false;
        if (string.IsNullOrEmpty(settings.Admin.Email))
            return false;
        return true;
    }

    /// <summary>
    /// Метод для получения всех ошибок Identity в виде одной строки.
    /// </summary>
    /// <param name="errors">Коллекция ошибок Identity.</param>
    /// <returns>Возвращает все ошибки в виде одной строки.</returns>
    private string GetIdentityErrorString(IEnumerable<IdentityError> errors)
    {
        // Инициализация StringBuilder для создания финальной строки
        StringBuilder sb = new StringBuilder();

        // Счетчик для отслеживания текущей итерации
        int counter = 0;

        // Попытка привести входные ошибки к массиву IdentityError.
        // Если это не удается (т.к. ошибки уже являются массивом),
        // используется метод ToArray() для преобразования в массив.
        var identityErrors = errors as IdentityError[] ?? errors.ToArray();

        // Перебор каждой ошибки в массиве
        foreach (var error in identityErrors)
        {
            // Проверка, что описание ошибки не пустое
            if (!string.IsNullOrEmpty(error.Description))
            {
                // Если это не последняя итерация, добавляется описание ошибки с запятой и пробелом
                // Если это последняя итерация, добавляется только описание ошибки
                if (counter != identityErrors.Count() - 1)
                {
                    sb.Append($"{error.Description}, ");
                }
                else
                {
                    sb.Append($"{error.Description}");
                }
            }

            // Увеличение счетчика на единицу
            counter++;
        }

        // Возвращение финальной строки с описаниями ошибок
        return sb.ToString();
    }

    /// <summary>
    /// Десериализует JSON-строку в объект указанного типа T.
    /// </summary>
    /// <typeparam name="T">Тип, в который десериализуется JSON-строка.</typeparam>
    /// <param name="json">JSON-строка для десериализации.</param>
    /// <returns>Объект указанного типа T, или null, если десериализация завершается с ошибкой.</returns>
    /// <exception cref="JsonException">Выбрасывается, когда десериализация завершается с ошибкой, с описанием ошибки.</exception>
    private T GetObjectFromJSON<T>(string json)
    {
        try
        {
            var result = JsonSerializer.Deserialize<T>(json);
            return result!;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "{method} Ошибка десериализации JSON: {json} Reason: {reason}",
                nameof(GetObjectFromJSON), json, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод для асинхронного чтения содержимого JSON-файла.
    /// Если файл не найден, в журнал ошибок записывается сообщение.
    /// </summary>
    /// <param name="path">Путь к JSON-файлу.</param>
    /// <returns>Содержимое JSON-файла в виде строки или <c>null</c>, если файл не найден.</returns>
    private async Task<string?> GetStringJsonFromJSONFileAsync(string path)
    {
        // Проверка существования файла по указанному пути
        if (File.Exists(path))
        {
            // Асинхронное чтение содержимого файла
            return await File.ReadAllTextAsync(path);
        }
        else
        {
            // Запись в журнал ошибок, если файл не найден
            _logger.LogError("{method} Файл не найден: {path}", nameof(GetStringJsonFromJSONFileAsync), path);
            // Возврат null, если файл не найден
            return null;
        }
    }
}