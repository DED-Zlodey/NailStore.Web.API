using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NailStore.Application.Interfaces;
using NailStore.Core.Interfaces;
using NailStore.Web.API.DTOs.Services;
using NailStore.Web.API.Extensions;

namespace NailStore.Web.API.Controllers
{
    /// <summary>
    /// Конечная точка для работы с услугами
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IProviderService<Guid> _providerService;
        private readonly IJWTManager _jwtService;
        private readonly ILogger<ServiceController> _logger;
        private readonly IUserService _userService;

        /// <summary>
        /// Основной консруктор
        /// </summary>
        /// <param name="providerService">Сервис для работы с услугами</param>
        /// <param name="logger">Логгер</param>
        /// <param name="jWtManager">Сервис для работы с JWT токенами</param>
        /// <param name="userService">Сервис для работы с пользователями</param>
        public ServiceController(IProviderService<Guid> providerService, ILogger<ServiceController> logger,
            IJWTManager jWtManager, IUserService userService)
        {
            _providerService = providerService;
            _logger = logger;
            _jwtService = jWtManager;
            _userService = userService;
        }
        
        /// <summary>
        /// Добавляет услугу.
        /// Метод доступен только авторизованным пользователям.
        /// </summary>
        /// <remarks>
        ///     {
        ///        "categoryId": 2,
        ///        "serviceName": "Маникюр \"Пальчики плюс\" (аппаратный)",
        ///        "durationTime": 90,
        ///        "price": 5700,
        ///        "descriptionList" : [
        ///             {
        ///                  "number": 1,
        ///                  "text": "Текст параграфа"
        ///             },
        ///             {
        ///                  "number": 2,
        ///                  "text": "Текст параграфа"
        ///             }        
        ///        ]
        ///     }
        /// </remarks> 
        /// <param name="model">Модель услуги.</param>
        /// <returns>
        /// Возвращает сообщение об успешном добавлении, если все операции прошли успешно.
        /// Возвращает ошибку 400 Bad Request, если не удалось распознать пользователя.
        /// Возвращает ошибку с описанием причины, если при вызове сервиса возникли ошибки.
        /// Возвращает ошибку 403 Forbidden, если у пользователя нет необходимой роли для добавления услуги.
        /// </returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> AddServiceAsync([FromBody] ServiceModelDTO model)
        {
            var userId = GetUserIdByToken();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Не удалось распознать пользователя");
            }

            if (await _userService.IsRolesAllowedAsync(userId, new List<string> { "Admin", "Master" }))
            {
                var array = new string[model.DescriptionList.Count];
                model.DescriptionList.Sort();
                for (int i = 0; i < model.DescriptionList.Count; i++)
                {
                    array[i] = model.DescriptionList[i].Text;
                }

                var result = await _providerService.AddServiceAsync(Guid.Parse(userId), model.CategoryId,
                    model.ServiceName, array, model.Price, model.DurationTime);
                if (result.Header.StatusCode != 200)
                {
                    return Problem
                    (
                        detail: result.Header.Error,
                        statusCode: result.Header.StatusCode,
                        instance: HttpContext.Request.Path
                    );
                }

                return Ok(result.Body.Message);
            }

            return Problem
            (
                detail: "Для доступа к ресурсу нет необходимой роли.",
                statusCode: StatusCodes.Status403Forbidden,
                instance: HttpContext.Request.Path
            );
        }
        /// <summary>
        /// Получает все услуги для определенной категории.
        /// </summary>
        /// <param name="categoryId">Идентификатор категории.</param>
        /// <param name="pageNumber">Номер страницы для пагинации.</param>
        /// <param name="pageSize">Количество элементов на странице.</param>
        /// <returns>
        /// Возвращает список услуг, если все операции прошли успешно.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<GetServicesDTO>> GetServicesByCategoryIdAsync(int categoryId, int pageNumber,
            int pageSize)
        {
            var result = await _providerService.GetServicesByCategoryAsync(categoryId, pageNumber, pageSize);
            return Ok(result.Body.GetServices);
        }

        /// <summary>
        /// Метод для получения всех услуг, привязанных к определенному пользователю, с поддержкой пагинации.
        /// Метод доступен только авторизованным пользователям.
        /// </summary>
        /// <param name="pageNumber">Номер страницы для пагинации.</param>
        /// <param name="pageSize">Количество элементов на странице.</param>
        /// <returns>
        /// Возвращает список услуг, если все операции прошли успешно.
        /// Возвращает ошибку 400 Bad Request, если не удалось распознать пользователя.
        /// Возвращает ошибку с описанием причины, если при вызове сервиса возникли ошибки.
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("GetAllServicesByUser/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<GetServicesDTO>> GetAllServicesByUserIdAsync(int pageNumber, int pageSize)
        {
            // Получение идентификатора пользователя из токена в заголовке запроса
            var userId = GetUserIdByToken();
            if (string.IsNullOrEmpty(userId))
            {
                // Если идентификатор пользователя не получен, возвращается ошибка 400 Bad Request
                return Problem
                (
                    detail: "Не удалось распознать пользователя выполняющего запрос",
                    statusCode: StatusCodes.Status400BadRequest,
                    instance: HttpContext.Request.Path
                );
            }

            // Вызов метода сервиса для получения всех услуг пользователя с учетом пагинации
            var result = await _providerService.GetAllServicesByUserIdAsync(Guid.Parse(userId), pageNumber, pageSize);
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

            // Если все операции прошли успешно, возвращается список услуг
            return Ok(result.Body.GetServices);
        }
        
        /// <summary>
        /// Удалить услугу по ее идентификатору. Удалить можно только свою услугу!
        /// Метод доступен только авторизованным пользователям.
        /// </summary>
        /// <param name="model">Модель удаления услуги</param>
        /// <returns>
        /// Возвращает сообщение об успешном удалении, если все операции прошли успешно.
        /// Возвращает ошибку 400 Bad Request, если не удалось распознать пользователя.
        /// Возвращает ошибку с описанием причины, если при вызове сервиса возникли ошибки.
        /// Возвращает ошибку 403 Forbidden, если у пользователя нет необходимой роли для удаления услуги.
        /// </returns>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<string>> DeleteService([FromBody] DeleteServiceModelDTO model)
        {
            var userId = GetUserIdByToken();
            if (string.IsNullOrEmpty(userId))
            {
                // Если идентификатор пользователя не получен, возвращается ошибка 400 Bad Request
                return BadRequest("Не удалось распознать пользователя");
            }

            if (await _userService.IsRolesAllowedAsync(userId, new List<string> { "Admin", "Master" }))
            {
                var result = await _providerService.RemoveServiceAsync(model.ServiceId, Guid.Parse(userId));
                if (result.Header.StatusCode == 200)
                {
                    // Если услуга успешно удалена, возвращается сообщение об успехе
                    return Ok(result.Body.Message);
                }

                // Если при вызове сервиса возникли ошибки, возвращается ошибка с описанием причины
                return Problem
                (
                    detail: result.Header.Error,
                    statusCode: result.Header.StatusCode,
                    instance: HttpContext.Request.Path
                );
            }

            // Если у пользователя нет необходимой роли для удаления услуги, возвращается ошибка 403 Forbidden
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
                var result = HttpContext.GetPayloadForTokenAsync(_jwtService);
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
        }    }
}