using System.ComponentModel.DataAnnotations;

namespace MsAuthentication.DTOs
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha antiga é obrigatória.")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A nova senha deve ter pelo menos 6 caracteres.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
