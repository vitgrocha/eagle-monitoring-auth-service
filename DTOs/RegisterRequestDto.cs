using System.ComponentModel.DataAnnotations;
using MsAuthentication.Enums;

namespace MsAuthentication.DTOs
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        public UserRole Role { get; set; }
    }
}
