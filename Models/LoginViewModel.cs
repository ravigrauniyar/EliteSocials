using System.ComponentModel.DataAnnotations;

namespace EliteSocials.Models
{
    public class LoginViewModel
    {
        [Key]
        public string username { get; set; } = null!;
        public string password { get; set; } = null!;
    }
    public class Jwt
    {
        public string key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
    }
}
