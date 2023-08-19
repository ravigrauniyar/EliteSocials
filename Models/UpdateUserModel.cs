using System.Text.Json.Serialization;

namespace EliteSocials.Models
{
    public class UpdateUserModel
    {
        public string username { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string fullName { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string email { get; set; } = string.Empty;
    }
}
