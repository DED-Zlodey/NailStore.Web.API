namespace NailStore.Core.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Отправкить электронное письмо
        /// </summary>
        /// <param name="email">Адрес получателя</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="body">Тело письма</param>
        /// <returns></returns>
        Task<bool> SendEmailAsync(string email, string subject, string body);
    }
}
