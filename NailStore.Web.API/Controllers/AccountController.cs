using Microsoft.AspNetCore.Mvc;
using NailStore.Core.Interfaces;
using NailStore.Core.Models;
using NailStore.Web.API.DTOs.Account;

namespace NailStore.Web.API.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    public AccountController(IUserService userService)
    {
        _userService = userService;
    }
    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <remarks>
    ///     {
    ///        "UserName" : "Никнейм пользователя", 
    ///        "Email" : "Email пользователя",
    ///        "Password" : "Пароль пользователя"
    ///     }
    /// </remarks>
    /// <param name="UserName">Никнейм пользователя</param>
    /// <param name="Email">Email пользователя</param>
    /// <param name="Password">Пароль пользователя</param>
    /// <returns></returns>
    [HttpPost("Register")]
    public async Task<ActionResult<string>> PostRegister([FromBody] RequestUser model)
    {
        if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return ValidationProblem("Одно или несколько обязательных полей не заполнены");
        }
        if (string.IsNullOrEmpty(model.Url))
        {
            model.Url = $"{Request.Scheme}://{Request.Host}/api/Account/ConfirmEmail/";
        }
        var result = await _userService.RegisterUserAsync(model.Url, model.UserName, model.Email, model.Password);
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
    /// <summary>
    /// Аутентификация пользователя
    /// </summary>
    /// <remarks>
    ///     {
    ///        "Email" : "Email пользователя",
    ///        "Password" : "Пароль пользователя"
    ///     }
    /// </remarks> 
    /// <param name="model">Модель запроса пользователя</param>
    /// <returns></returns>
    [HttpPost("Login")]
    public async Task<ActionResult<string>> PostLogin([FromBody] RequestUser model)
    {
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return ValidationProblem("Одно или несколько обязательных полей не заполнены");
        }
        var result = await _userService.LoginUserAsync(model.Email, model.Password);
        if (result.Header.StatusCode != 200)
        {
            return Problem
                           (
                               detail: result.Header.Error,
                               statusCode: result.Header.StatusCode,
                               instance: HttpContext.Request.Path
                           );
        }
        return Ok(result.Body.Token);
    }
    /// <summary>
    /// Подтверждение Email пользователя с помощью которого производилась регистрация
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="code">Секретный код высланный на почту</param>
    /// <returns></returns>
    [HttpGet]
    [Route("ConfirmEmail/{userId}/{code}")]
    public async Task<ActionResult<string>> ConfirmedEmail(string userId, string code)
    {
        var confirmModel = UserConfirmitedEmail.Create(userId, code);
        if (confirmModel.Validator.StatusCode != 200)
        {
            return Problem
                           (
                               detail: confirmModel.Validator.Error,
                               statusCode: confirmModel.Validator.StatusCode,
                               instance: HttpContext.Request.Path
                           );
        }
        var result = await _userService.ConfirmedEmailUser(confirmModel.ConfirmModel);
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
    /// <summary>
    /// Проверят свободен ли никнейм пользователя.
    /// Вернет true, если имя свободно и false, если несвободно
    /// </summary>
    /// <param name="username">Никнейм пользователя</param>
    /// <returns>Вернет <b>true</b>, если имя свободно и <b>false</b>, если несвободно</returns>
    [HttpGet]
    [Route("UserNameIsFree/{username}")]
    public async Task<ActionResult<bool>> UserNameIsFree(string username)
    {
        return Ok(await _userService.UserNameIsFreeAsync(username));
    }
    /// <summary>
    /// Отправить на почту инструкцию по восстановлению пароля
    /// </summary>
    /// <remarks>
    ///     {
    ///        "Email" : "Email пользователя"
    ///     }
    /// </remarks> 
    /// <param name="email">Email пользователя</param>
    /// <returns></returns>
    [HttpPost]
    [Route("SendRecoverPassword")]
    public async Task<ActionResult<string>> SendRecoverPassword([FromBody] SendRecoverPasswordDTO model)
    {
        if (string.IsNullOrEmpty(model.Url))
        {
            model.Url = $"{Request.Scheme}://{Request.Host}/api/Account/ConfirmEmail/";
        }
        var result = await _userService.RecoveryPasswordSend(model.Email, model.Url);
        return Ok(result.Body.Message);
    }
    /// <summary>
    /// Изменить пароль на новый
    /// </summary>
    /// <remarks>
    ///     {
    ///        "userId" : "идентификатор пользователя",
    ///        "token" : "код отправленный на почту пользователя",
    ///        "password" : "Новый пароль"
    ///     }
    /// </remarks> 
    /// <param name="model">Модель смены пароля на новый</param>
    /// <returns></returns>
    [HttpPost]
    [Route("ReSetPassword")]
    public async Task<ActionResult<string>> RecoverPassword([FromBody] RecoverPasswordDTO model)
    {
        var result = await _userService.RecoveryPassword(model.UserId.ToString(), model.Token, model.Password);
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

}
