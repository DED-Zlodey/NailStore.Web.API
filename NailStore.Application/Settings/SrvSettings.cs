namespace NailStore.Application.Settings;

public class SrvSettings
{
    public AdminModel? Admin { get; set; }
    public EmailSettingsModel? EmailSettings { get; set; }
    public string? ServerKey { get; set; }
    public string[]? CorsHosts { get; set; }
}
