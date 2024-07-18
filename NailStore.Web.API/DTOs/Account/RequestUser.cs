using System.ComponentModel.DataAnnotations;

namespace NailStore.Web.API.DTOs.Account
{
    /// <summary>
    /// Объект пользователя
    /// </summary>
    public class RequestUser
    {
        /// <summary>
        /// Ссылка для формирования колбэк ссылки (https://localhost/api/Account/ConfirmedEmail/) "/" - обязательно в конце!
        /// В итоге ссылка будет иметь такой вид: https://localhost/api/Account/ConfirmedEmail/{userId}/{code}
        /// </summary>
        public string? Url { get; set; }
        /// <summary>
        /// Никнейм пользователя
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// Email пользователя
        /// </summary>
        [EmailAddress]
        public required string Email { get; set; }
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
