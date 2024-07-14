using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NailStore.Application.Settings;
using NailStore.Data.Models;
using System.Text;

namespace NailStore.Web.API;

public class AppInit
{
    /// <summary>
    /// Инициализация БД начальными данными
    /// </summary>
    /// <param name="service">Сервис-провайдер</param>
    /// <returns></returns>
    public static async Task InitializeAsync(IServiceProvider service)
    {
        var _logger = service.GetRequiredService<ILogger<AppInit>>();
        var settings = service.GetService<IOptions<SrvSettings>>()!.Value;
        var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = service.GetRequiredService<UserManager<UserEntity>>();
        if (ValidationSectionAdmin(settings))
        {
            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                var res = await roleManager.CreateAsync(new IdentityRole("Admin"));
                if (res.Succeeded)
                {
                    _logger.LogInformation("{InitializeAsync}: Создана роль администратора: \"Admin\"", nameof(InitializeAsync));
                }
                else
                {
                    _logger.LogError("{InitializeAsync}: Не удалось создать роль администратора. Reason: {errorString}", nameof(InitializeAsync), GetIdentityErrorString(res.Errors));
                }
            }
            if (await roleManager.FindByNameAsync("User") == null)
            {
                var res = await roleManager.CreateAsync(new IdentityRole("User"));
                if (res.Succeeded)
                {
                    _logger.LogInformation("{InitializeAsync}: Создана роль пользователя: \"User\"", nameof(InitializeAsync));
                }
                else
                {
                    _logger.LogError("{InitializeAsync}: Не удалось создать роль пользователя. Reason: {errorString}", nameof(InitializeAsync), GetIdentityErrorString(res.Errors));
                }
            }
            if (await userManager.FindByEmailAsync(settings.Admin!.Email) == null)
            {
                var user = new UserEntity
                {
                    UserName = settings.Admin.UserName,
                    Email = settings.Admin.Email,
                    EmailConfirmed = true,
                    RegisterAt = DateTime.Now.ToUniversalTime(),
                    PhoneNumberConfirmed = true,
                    Enable = true,
                };
                IdentityResult result = await userManager.CreateAsync(user, settings.Admin.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("{InitializeAsync}: Создан пользователь: {Email}", nameof(InitializeAsync), settings.Admin!.Email);
                    var res = await userManager.AddToRoleAsync(user, "Admin");
                    if (res.Succeeded)
                    {
                        _logger.LogInformation("{InitializeAsync}: Роль: \"Admin\" присвоена пользователю: {Email}", nameof(InitializeAsync), settings.Admin!.Email);
                    }
                    else
                    {
                        _logger.LogError("{InitializeAsync}: Не удалось присвоить роль: \"Admin\", пользовтаелю: {Email}. Reason: {errorString}", nameof(InitializeAsync), settings.Admin!.Email, GetIdentityErrorString(result.Errors));
                    }
                }
                else
                {
                    _logger.LogError("{InitializeAsync}: Не удалось создать пользователя: {Email}. Reason: {errorString}", nameof(InitializeAsync), settings.Admin!.Email, GetIdentityErrorString(result.Errors));
                }
            }
        }
        else
        {
            _logger.LogError("{InitializeAsync}: Ошибки в конфигурационном файле, в секции {Admin}", nameof(InitializeAsync), settings.Admin);
        }
        await Task.CompletedTask;
    }
    /// <summary>
    /// Валидация секции Admin из файла конфигурации
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    private static bool ValidationSectionAdmin(SrvSettings settings)
    {
        if (settings == null)
            return false;
        if (settings.Admin == null) return false;
        if (string.IsNullOrEmpty(settings.Admin.Password))
            return false;
        if (string.IsNullOrEmpty(settings.Admin.Email))
            return false;
        return true;
    }
    /// <summary>
    /// Получить все ошибки Identity в одну строку
    /// </summary>
    /// <param name="errors">Список ошибок</param>
    /// <returns>Верент все ошибки в одну строку</returns>
    private static string GetIdentityErrorString(IEnumerable<IdentityError> errors)
    {
        StringBuilder sb = new StringBuilder();
        int counter = 0;
        foreach (var error in errors)
        {
            if (!string.IsNullOrEmpty(error.Description))
            {
                if (counter != errors.Count() - 1)
                {
                    sb.Append($"{error.Description}, ");
                }
                else
                {
                    sb.Append($"{error.Description}");
                }
            }
            counter++;
        }
        return sb.ToString();
    }
}
