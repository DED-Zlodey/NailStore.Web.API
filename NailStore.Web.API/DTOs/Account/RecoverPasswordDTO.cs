namespace NailStore.Web.API.DTOs.Account
{
    /// <summary>
    /// Модель установки нового пароля
    /// </summary>
    public class RecoverPasswordDTO
    {
        /// <summary>
        /// Идентификатор пользователя для аккаунта которого устанавливается новый пароль
        /// </summary>
        public required Guid UserId { get; set; }
        /// <summary>
        /// Код отправленный на почту для установки нового пароля
        /// </summary>
        public required string Token { get; set; }
        /// <summary>
        /// Новый пароль
        /// </summary>
        public required string Password { get; set; }
    }
}
