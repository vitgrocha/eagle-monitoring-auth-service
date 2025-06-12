using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MsAuthentication.Models;
using MsAuthentication.Config;

namespace MsAuthentication.Services
{
    public class AuthService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly IMemoryCache _cache;

        public AuthService(JwtConfig jwtConfig, IMemoryCache cache)
        {
            _jwtConfig = jwtConfig;
            _cache = cache;
        }

        // Geração de token JWT
        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenId = Guid.NewGuid().ToString(); // 🔐 ID único do token (jti)

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, tokenId), // Adiciona jti
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Geração de refresh token aleatório
        public string GenerateRefreshToken(string Email)
        {
            var randomNumber = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(randomNumber);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(7)); // validade 7 dias

            _cache.Set(refreshToken, Email, cacheEntryOptions);

            return refreshToken;
        }

        // Validar refresh token
        public bool ValidateRefreshToken(string refreshToken, string Email)
        {
            if (_cache.TryGetValue(refreshToken, out string? cachedEmail))
            {
                return cachedEmail == Email;
            }
            return false;
        }

        // Revogar refresh token
        public void RevokeRefreshToken(string refreshToken)
        {
            _cache.Remove(refreshToken);
        }

        // Invalidar token JWT
        public void InvalidateToken(string tokenId)
        {
            _cache.Set(tokenId, "revoked", new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });
        }

        // Verificar se o token JWT foi revogado
        public bool IsTokenRevoked(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var tokenId = jwtToken?.Id;

            if (string.IsNullOrEmpty(tokenId))
                return false;

            return _cache.TryGetValue(tokenId, out _);
        }

        // Validar token JWT
        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtConfig.Issuer,
                    ValidAudience = _jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key))
                }, out _);

                return true;
            }
            catch
            {
                return false; // Token inválido
            }
        }
    }
}
