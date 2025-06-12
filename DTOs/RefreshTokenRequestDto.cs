namespace MsAuthentication.DTOs
{
    public class RefreshTokenRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}