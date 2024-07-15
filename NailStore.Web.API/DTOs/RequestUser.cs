using System.ComponentModel.DataAnnotations;

namespace NailStore.Web.API.DTOs
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
        public string Url { get; set; }
        /// <summary>
        /// Никнейм пользователя
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// Email пользователя
        /// </summary>
        public string Email { get; set; } = default!;
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
}
