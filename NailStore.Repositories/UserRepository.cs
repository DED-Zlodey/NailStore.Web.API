using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NailStore.Core.Interfaces;
using NailStore.Core.Models;
using NailStore.Data;

namespace NailStore.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    /// <summary>
    /// Получить время ограничения входа пользователя
    /// </summary>
    /// <param name="email">Email пользователя</param>
    /// <returns>Возвращает модель ответа</returns>
    public async Task<ResponseModelCore> GetLockOutTimeUser(string email)
    {
        var user = await _context.Users.Select(x => new { x.Email, x.LockoutEnd }).AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        if (user == null)
        {
            _logger.LogError("{method} Пользователь {email} не найден", nameof(GetLockOutTimeUser), email);
            return new ResponseModelCore
            {
                Header = new ResponseHeaderCore
                {
                    Error = $"Пользователь {email} не найден",
                    StatusCode = 404,
                }
            };
        }
        return new ResponseModelCore
        {
            Header = new ResponseHeaderCore
            {
                Error = string.Empty,
                StatusCode = 200,
            },
            Body = new ResponseBodyCore
            {
                LockedOutTime = user.LockoutEnd
            }
        };
    }
    public async Task<ResponseModelCore> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.Select(x => new { x.Id, x.UserName, x.PhoneNumber, x.Enable, x.RegisterAt }).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
        {
            _logger.LogError("{method} Пользователь с идентификатором {id} не найден", nameof(GetUserByIdAsync), id);
            return new ResponseModelCore { Header = new ResponseHeaderCore { Error = $"Пользователь с идентификатором {id} не найден.", StatusCode = 404 } };
        }
        return UserIdentityCoreModel.CreateUser(user!.Id, user.UserName!, user.RegisterAt, user.PhoneNumber, user.Enable);
    }
    /// <summary>
    /// Получить пользователя по Email
    /// </summary>
    /// <param name="email">Email пользователя</param>
    /// <returns>Вернет пользователя или <b>null</b></returns>
    public async Task<ResponseModelCore> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user == null)
        {
            _logger.LogError("{method} Не удалось получить пользователя {email}", nameof(GetUserByEmailAsync), email);
            return new ResponseModelCore 
            {
                Header = new()
                {
                    Error = "Не удалось получить пользователя",
                    StatusCode = 404
                }
            };
        }
        return UserIdentityCoreModel.CreateUser(user!.Id, user.UserName!, user.RegisterAt, user.PhoneNumber, user.Enable);
    }
    /// <summary>
    /// Проверяет свободно ли имя пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <returns>Вернет <b>true</b>, если имя свободно и <b>false</b>, если не свободно</returns>
    public async Task<bool> UserNameIsFreeAsync(string username)
    {
        var user = await _context.Users.Select(x => new IdentityUser<Guid>
        {
            UserName = x.UserName,
            Id = x.Id,
        }).FirstOrDefaultAsync(x => x.UserName == username);
        if (user == null)
        {
            return true;
        }
        return false;
    }
}
