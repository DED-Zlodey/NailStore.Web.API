using NailStore.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NailStore.Core.Interfaces
{
    public interface IJWTManager
    {
        /// <summary>
        /// Проверка JWT токена
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        ClaimsPrincipal GetPrincipal(string token);
        /// <summary>
        /// Получить JwtSecurityToken из строки
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        JwtSecurityToken GetJwtToken(string token);
        /// <summary>
        /// Получить токен для пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        Task<string> GetBearerTokenAsync(UserEntity user);
        /// <summary>
        /// Получить полезную нагрузку из токена
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        JwtSecurityToken GetPayloadToken(string token);
    }
}
