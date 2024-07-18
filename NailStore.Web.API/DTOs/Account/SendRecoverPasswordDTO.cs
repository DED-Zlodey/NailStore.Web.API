using System.ComponentModel.DataAnnotations;

namespace NailStore.Web.API.DTOs.Account
{
    /// <summary>
    /// Модель отправки запроса на восстановление пароля
    /// </summary>
    public class SendRecoverPasswordDTO
    {
        /// <summary>
        /// Ссылка для формирования колбэк ссылки (https://localhost/api/Account/ConfirmedEmail/) "/" - обязательно в конце!
        /// В итоге ссылка будет иметь такой вид: https://localhost/api/Account/ConfirmedEmail/{userId}/{code}
        /// </summary>
        public string? Url { get; set; }
        /// <summary>
        /// Email  адрес аккаунта  пароль для которого требуется восстановить
        /// </summary>
        [EmailAddress]
        public required string Email { get; set; }
    }
}
