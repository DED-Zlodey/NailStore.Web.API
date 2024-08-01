using System.IdentityModel.Tokens.Jwt;
using NailStore.Application.Interfaces;

namespace NailStore.Web.API.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Получить нагрузку JWT токена
        /// </summary>
        /// <param name="context">Контекст</param>
        /// <param name="_jwtService">Сервис JWT токенов</param>
        /// <returns></returns>
        public static JwtSecurityToken? GetPayloadForTokenAsync(this HttpContext context, IJWTManager _jwtService)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out var bearer))
            {
                string token = bearer!;
                token = token.Replace("Bearer", "").Replace(" ", "");
                if (!string.IsNullOrEmpty(token))
                {
                    var jwtToken = _jwtService.GetPayloadToken(token);
                    return jwtToken;
                }
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}
