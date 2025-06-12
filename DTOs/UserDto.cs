using System.ComponentModel.DataAnnotations;

namespace MsAuthentication.DTOs
{
    public class UserDto
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}
