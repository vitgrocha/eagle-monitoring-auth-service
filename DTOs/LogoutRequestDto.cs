using System.ComponentModel.DataAnnotations;

namespace MsAuthentication.DTOs
{
    public class LogoutRequestDto
    {
        [Required(ErrorMessage = "O refresh token é obrigatório.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
