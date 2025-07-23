using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MsAuthentication.Models;
using MsAuthentication.Config;
using MsAuthentication.Data; // DbContext
using MsAuthentication.DTOs; // Seus DTOs
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace MsAuthentication.Services
{
    public class AuthService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly IMemoryCache _cache;
        private readonly AuthDbContext _context;
        private readonly EmailService _emailService;

        public AuthService(JwtConfig jwtConfig, IMemoryCache cache, AuthDbContext context, EmailService emailService)
        {
            _jwtConfig = jwtConfig;
            _cache = cache;
            _context = context;
            _emailService = emailService;
        }

        // Geração de token JWT
        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenId = Guid.NewGuid().ToString(); // ID único do token (jti)

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, tokenId),
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
        public string GenerateRefreshToken(string email)
        {
            var randomNumber = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(randomNumber);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(7));

            _cache.Set(refreshToken, email, cacheEntryOptions);

            return refreshToken;
        }

        public bool ValidateRefreshToken(string refreshToken, string email)
        {
            var normalizedEmail = email.Trim().ToLower();
            if (_cache.TryGetValue(refreshToken, out string? cachedEmail) && !string.IsNullOrEmpty(cachedEmail))
            {
                return cachedEmail.Trim().ToLower() == normalizedEmail;
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
                return false;
            }
        }

        // Registro de usuário
        public async Task<IdentityResult> RegisterUserAsync(RegisterRequestDto request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail))
                return IdentityResult.Failed(new IdentityError { Description = "Email já registrado." });

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = normalizedEmail,
                PasswordHash = hashedPassword,
                Role = request.Role.ToString()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return IdentityResult.Success;
        }

        // Login de usuário
        public async Task<TokenResponse?> LoginUserAsync(LoginRequestDto request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
            if (user == null)
                return null;

            bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!validPassword)
                return null;

            var jwtToken = GenerateToken(user);
            var refreshToken = GenerateRefreshToken(user.Email);

            return new TokenResponse
            {
                Token = jwtToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLower();

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (user == null)
                return false;

            var token = GeneratePasswordResetToken(user.Email);
            var resetLink = $"http://localhost:3000/reset-password?email={normalizedEmail}&token={token}";


             string body = $@"
    <div style='
        font-family: Arial, sans-serif; 
        max-width: 420px; 
        margin: 0 auto; 
        border: 1px solid #ececec; 
        border-radius: 8px; 
        background: #faf6ff; 
        padding: 24px;
        box-shadow: 0 2px 8px #eee;'>
        <h2 style='color: #7b32bf;'>Recuperação de Senha</h2>
        <p>Olá!</p>
        <p>Recebemos uma solicitação para redefinir a sua senha.</p>
        <p>Para criar uma nova senha, clique no botão abaixo:</p>
        <a href='{resetLink}' style='
            display: inline-block;
            background: linear-gradient(90deg, #b98cff 0%, #7b32bf 100%);
            color: #fff;
            text-decoration: none;
            font-weight: bold;
            padding: 12px 28px;
            border-radius: 5px;
            margin: 20px 0;
            box-shadow: 0 2px 8px #ddd;
            letter-spacing: 1px;'>
            Mudar Senha
        </a>
        <p style='font-size: 13px; color: #888;'>
            Se você não solicitou esta alteração, pode ignorar este e-mail.<br>
            Este link expira em 1 hora.
        </p>
        <hr style='margin: 28px 0 10px 0; border: none; border-top: 1px solid #eee;'/>
        <p style='font-size: 12px; color: #bbb;'>
            Este e-mail foi enviado automaticamente. Por favor, não responda.
        </p>
    </div>";


            try
            {
                _emailService.SendEmail(normalizedEmail, "Recuperação de Senha", body);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Resetar senha com token
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();

            if (!ValidatePasswordResetToken(normalizedEmail, request.Token))
                return IdentityResult.Failed(new IdentityError { Description = "Token inválido ou expirado." });

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            RemovePasswordResetToken(request.Token);

            return IdentityResult.Success;
        }

        // Refresh token
        public async Task<TokenResponse?> RefreshTokenAsync(string email, string refreshToken)
        {
            var normalizedEmail = email.Trim().ToLower();

            if (!ValidateRefreshToken(refreshToken, normalizedEmail))
                return null;

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
            if (user == null)
                return null;

            var newJwtToken = GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken(normalizedEmail);

            RevokeRefreshToken(refreshToken);

            return new TokenResponse
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            };
        }

        // Logout
        public async Task LogoutAsync(string email)
        {
            await Task.CompletedTask;
        }

        // Auxiliares para token de recuperação de senha
        private string GeneratePasswordResetToken(string email)
        {
            var normalizedEmail = email.Trim().ToLower();
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
            _cache.Set(token, normalizedEmail, options);
            return token;
        }

        private bool ValidatePasswordResetToken(string email, string token)
        {
            var normalizedEmail = email.Trim().ToLower();
            if (_cache.TryGetValue(token, out string? cachedEmail) && !string.IsNullOrEmpty(cachedEmail))
                return cachedEmail.Trim().ToLower() == normalizedEmail;


            return false;
        }

        private void RemovePasswordResetToken(string token)
        {
            _cache.Remove(token);
        }
    }

    // Classes auxiliares para resultados e erros
    public class IdentityResult
    {
        public bool Succeeded { get; set; }
        public IdentityError[] Errors { get; set; } = Array.Empty<IdentityError>();

        public static IdentityResult Success => new IdentityResult { Succeeded = true };

        public static IdentityResult Failed(params IdentityError[] errors)
        {
            return new IdentityResult { Succeeded = false, Errors = errors };
        }
    }

    public class IdentityError
    {
        public string Description { get; set; } = string.Empty;
    }

    // DTO de resposta do token
    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
