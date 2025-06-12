namespace MsAuthentication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        // Construtor usado manualmente no código
        public User(string Email, string passwordHash, string role = "User")
        {
            this.Email = Email;
            PasswordHash = passwordHash;
            Role = role;
        }

        // Construtor vazio exigido pelo EF Core
        public User() { }
    }
}
