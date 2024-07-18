using NailStore.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace NailStore.Web.API.Extensions
{
    public static class HttpContextExtensions
    {
        public static JwtSecurityToken GetPayloadForTokenAsync(this HttpContext context, IJWTManager _jwtService)
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
