namespace NailStore.Core.Models;

public class UserConfirmitedEmail
{
    public string UserId { get; }
    public string Code { get; }

    public UserConfirmitedEmail(string userId, string code)
    {
        Code = code;
        UserId = userId;
    }

    public static (UserConfirmitedEmail ConfirmModel, CommonValidator Validator) Create(string userId, string code)
    {
        var validator = new CommonValidator
        {
            Error = string.Empty,
            StatusCode = 200
        };
        if (!IsGuid(userId))
        {
            validator.Error = $" \"userId\" не является идентификатором пользователя";
            validator.StatusCode = 400;
        }
        if (string.IsNullOrEmpty(code))
        {
            validator.Error = $" поле \"code\" не может быть пустым";
            validator.StatusCode = 400;
        }
        var confirmed = new UserConfirmitedEmail(userId, code);
        return (confirmed, validator);
    }
    /// <summary>
    /// Проверяет является ли строка гуидом
    /// </summary>
    /// <param name="userId">идентификатор пользователя</param>
    /// <returns>Верент <b>true</b>,  если строка является гуидом и <b>false</b>, если не является</returns>
    private static bool IsGuid(string userId)
    {
        try
        {
            if (Guid.TryParse(userId, out Guid guid))
            {
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
