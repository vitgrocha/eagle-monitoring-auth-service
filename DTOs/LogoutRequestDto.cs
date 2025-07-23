using System.ComponentModel.DataAnnotations;

namespace MsAuthentication.DTOs
{
    public class LogoutRequestDto
    {
        [Required(ErrorMessage = "O refresh token é obrigatório.")]
        public string RefreshToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;
    }
}
