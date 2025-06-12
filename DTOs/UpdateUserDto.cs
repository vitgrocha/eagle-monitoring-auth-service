using System.ComponentModel.DataAnnotations;

public class UpdateUserDto
{
    [Required(ErrorMessage = "O nome de usuário atual é obrigatório.")]
    public string Email { get; set; } = string.Empty;
    public string NewEmail { get; set; } = string.Empty;
    public string NewRole { get; set; } = string.Empty;
}
