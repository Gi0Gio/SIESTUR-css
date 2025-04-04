using System.Globalization;

namespace Turnero.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class AuthDto
    {
        public string Username { get; set; }

        public string PasswordHash { get; set; }
    }
}
