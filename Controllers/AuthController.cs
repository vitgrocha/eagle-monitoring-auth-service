using Microsoft.AspNetCore.Mvc;
using MsAuthentication.Data;
using MsAuthentication.Models;
using MsAuthentication.DTOs;
using MsAuthentication.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MsAuthentication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly AuthService _authService;
        private readonly IMemoryCache _cache;
        private readonly TwoFactorService _twoFactorService;
        private readonly EmailService _emailService;

        public AuthController(
            AuthDbContext context,
            AuthService authService,
            IMemoryCache cache,
            TwoFactorService twoFactorService,
            EmailService emailService)
        {
            _context = context;
            _authService = authService;
            _cache = cache;
            _twoFactorService = twoFactorService;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDto userDto)
        {
            if (_context.Users.Any(u => u.Email == userDto.Email))
                return BadRequest("Não é possível utilizar este usuário!");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            var user = new User(userDto.Email, hashedPassword, "User");

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Usuário registrado com sucesso!");
        }

        // ... (seus outros endpoints)

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] LogoutRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest("Refresh token não fornecido!");

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwtToken = null;

            try
            {
                jwtToken = handler.ReadJwtToken(request.RefreshToken);
            }
            catch
            {
                return BadRequest("Refresh token inválido!");
            }

            var tokenId = jwtToken?.Id;
            if (string.IsNullOrEmpty(tokenId))
                return BadRequest("Token inválido!");

            _authService.InvalidateToken(tokenId);

            return Ok("Logout bem-sucedido!");
        }

        [AllowAnonymous]
        [HttpPost("token/refresh")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken) || string.IsNullOrEmpty(request.Email))
                return BadRequest("Email e refresh token são obrigatórios.");

            if (!_authService.ValidateRefreshToken(request.RefreshToken, request.Email))
                return Unauthorized("Refresh token inválido!");

            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
                return NotFound("Usuário não encontrado!");

            var newJwtToken = _authService.GenerateToken(user);
            var newRefreshToken = _authService.GenerateRefreshToken(user.Email);

            _authService.RevokeRefreshToken(request.RefreshToken);

            return Ok(new
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

        // ... (demais endpoints permanecem iguais)
    }

    // DTOs inline que você tinha no final (pode mover para arquivos separados)
    public class TwoFactorCodeRequest
    {
        public required string Email { get; set; }
        public required string Code { get; set; }
    }

    public class EmailTestDto
    {
        public required string ToEmail { get; set; }
    }
}
