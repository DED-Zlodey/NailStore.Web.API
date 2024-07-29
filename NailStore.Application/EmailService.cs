using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using NailStore.Application.Settings;
using NailStore.Core.Interfaces;
using NailStore.Core.Models;

namespace NailStore.Application;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly string _orgName;
    private readonly string _emailOrg;
    private readonly bool _useSSL;
    private readonly string _emailPassword;
    private readonly int _portSmtpServer;
    private readonly ILogger<EmailService> _logger;
    private SmtpClient _smtpClient;

    public EmailService(IOptions<SrvSettings> options, ILogger<EmailService> logger, SmtpClient smtpClient)
    {
        _smtpServer = options.Value.EmailSettings!.SMTPHost!;
        _orgName = options.Value.EmailSettings.OrgName!;
        _emailOrg = options.Value.EmailSettings.EmailOrg!;
        _useSSL = options.Value.EmailSettings.UseSSL;
        _emailPassword = options.Value.EmailSettings.EmailPass!;
        _portSmtpServer = options.Value.EmailSettings.SMTPPort;
        _logger = logger;
        _smtpClient = smtpClient;
    }
    /// <summary>
    /// Отправка письма
    /// </summary>
    /// <param name="email">Адрес отправки</param>
    /// <param name="subject">Тема письма</param>
    /// <param name="body">Тело письма</param>
    /// <returns></returns>
    public async Task<EmailServiceResultModel> SendEmailAsync(string email, string subject, string body)
    {
        var response = new EmailServiceResultModel
        {
            IsSending = false,
            MessageResultSending = string.Empty,
        };
        try
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_orgName, _emailOrg));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = body
            };
            using (_smtpClient)
            {
                await _smtpClient.ConnectAsync(_smtpServer, _portSmtpServer, _useSSL);
                await _smtpClient.AuthenticateAsync(_emailOrg, _emailPassword);
                response.MessageResultSending = await _smtpClient.SendAsync(emailMessage);
                await _smtpClient.DisconnectAsync(true);
            }
            response.IsSending = true;
            return response;
        }
        catch (SmtpCommandException ex)
        {
            _logger.LogError(ex, "{method} Отправка письма по адресу: {address} не удалась. {statuscode} Reason: {message}", nameof(SendEmailAsync), email, ex.StatusCode, ex.Message);
            response.MessageResultSending = ex.Message;
            response.StatusCode = (short)ex.StatusCode;
            return response;
        }
    }
}
