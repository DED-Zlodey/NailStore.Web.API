namespace NailStore.Application.Settings;

public class EmailSettingsModel
{
    /// <summary>
    /// Наименование организации. Пишется в в поле от кого при отправке почты, при регистрации нового аккаунта. Задается в файле настроек appsettings.json
    /// </summary>
    public string? OrgName { get; set; }
    /// <summary>
    /// Адрес почтового ящика с которого будет отправляться корреспонденция. Задается в файле настроек appsettings.json
    /// </summary>
    public string? EmailOrg { get; set; }
    /// <summary>
    /// Пароль к почтовому ящику с которого будет отправляться корреспонденция. Задается в файле настроек appsettings.json
    /// </summary>
    public string? EmailPass { get; set; }
    /// <summary>
    /// Адрес SMTP сервера, который используется при отправке коореспонденции. Задается в файле настроек appsettings.json
    /// </summary>
    public string? SMTPHost { get; set; }
    /// <summary>
    /// Порт SMTP сервера, который используется при отправке коореспонденции. Задается в файле настроек appsettings.json
    /// </summary>
    public int SMTPPort { get; set; }
    /// <summary>
    /// Используется ли защищенный протокол при отправке коореспонденции. Задается в файле настроек appsettings.json
    /// </summary>
    public bool UseSSL { get; set; }
}