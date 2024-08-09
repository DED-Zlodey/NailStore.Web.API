using NailStore.Core.Models;

namespace NailStore.Core.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="url">Начальный адрес для подтверждения Email</param>
    /// <param name="userName">Никнейм пользователя</param>
    /// <param name="email">Email пользовтаеля</param>
    /// <param name="password">Пароль пользователя</param>
    /// <returns>Возвращает объект ответа</returns>
    Task<ResponseModelCore<string>> RegisterUserAsync(string url, string userName, string email, string password);
    /// <summary>
    /// Аутентификация пользователя по Email и паролю
    /// </summary>
    /// <param name="email">Email пользователя</param>
    /// <param name="password">Пароль пользователя</param>
    /// <returns>Возвращает объект ответа</returns>
    Task<ResponseModelCore<string>> LoginUserAsync(string email, string password);
    /// <summary>
    /// Получить пользователя по его идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Возвращает объект ответа</returns>
    Task<ResponseModelCore<UserIdentityCoreModel>> GetUserByIdAsync(Guid id);
    /// <summary>
    ///  Подтверждение Email пользователя
    /// </summary>
    /// <param name="userConfirmited">модель подтверждения Email</param>
    /// <returns>Возвращает объект ответа</returns>
    Task<ResponseModelCore<string>> ConfirmedEmailUser(UserConfirmitedEmail userConfirmited);
    /// <summary>
    /// Проверяет свободно ли имя пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <returns>Вернет <b>true</b>, если имя свободно и <b>false</b>, если не свободно</returns>
    Task<bool> UserNameIsFreeAsync(string username);
    /// <summary>
    /// Отправить инструкции на почту по восстановлению пароля
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <param name="url">URL для формирования колбэк ссылки</param>
    /// <returns></returns>
    Task<ResponseModelCore<string>> RecoveryPasswordSend(string email, string url);
    /// <summary>
    /// Сменить пароль на новый
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="inputСode">Токен подтверждения смены пароля</param>
    /// <param name="newPass">Новый пароль</param>
    /// <returns>Возвращает объект ответа</returns>
    Task<ResponseModelCore<string>> RecoveryPassword(string userId, string inputСode, string newPass);
    /// <summary>
    /// Получить роли пользователя
    /// </summary>
    /// <param name="userId">Идентификтор пользователя</param>
    /// <returns>Верент список ролей пользователя</returns>
    Task<string[]> GetUserRolesAsync(string userId);
    /// <summary>
    /// Имеются ли роли у пользователя?
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="inputRoles">Роли которые проверяются</param>
    /// <returns>Верент <b>true</b>,  если роли у пользователя имеются и <b>false</b>, если ни одной роли пользователь не имеет</returns>
    Task<bool> IsRolesAllowedAsync(string? userId, List<string> inputRoles);
}
