using Microsoft.AspNetCore.Mvc;
using NailStore.Core.Interfaces;
using NailStore.Core.Models;
using NailStore.Web.API.DTOs;

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
        if (model.Url == null) 
        {
            model.Url = $"{Request.Scheme}://{Request.Host}/api/Account/ConfirmEmail/";
        }
        var result = await _userService.RegisterUserAsync(model.Url, model.UserName, model.Email, model.Password);
        if (result.Header.StatusCode != 200)
        {
            return ValidationProblem(result.Header.Error);
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
            HttpContext.Response.StatusCode = result.Header.StatusCode;
            return ValidationProblem(result.Header.Error);
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
            return BadRequest(confirmModel.Validator.Error);
        }
        var result = await _userService.ConfirmedEmailUser(confirmModel.ConfirmModel);
        if (result.Header.StatusCode != 200)
        {
            HttpContext.Response.StatusCode = result.Header.StatusCode;
            return ValidationProblem(result.Header.Error);
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
    public async Task<ActionResult<string>> SendRecoverPassword(string email, string? url)
    {
        if(string.IsNullOrEmpty(email))
        {
            return ValidationProblem("Одно или несколько обязательных полей не заполнены");
        }
        if(url == null)
        {
            url = $"{Request.Scheme}://{Request.Host}/api/Account/ConfirmEmail/";
        }
        var result = await _userService.RecoveryPasswordSend(email, url);
        return Ok(result.Body.Message);
    }
    /// <summary>
    /// Изменить пароль на новый
    /// </summary>
    /// <remarks>
    ///     {
    ///        "userId" : "идентификатор пользователя",
    ///        "code" : "код отправленный на почту пользователя",
    ///        "password" : "Новый пароль"
    ///     }
    /// </remarks> 
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="code">Код отправленный на почту пользователя</param>
    /// <param name="password">Новый пароль</param>
    /// <returns></returns>
    [HttpPost]
    [Route("ReSetPassword")]
    public async Task<ActionResult<string>> RecoverPassword(string userId, string code, string password)
    {
        var result = await _userService.RecoveryPassword(userId, code, password);
        if(result.Header.StatusCode != 200)
        {
            HttpContext.Response.StatusCode = result.Header.StatusCode;
            return ValidationProblem(result.Header.Error);
        }
        return Ok(result.Body.Message);
    }
}
