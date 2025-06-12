using System.ComponentModel.DataAnnotations;

public class TokenDto
{
    [Required(ErrorMessage = "O token é obrigatório.")]
    public string Token { get; set; } = string.Empty;
}
