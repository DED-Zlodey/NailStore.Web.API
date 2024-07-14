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
    /// <returns>Возвращает кортеж, где "Result" - сообщение о результате операции и "Validator" - объект содержащий код выполнения операции и сообщение об ошибке, если таковая есть</returns>
    Task<ResponseModelCore> RegisterUserAsync(string url, string userName, string email, string password);
    Task<ResponseModelCore> LoginUserAsync(string email, string password);
    Task<ResponseModelCore> GetUserByIdAsync(string id);
    /// <summary>
    ///  Подтверждение Email пользователя
    /// </summary>
    /// <param name="userConfirmited">модель подтверждения Email</param>
    /// <returns></returns>
    Task<ResponseModelCore> ConfirmedEmailUser(UserConfirmitedEmail userConfirmited);
    /// <summary>
    /// Проверяет свободно ли имя пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <returns>Вернет <b>true</b>, если имя свободно и <b>false</b>, если не свободно</returns>
    Task<bool> UserNameIsFreeAsync(string username);
}
