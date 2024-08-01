using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NailStore.Application.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NailStore.Core.Interfaces;
using NailStore.Data.Models;

namespace NailStore.Application;

public class JWTManager : IJWTManager
{
    private readonly ILogger<JWTManager> _logger;
    private readonly UserManager<UserEntity> _userManager;
    private readonly SrvSettings _settings;
    public JWTManager(ILogger<JWTManager> logger, IOptions<SrvSettings> srvSettings, UserManager<UserEntity> userManager)
    {
        _logger = logger;
        _userManager = userManager;
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
                return null!;
            }
            if (!jwtToken.Header.Alg.Equals("HS256"))
            {
                return null!;
            }
            if (!jwtToken.Header.Typ.Equals("JWT"))
            {
                return null!;
            }
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                IssuerSigningKey = GetSymmetricSecurityKey(_settings.ServerKey!),
                ValidateIssuerSigningKey = true,
                LifetimeValidator = LifetimeValidator,
                ValidIssuer = "NailStoreApi",
                ValidAudience = "NailStore.Company"
            };
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{method}: Что-то пошло не так. JWT Token: {token} Reason: {message}", nameof(GetPrincipal), token, ex.Message);
            return null!;
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
    public JwtSecurityToken? GetPayloadToken(string token)
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
            return null!;
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
    public async Task<string> GetBearerTokenAsync(string userId)
    {
        var identity = await GetIdentityCaimsAsync(userId);
        var now = DateTime.Now;
        var jwt = new JwtSecurityToken(
                issuer: "NailStoreApi",
                audience: "NailStore.Company",
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(1440)),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(_settings.ServerKey!), SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    public async Task<ClaimsIdentity> GetIdentityCaimsAsync(string userId)
    {
        var claims = new List<Claim>
        { 
            new("Id", userId), 
        };
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ClaimsIdentity(claims, "Token");
        }
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            claims.Add(new Claim("Role", role));
        }
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");
        return claimsIdentity;
    }
}
