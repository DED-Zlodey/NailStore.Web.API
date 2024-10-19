using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NailStore.Core.Interfaces;
using NailStore.Core.Models.Country;
using NailStore.Core.Models.GeoLocation;
using NailStore.Web.API.Extensions;

namespace NailStore.Web.API.Controllers;

/// <summary>
/// Контроллер для работы с геоданными
/// </summary>
[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class GeoController : ControllerBase
{
    /// <summary>
    /// Сервис для работы с геоданными
    /// </summary>
    private readonly IGeoService _geoService;
    /// <summary>
    /// Инструмент для ведения журнала
    /// </summary>
    private readonly ILogger<GeoController> _logger;
    /// <summary>
    /// Менеджер для работы с JWT-токенами
    /// </summary>
    private readonly IJWTManager _jWtManager;
    /// <summary>
    /// Сервис для работы с пользователями
    /// </summary>
    private readonly IUserService _userService;

    /// <summary>
    /// Конструктор контроллера GeoController. Инициализирует зависимости и необходимые сервисы.
    /// </summary>
    /// <param name="geoService">Сервис для работы с геоданными.</param>
    /// <param name="logger">Инструмент для ведения журнала.</param>
    /// <param name="jWtManager">Менеджер для работы с JWT-токенами.</param>
    /// <param name="userService">Сервис для работы с пользователями.</param>
    public GeoController(IGeoService geoService, ILogger<GeoController> logger, IJWTManager jWtManager,
        IUserService userService)
    {
        _geoService = geoService;
        _logger = logger;
        _jWtManager = jWtManager;
        _userService = userService;
    }
    /// <summary>
    /// Обрабатывает HTTP-запрос GET для получения списка городов по идентификатору региона.
    /// </summary>
    /// <param name="regionId">Идентификатор региона.</param>
    /// <returns>
    /// Возвращает <see cref="ActionResult{List}"/> со списком городов или <see cref="OkObjectResult"/> с пустым списком, если городов не найдено.
    /// </returns>
    [HttpGet]
    [Authorize]
    [Route("CitiesByRegionId")]
    public async Task<ActionResult<List<CityDTO>>> GetCitiesFromRegionIdAsync(int regionId)
    {
        return Ok(await _geoService.GetCitiesFromRegionId(regionId));
    }

    /// <summary>
    /// Обрабатывает HTTP-запрос POST для добавления списка геолокаций в систему.
    /// </summary>
    /// <param name="locations">Список объектов DTO геолокаций для добавления.</param>
    /// <returns>
    /// Возвращает <see cref="ActionResult{String}"/> со следующими условиями:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// Если пользователь не распознан, возвращается <see cref="ProblemDetails"/> с кодом состояния 403 (Forbidden) и сообщением "Для доступа к ресурсу нет необходимой роли".
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Если у пользователя нет роли "Admin", возвращается <see cref="ProblemDetails"/> с кодом состояния 403 (Forbidden) и сообщением "Для доступа к ресурсу нет необходимой роли".
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Если геолокации успешно добавлены, возвращается <see cref="OkObjectResult"/> с сообщением "Ok".
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Если при вызове сервиса возникли ошибки, возвращается <see cref="ProblemDetails"/> с соответствующим кодом состояния и подробностями об ошибке.
    /// </description>
    /// </item>
    /// </list>
    /// </returns>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<string>> AddLocationsAsync(List<GeolocationDTO> locations)
    {
        var userId = GetUserIdByToken();
        if (string.IsNullOrEmpty(userId))
        {
            return Problem
            (
                detail: "Для доступа к ресурсу нет необходимой роли.",
                statusCode: StatusCodes.Status403Forbidden,
                instance: HttpContext.Request.Path
            );
        }

        if (await _userService.IsRolesAllowedAsync(userId, new List<string> { "Admin" }))
        {
            var result = await _geoService.AddGeolocationsAsync(locations);
            if (result.Header.StatusCode != 200)
            {
                // Если при вызове сервиса возникли ошибки, возвращается ошибка с описанием причины
                return Problem
                (
                    detail: result.Header.Error,
                    statusCode: result.Header.StatusCode,
                    instance: HttpContext.Request.Path
                );
            }

            return Ok("Ok");
        }

        return Problem
        (
            detail: "Для доступа к ресурсу нет необходимой роли.",
            statusCode: StatusCodes.Status403Forbidden,
            instance: HttpContext.Request.Path
        );
    }

    /// <summary>
    /// На основании токена переданного в заголовке возвращает идентификатор пользователя.
    /// </summary>
    /// <returns>
    /// Возвращает идентификатор пользователя или <c>null</c>, если не удалось получить идентификатор.
    /// </returns>
    private string? GetUserIdByToken()
    {
        try
        {
            var result = HttpContext.GetPayloadForTokenAsync(_jWtManager);
            if (result == null)
            {
                // Если не удалось получить пользователя из токена в заголовке, null. В журнал записывается соответствующая запись.
                _logger.LogError("{method} Не удалось получить пользователя из токена в заголовке. result == null",
                    nameof(GetUserIdByToken));
                return null;
            }

            if (result.Payload == null)
            {
                // Если не удалось получить пользователя из токена в заголовке, null. В журнал записывается соответствующая запись.
                _logger.LogError(
                    "{method} Не удалось получить пользователя из токена в заголовке. result.Payload == null",
                    nameof(GetUserIdByToken));
                return null;
            }

            // Если удалось получить пользователя из токена в заголовке, возвращается его идентификатор
            return result.Payload!.GetValueOrDefault("Id")!.ToString()!;
        }
        catch (Exception ex)
        {
            // Если при получении пользователя из токена в заголовке возникла ошибка, возвращается null. В журнал записывается соответствующая запись.
            _logger.LogError(ex,
                "{method} Не удалось получить пользователя из токена в заголовке. Reason: {reason}",
                nameof(GetUserIdByToken), ex.Message);
            return null;
        }
    }
}