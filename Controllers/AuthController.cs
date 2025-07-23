using Microsoft.AspNetCore.Mvc;
using MsAuthentication.DTOs;
using MsAuthentication.Services;

namespace MsAuthentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            request.Email = request.Email.Trim().ToLower();
            var result = await _authService.RegisterUserAsync(request);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Usuário registrado com sucesso.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            request.Email = request.Email.Trim().ToLower();
            var tokenResponse = await _authService.LoginUserAsync(request);
            if (tokenResponse == null)
                return Unauthorized("Email ou senha inválidos.");

            return Ok(tokenResponse);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var email = request.Email.Trim().ToLower();
            var result = await _authService.SendPasswordResetEmailAsync(email);
            if (!result)
                return NotFound("Usuário não encontrado.");

            return Ok("Email de recuperação enviado com sucesso.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            request.Email = request.Email.Trim().ToLower();
            var result = await _authService.ResetPasswordAsync(request);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Senha redefinida com sucesso.");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            request.Email = request.Email.Trim().ToLower();
            var result = await _authService.RefreshTokenAsync(request.Email, request.RefreshToken);
            if (result == null)
                return Unauthorized("Token inválido ou expirado.");

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
        {
            request.Email = request.Email.Trim().ToLower();
            await _authService.LogoutAsync(request.Email);
            return Ok("Logout realizado com sucesso.");
        }
    }
}
