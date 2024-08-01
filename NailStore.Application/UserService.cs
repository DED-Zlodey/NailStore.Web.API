using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NailStore.Core.Interfaces;
using NailStore.Core.Models;
using NailStore.Data.Models;
using System.Runtime.InteropServices;
using System.Text;
using NailStore.Application.Interfaces;
using NailStore.Application.Mapping;

namespace NailStore.Application;

public class UserService : IUserService
{
    readonly ILogger<UserService> _logger;
    readonly UserManager<UserEntity> _userManager;
    readonly RoleManager<IdentityRole<Guid>> _roleManager;
    readonly SignInManager<UserEntity> _signInManager;
    readonly IJWTManager _jwtManager;
    readonly IEmailService _emailService;

    public UserService(UserManager<UserEntity> userManager, RoleManager<IdentityRole<Guid>> roleManager,
        SignInManager<UserEntity> signInManager,
        IJWTManager jWTManager, IEmailService emailService, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _jwtManager = jWTManager;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    ///  Подтверждение Email пользователя
    /// </summary>
    /// <param name="userConfirmited">модель подтверждения Email</param>
    /// <returns>Возвращает объект ответа</returns>
    public async Task<ResponseModelCore> ConfirmedEmailUser(UserConfirmitedEmail userConfirmited)
    {
        var user = await _userManager.FindByIdAsync(userConfirmited.UserId);
        if (user == null)
        {
            return new ResponseModelCore
            {
                Header = new()
                {
                    Error = $"Не удалось получить идентификатор пользователя '{userConfirmited.UserId}'.",
                    StatusCode = 500
                }
            };
        }

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userConfirmited.Code));
        var result = await _userManager.ConfirmEmailAsync(user!, code);
        if (result.Succeeded)
        {
            return new ResponseModelCore
            {
                Header = new()
                {
                    Error = string.Empty,
                    StatusCode = 200
                },
                Body = new()
                {
                    Message = "Спасибо, что подтвердили свой адрес электронной почты."
                }
            };
        }
        else
        {
            var errorStr = GetIdentityErrorString(result.Errors.ToList());
            _logger.LogError("{method} Не удалось сменить пароль для акканта {accont}. Reason: {reason}",
                nameof(ConfirmedEmailUser), user.Email, errorStr);
            return new ResponseModelCore
            {
                Header = new()
                {
                    Error = $"Не удалось подтвердить почтовый ящик {user.Email} Reason: {errorStr}",
                    StatusCode = 500
                },
                Body = new()
                {
                    Message = $"Не удалось подтвердить почтовый ящик {user.Email} Reason: {errorStr}"
                }
            };
        }
    }

    /// <summary>
    /// Получить пользователя по его идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Возвращает объект ответа</returns>
    public async Task<ResponseModelCore> GetUserByIdAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            return UserIdentityCoreModel.CreateUser(user!.Id, user.UserName!, user.RegisterAt, user.PhoneNumber,
                user.Enable);
        }
        _logger.LogError("{method} Пользователь с идентификатором {id} не найден", nameof(GetUserByIdAsync), id);
        return new ResponseModelCore
        {
            Header = new()
            {
                Error = $"Пользователь с идентификатором '{id}' не найден.",
                StatusCode = 404
            }
        };
    }

    /// <summary>
    /// Асинхронно выполняет вход пользователя в систему.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <param name="password">Пароль пользователя.</param>
    /// <returns>
    /// Возвращает объект ответа, содержащий токен доступа в случае успешного входа.
    /// Если вход не удачен, возвращает соответствующее сообщение об ошибке.
    /// </returns>
    public async Task<ResponseModelCore> LoginUserAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
            if (result.Succeeded)
            {
                var userDTO = Mapper.MapTo(user);
                var token = await _jwtManager.GetBearerTokenAsync(userDTO);
                return new ResponseModelCore
                {
                    Header = new()
                    {
                        Error = string.Empty,
                        StatusCode = 200,
                    },
                    Body = new()
                    {
                        Token = token
                    }
                };
            }
            else
            {
                if (result.IsLockedOut)
                {
                    var lockoutEndDate = _userManager.GetLockoutEndDateAsync(user);
                    if (lockoutEndDate.IsCompletedSuccessfully)
                    {
                        var responseModel = new ResponseModelCore
                        {
                            Header = new()
                            {
                                Error =
                                    $"Из-за превышения неудачных попыток входа, пользователь заблокирован до {lockoutEndDate.Result.Value.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss")} UTC+8",
                                StatusCode = 403,
                            },
                            Body = new()
                            {
                                Message = "Из-за превышения неудачных попыток входа, пользователь заблокирован."
                            }
                        };
                        return responseModel;
                    }
                    return new ResponseModelCore
                    {
                        Header = new()
                        {
                            Error = "Доступ запрещен",
                            StatusCode = 403,
                        },
                        Body = new()
                        {
                            Message = "Доступ запрещен",
                        }
                    };
                }

                return new ResponseModelCore
                {
                    Header = new()
                    {
                        Error = $"Email {email} или пароль пользователя не верный",
                        StatusCode = 403
                    },
                    Body = new()
                    {
                        Message = $"Email {email} или пароль пользователя не верный"
                    }
                };
            }
        }
        else
        {
            return new ResponseModelCore
            {
                Header = new()
                {
                    Error = $"Email {email} или пароль пользователя не верный",
                    StatusCode = 403
                },
                Body = new()
                {
                    Message = $"Email {email} или пароль пользователя не верный"
                }
            };
        }
    }

    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="url">Начальный адрес для подтверждения Email</param>
    /// <param name="userName">Никнейм пользователя</param>
    /// <param name="email">Email пользователя</param>
    /// <param name="password">Пароль пользователя</param>
    /// <returns>Возвращает объект ответа</returns>
    public async Task<ResponseModelCore> RegisterUserAsync(string url, string userName, string email, string password)
    {
        if (!IsStopNickName(userName))
        {
            if (await UserNameIsFreeAsync(userName))
            {
                var userEmail = await _userManager.FindByEmailAsync(email);
                if (userEmail != null)
                {
                    var reason = $"{email} - уже существует!";
                    _logger.LogError("{nameMethod}: Пользователь {Email} не зарегистрирован. Reson: {errorString}",
                        nameof(RegisterUserAsync), email, reason);
                    return new ResponseModelCore
                    {
                        Header = new()
                            { Error = $"Пользователь {email} не зарегистрирован. Reson: {reason}", StatusCode = 400 },
                    };
                }

                var regUser = new UserEntity
                {
                    UserName = userName,
                    Email = email,
                    RegisterAt = DateTime.Now.ToUniversalTime(),
                    Enable = true
                };
                var result = await _userManager.CreateAsync(regUser, password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(regUser);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var urlCallback = $"{url}{regUser.Id}/{code}";
                    var resultSendEmail = await _emailService.SendEmailAsync(regUser.Email, "Подтверждение регистрации",
                        "Подтвердите вашу учетную запись, кликнув <a href=\"" + urlCallback + "\">здесь</a>");
                    if (resultSendEmail.IsSending)
                    {
                        var res = await _userManager.AddToRoleAsync(regUser, "User");
                        if (res.Succeeded)
                        {
                            _logger.LogInformation("{nameMethod}: Роль: \"User\" присвоена пользователю: {Email}",
                                nameof(RegisterUserAsync), regUser.Email);
                        }
                        else
                        {
                            _logger.LogError(
                                "{nameMethod}: Не удалось присвоить роль: \"User\", пользовтаелю: {Email}.",
                                nameof(RegisterUserAsync), regUser.Email);
                        }

                        return new ResponseModelCore
                        {
                            Header = new()
                            {
                                Error = string.Empty,
                                StatusCode = 200
                            },
                            Body = new()
                            {
                                Message =
                                    "Регистрация прошла успешно! На электронную почту выслано письмо, для завершения регистрации выполните инструкции в отправленном письме. Спасибо!"
                            }
                        };
                    }
                    else
                    {
                        await _userManager.DeleteAsync(regUser);
                        _logger.LogError(
                            "{nameMethod}: Пользователь {Email} не зарегистрирован. Не удалось отправить письмо",
                            nameof(RegisterUserAsync), regUser.Email);
                        return new ResponseModelCore
                        {
                            Header = new()
                            {
                                Error =
                                    $"Пользователь {regUser.Email} не зарегистрирован. Не удалось отправить письмо. Error: {resultSendEmail.StatusCode}",
                                StatusCode = 500
                            }
                        };
                    }
                }
                else
                {
                    var errorString = GetIdentityErrorString(result.Errors.ToList());
                    _logger.LogError("{nameMethod}: Пользователь {Email} не зарегистрирован. Reson: {errorString}",
                        nameof(RegisterUserAsync), regUser.Email, errorString);
                    return new ResponseModelCore
                    {
                        Header = new()
                        {
                            Error = $"Пользователь {regUser.Email} не зарегистрирован. Reson: {errorString}",
                            StatusCode = 500
                        },
                    };
                }
            }
            else
            {
                var reason = $"{userName} - никнейм уже занят!";
                _logger.LogError("{nameMethod}: Пользователь {Email} не зарегистрирован. Reson: {errorString}",
                    nameof(RegisterUserAsync), email, reason);
                return new ResponseModelCore
                {
                    Header = new()
                        { Error = $"Пользователь {email} не зарегистрирован. Reson: {reason}", StatusCode = 400 },
                };
            }
        }
        else
        {
            var reason = $"{userName} - никнейм уже занят!";
            _logger.LogError("{nameMethod}: Пользователь {Email} не зарегистрирован. Reson: {errorString}",
                nameof(RegisterUserAsync), email, reason);
            return new ResponseModelCore
            {
                Header = new()
                    { Error = $"Пользователь {email} не зарегистрирован. Reson: {reason}", StatusCode = 400 },
            };
        }
    }

    /// <summary>
    /// Проверяет свободно ли имя пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <returns>Вернет <b>true</b>, если имя свободно и <b>false</b>, если не свободно</returns>
    public async Task<bool> UserNameIsFreeAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return false;
        }
        if (username.Length < 3)
        {
            return false;
        }

        var result = await _userManager.FindByNameAsync(username);
        if (result != null)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Отправить инструкции на почту по восстановлению пароля
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <param name="url">URL для формирования колбэк ссылки</param>
    /// <returns>Возвращает объект ответа</returns>
    public async Task<ResponseModelCore> RecoveryPasswordSend(string email, string url)
    {
        if (string.IsNullOrEmpty(email))
        {
            return new ResponseModelCore
            {
                Header = new()
                {
                    Error = "Восстановление пароля невозможно. Reason: Email не может быть пустым или равным null",
                    StatusCode = 400
                }
            };
        }
        if (string.IsNullOrEmpty(url))
        {
            return new ResponseModelCore
            {
                Header = new()
                {
                    Error = "Восстановление пароля невозможно. Reason: URL не может быть пустым или равным null",
                    StatusCode = 400
                }
            };
        }
        var user = await _userManager.FindByNameAsync(email);
        if (user == null)
        {
            _logger.LogError(
                "{nameMethod}: Пользователь, с почтовым ящиком: {Email}, не зарегистрирован в системе. Восстановить пароль для данного пользователя невозможно",
                nameof(RecoveryPasswordSend), email);
            return new ResponseModelCore
            {
                Header = new() { Error = string.Empty, StatusCode = 200 },
                Body = new()
                {
                    Message = "На указанную Вами почту отправлены инструкции для восстановления пароля"
                }
            };
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var urlCallback = $"{url}{user.Id}/{code}";
        var resultSendEmail = await _emailService.SendEmailAsync(user.Email!, "Восстановление пароля",
            "Подтвердите вашу учетную запись, кликнув <a href=\"" + urlCallback + "\">здесь</a>");
        if (resultSendEmail.IsSending)
        {
            return new ResponseModelCore
            {
                Header = new() { Error = string.Empty, StatusCode = 200 },
                Body = new()
                {
                    Message = "На указанную Вами почту отправлены инструкции для восстановления пароля"
                }
            };
        }
        else
        {
            return new ResponseModelCore
            {
                Header = new() { Error = string.Empty, StatusCode = resultSendEmail.StatusCode },
                Body = new()
                {
                    Message =
                        $"Не удалось отправить инструкции по восстановлению пароля на указанную Вами почту. Возможно, были допущены опечатки при вводе адреса эл. почты? Error: {resultSendEmail.StatusCode}"
                }
            };
        }
    }

    /// <summary>
    /// Сменить пароль на новый
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="inputСode">Токен подтверждения смены пароля</param>
    /// <param name="newPass">Новый пароль</param>
    /// <returns>Возвращает объект ответа</returns>
    public async Task<ResponseModelCore> RecoveryPassword(string userId, string inputСode, string newPass)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return new ResponseModelCore
            {
                Header = new()
                {
                    Error = "Смена пароля невозможна. Reason: userId не может быть пустым или равным null",
                    StatusCode = 400
                }
            };
        }

        if (string.IsNullOrEmpty(inputСode))
        {
            return new ResponseModelCore
            {
                Header = new()
                {
                    Error = "Смена пароля невозможна. Reason: Token не может быть пустым или равным null",
                    StatusCode = 400
                }
            };
        }

        if (string.IsNullOrEmpty(newPass))
        {
            return new ResponseModelCore
            {
                Header = new()
                {
                    Error = "Смена пароля невозможна. Reason: Новый пароль не может быть пустым или равным null",
                    StatusCode = 400
                }
            };
        }
        
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("{method} Не удалось получить пользователя по его идентификатору: {id}",
                    nameof(RecoveryPassword), userId);
                return new ResponseModelCore
                {
                    Header = new()
                    {
                        Error = $"Не удалось получить пользователя по его идентификатору: '{userId}'.",
                        StatusCode = 404
                    }
                };
            }

            //var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(inputСode));
            var result = await _userManager.ResetPasswordAsync(user!, inputСode, newPass);
            if (result.Succeeded)
            {
                return new ResponseModelCore
                {
                    Header = new()
                    {
                        Error = string.Empty,
                        StatusCode = 200
                    },
                    Body = new()
                    {
                        Message = $"Пароль успешно изменен"
                    }
                };
            }
            else
            {
                var errorStr = GetIdentityErrorString(result.Errors.ToList());
                _logger.LogError("{method} Не удалось сменить пароль для аккаунта {accont}. Reason: {reason}",
                    nameof(RecoveryPassword), user.Email, errorStr);
                return new ResponseModelCore
                {
                    Header = new()
                    {
                        Error = $"Не удалось сменить пароль. Reason: {errorStr}",
                        StatusCode = 500
                    },
                    Body = new()
                    {
                        Message = $"Не удалось сменить пароль. Reason: {errorStr}"
                    }
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{method} Не удалось сменить пароль для аккаунта c Id: {accont}. Reason: {reason}",
                nameof(RecoveryPassword), userId, ex.Message);
            return new ResponseModelCore
            {
                Header = new()
                {
                    Error = $"Не удалось сменить пароль.",
                    StatusCode = 500
                },
                Body = new()
                {
                    Message = $"Не удалось сменить пароль."
                }
            };
        }
    }

    /// <summary>
    /// Получить роли пользователя
    /// </summary>
    /// <param name="userId">Идентификтор пользователя</param>
    /// <returns>Верент список ролей пользователя</returns>
    public async Task<string[]> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null!;
        }

        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToArray();
    }

    /// <summary>
    /// Имеются ли роли у пользователя?
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="inputRoles">Роли которые проверяются</param>
    /// <returns>Верент <b>true</b>,  если роли у пользователя имеются и <b>false</b>, если ни одной роли пользователь не имеет</returns>
    public async Task<bool> IsRolesAllowedAsync(string? userId, List<string> inputRoles)
    {
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
        {
            return false;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var intersect = roles.Intersect(inputRoles).ToArray();
        if (intersect.Length > 0)
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Получить все ошибки Identity в одну строку
    /// </summary>
    /// <param name="errors">Список ошибок</param>
    /// <returns>Верент все ошибки в одну строку</returns>
    private string GetIdentityErrorString(List<IdentityError> errors)
    {
        StringBuilder sb = new StringBuilder();
        int counter = 0;
        foreach (var error in CollectionsMarshal.AsSpan(errors))
        {
            if (!string.IsNullOrEmpty(error.Description))
            {
                if (counter != errors.Count - 1)
                {
                    sb.Append($"{error.Description}, ");
                }
                else
                {
                    sb.Append($"{error.Description}");
                }
            }

            counter++;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Проверяет, запрещен ли никнейм для регистрации.
    /// </summary>
    /// <param name="nickName">Никнейм, который необходимо проверить.</param>
    /// <returns>
    /// Возвращает <c>true</c>, если никнейм запрещен, в противном случае возвращает <c>false</c>.
    /// </returns>
    private bool IsStopNickName(ReadOnlySpan<char> nickName)
    {
        // Проверяет, содержит ли никнейм "Admin" или "admin"
        if (nickName.IndexOf("Admin") >= 0 || nickName.IndexOf("admin") >= 0)
        {
            return true;
        }

        // Если никнейм не содержит "Admin" или "admin", он разрешен
        return false;
    }
}