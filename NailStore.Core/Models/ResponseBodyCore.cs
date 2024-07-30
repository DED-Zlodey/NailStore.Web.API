using NailStore.Core.Models.ResponseModels.Services;

namespace NailStore.Core.Models;

/// <summary>
/// Представляет тело ответа для различных основных операций в приложении NailStore.
/// </summary>
public class ResponseBodyCore
{
    /// <summary>
    /// Возвращает или устанавливает токен аутентификации для пользователя.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Возвращает или устанавливает сообщение, связанное с ответом.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Возвращает или устанавливает информацию об идентификаторе пользователя.
    /// </summary>
    public UserIdentityCoreModel? User { get; set; }

    /// <summary>
    /// Возвращает или устанавливает время, когда учетная запись пользователя заблокирована из-за чрезмерных неудачных попыток входа.
    /// </summary>
    public DateTimeOffset? LockedOutTime { get; set; }

    /// <summary>
    /// Возвращает или устанавливает данные ответа для получения услуг.
    /// </summary>
    public ResponseGetServiceModelCore GetServices { get; set; }
}