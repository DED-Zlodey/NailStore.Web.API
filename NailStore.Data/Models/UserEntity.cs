using Microsoft.AspNetCore.Identity;

namespace NailStore.Data.Models;

public class UserEntity : IdentityUser
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string FirstName { get; set; }
    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    public string LastName { get; set; }
    /// <summary>
    /// Дата регистрации пользователя
    /// </summary>
    public DateTime RegisterAt { get; set; }
    /// <summary>
    /// Активна ли учетная запись (true - активна, false - отключена (пользователь не может выполнять ни какие операции))
    /// </summary>
    public bool Enable { get; set; }
}
