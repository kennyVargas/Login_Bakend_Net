namespace Apitransac.DTOs.Auth
{
    public class UserResponseDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
