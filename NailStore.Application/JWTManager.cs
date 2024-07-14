using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NailStore.Application.Settings;
using NailStore.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NailStore.Application;

public class JWTManager : IJWTManager
{
    private readonly ILogger<JWTManager> _logger;
    private readonly SrvSettings _settings;
    public JWTManager(ILogger<JWTManager> logger, IOptions<SrvSettings> srvSettings)
    {
        _logger = logger;
        _settings = srvSettings.Value;
    }
    public ClaimsPrincipal GetPrincipal(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = GetJwtToken(token);
            if (jwtToken == null)
            {
                return null;
            }
            if (!jwtToken.Header.Alg.Equals("HS256"))
            {
                return null;
            }
            if (!jwtToken.Header.Typ.Equals("JWT"))
            {
                return null;
            }
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                IssuerSigningKey = GetSymmetricSecurityKey(_settings.ServerKey!),
                ValidateIssuerSigningKey = true,
                LifetimeValidator = LifetimeValidator,
                ValidIssuer = "Master-Guard",
                ValidAudience = "companyGuard"
            };
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{method}: Что-то пошло не так. JWT Token: {token} Reason: {message}", nameof(GetPrincipal), token, ex.Message);
            return null;
        }
    }
    private bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
    {
        if (expires != null)
        {
            if (DateTime.Now.ToUniversalTime() < expires.Value.ToUniversalTime()) return true;
        }
        return false;
    }
    public JwtSecurityToken GetPayloadToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = GetJwtToken(token);
        if (jwtToken == null)
        {
            _logger.LogError("{method}: Не удалось получить payloads в JWT Token: {token}", nameof(GetPayloadToken), token);
            return null;
        }
        return jwtToken;
    }
    public JwtSecurityToken GetJwtToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadToken(token!) as JwtSecurityToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{method}: Не удалось прочитать JWT Token: {token} Reason: {message}", nameof(GetJwtToken), token, ex.Message);
            return null;
        }
    }
    public SymmetricSecurityKey GetSymmetricSecurityKey(string key)
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
    }
    /// <summary>
    /// Получить токен
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Возвращает мастер-токен</returns>
    public string GetBearerToken(IdentityUser user)
    {
        var identity = GetIdentityCaimsAsync(user);
        var now = DateTime.Now.ToUniversalTime();
        var jwt = new JwtSecurityToken(
                issuer: "Master-Guard",
                audience: "companyGuard",
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(1440)),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(_settings.ServerKey!), SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    private ClaimsIdentity GetIdentityCaimsAsync(IdentityUser user)
    {
        var claims = new List<Claim>
                {
                    new Claim("Id", user.Id),
                    new Claim("Roles", "User")
                };
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");
        return claimsIdentity;
    }
}
