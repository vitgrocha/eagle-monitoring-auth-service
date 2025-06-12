using System.ComponentModel.DataAnnotations;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
    public required string Email { get; set; }

}
