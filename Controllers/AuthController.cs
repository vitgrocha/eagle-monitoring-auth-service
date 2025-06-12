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

        public AuthController(AuthDbContext context, AuthService authService, IMemoryCache cache)
        {
            _context = context;
            _authService = authService;
            _cache = cache;
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

        [HttpGet("users/{Email}")]
        public IActionResult GetUserInfo(string Email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null)
                return NotFound("Usuário não encontrado!");

            return Ok(new
            {
                Email = user.Email,
                Role = user.Role
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email e senha são obrigatórios.");
            }

            // Buscar usuário por email (ajustei para buscar por Email, como no seu exemplo)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Credenciais inválidas!");
            }

            var token = _authService.GenerateToken(user);
            var refreshToken = _authService.GenerateRefreshToken(user.Email);

            return Ok(new
            {
                Token = token,
                RefreshToken = refreshToken
            });
        }


        [HttpPut("users/{email}/password")]
        public IActionResult ChangePassword(string email, [FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return NotFound("Usuário não encontrado!");

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, user.PasswordHash))
                return Unauthorized("Senha antiga incorreta!");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            _context.SaveChanges();

            return Ok("Senha alterada com sucesso!");
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] TokenDto tokenDto)
        {
            if (tokenDto == null || string.IsNullOrEmpty(tokenDto.Token))
                return BadRequest("Token não fornecido!");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenDto.Token);
            var tokenId = jwtToken?.Id;

            if (string.IsNullOrEmpty(tokenId))
                return BadRequest("Token inválido!");

            // Chama o método do serviço para invalidar o token pelo tokenId (jti)
            _authService.InvalidateToken(tokenId);

            return Ok("Logout bem-sucedido!");
        }

        [AllowAnonymous]
        [HttpPost("password/forgot")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == forgotPasswordDto.Email);
            if (user == null)
                return NotFound("Usuário não encontrado!");

            // TODO: implementar envio de e-mail para recuperação de senha
            return Ok("Instruções para recuperação de senha enviadas por e-mail.");
        }

        [HttpGet("protected-data")]
        public IActionResult GetProtectedData([FromHeader] string authorization)
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
                return Unauthorized("Token não fornecido!");

            var token = authorization["Bearer ".Length..].Trim();

            if (_authService.IsTokenRevoked(token))
                return Unauthorized("Token revogado.");

            return Ok(new { Data = "Dados protegidos" });
        }

        [HttpPut("users/{Email}")]
        public IActionResult UpdateUser(string Email, [FromBody] UpdateUserDto updateUserDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null)
                return NotFound("Usuário não encontrado!");

            user.Email = updateUserDto.NewEmail ?? user.Email;
            user.Role = updateUserDto.NewRole ?? user.Role;

            _context.SaveChanges();

            return Ok("Dados do usuário atualizados com sucesso!");
        }

        [HttpPost("token/validate")]
        public IActionResult ValidateToken([FromBody] TokenDto tokenDto)
        {
            var isValid = _authService.ValidateToken(tokenDto.Token);

            if (!isValid)
                return Unauthorized("Token inválido!");

            return Ok("Token válido!");
        }

        [AllowAnonymous]
        [HttpPost("token/refresh")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
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
    }
}
