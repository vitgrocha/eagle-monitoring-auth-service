using System.ComponentModel.DataAnnotations;

namespace MsAuthentication.DTOs
{
    public class ResetPasswordRequestDto
    {
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O token é obrigatório.")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
