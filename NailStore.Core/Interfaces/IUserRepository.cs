using NailStore.Core.Models;

namespace NailStore.Core.Interfaces;

public interface IUserRepository
{
    /// <summary>
    /// Получить время ограничения входа пользователя
    /// </summary>
    /// <param name="email">Email пользователя</param>
    /// <returns>Возвращает модель ответа</returns>
    Task<ResponseModelCore> GetLockOutTimeUser(string email);
    /// <summary>
    /// Получить пользователя по его идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Возвращает модель ответа</returns>
    Task<ResponseModelCore> GetUserByIdAsync(Guid id);
    /// <summary>
    /// Получить пользователя по его Email
    /// </summary>
    /// <param name="email">Email пользователя</param>
    /// <returns>Возвращает модель ответа</returns>
    Task<ResponseModelCore> GetUserByEmailAsync(string email);
    /// <summary>
    /// Проверяет свободно ли имя пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <returns>Вернет <b>true</b>, если имя свободно и <b>false</b>, если не свободно</returns>
    Task<bool> UserNameIsFreeAsync(string username);
}
