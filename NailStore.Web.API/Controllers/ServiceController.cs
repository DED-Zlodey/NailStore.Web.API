using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NailStore.Core.Interfaces;
using NailStore.Web.API.DTOs.Services;
using NailStore.Web.API.Extensions;

namespace NailStore.Web.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IProviderService _providerService;
        private readonly IJWTManager _jwtService;
        private readonly ILogger<ServiceController> _logger;
        private readonly IUserService _userService;

        public ServiceController(IProviderService providerService, ILogger<ServiceController> logger, IJWTManager jWTManager, IUserService userService)
        {
            _providerService = providerService;
            _logger = logger;
            _jwtService = jWTManager;
            _userService = userService;
        }

        /// <summary>
        /// Добавить услугу
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
        /// <param name="model">Модель услуги</param>
        /// <returns></returns>
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
                var result = await _providerService.AddServiceAsync(Guid.Parse(userId), model.CategoryId, model.ServiceName, array, model.Price, model.DurationTime);
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
            else
            {
                return Problem
                            (
                                detail: "Для доступа к ресурсу нет необходимой роли.",
                                statusCode: StatusCodes.Status403Forbidden,
                                instance: HttpContext.Request.Path
                            );
            }
        }
        /// <summary>
        /// Получить все услуги для определенной категории
        /// </summary>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<ServiceModelDTO>>> GetServicesByCategoryIdAsync(int categoryId)
        {
            var result = await _providerService.GetServicesByCategoryAsync(categoryId);
            return Ok(result.Body.Services);
        }

        /// <summary>
        /// На основании токена переданного в заголовке возвращает идентификатор пользователя.
        /// </summary>
        /// <returns>Возвращает идентификатор пользователя или  <b>null</b></returns>
        private string GetUserIdByToken()
        {
            try
            {
                var result = HttpContext.GetPayloadForTokenAsync(_jwtService!);
                if (result == null)
                {
                    _logger.LogError("{method} Не удалось получить пользователя из токена в заголовке. result == null", nameof(GetUserIdByToken));
                    return null;
                }
                if (result.Payload == null)
                {
                    _logger.LogError("{method} Не удалось получить пользователя из токена в заголовке. result.Payload == null", nameof(GetUserIdByToken));
                    return null;
                }
                return result!.Payload!.GetValueOrDefault("Id")!.ToString()!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{method} Не удалось получить пользователя из токена в заголовке. Reason: {reason}", nameof(GetUserIdByToken), ex.Message);
                return null;
            }
        }
    }
}
