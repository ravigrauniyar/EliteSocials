using System.Text.Json.Serialization;

namespace EliteSocials.Models
{
    public class UserViewModel
    {
        public Guid userId { get; set; }
        public string username { get; set; } = string.Empty;
        public string fullName { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public bool isTFAEnabled { get; set; }
    }
}
