using Microsoft.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace EliteSocials.Models
{
    public class SessionHandler<UserViewModel>
    {
        private UserViewModel? user { get; set;}
        private string jwtToken { get; set; } = string.Empty;
        public async void SetSessionVariables(HttpResponseMessage message)
        {
            var response = await message.Content.ReadAsStringAsync();
            ServiceResult<string> tokenResponse = JsonSerializer.Deserialize<ServiceResult<string>>(response)!;
            jwtToken = tokenResponse.data.value;

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);
            string userClaimValue = token.Claims.FirstOrDefault(c => c.Type == "User")?.Value!;

            user = JsonSerializer.Deserialize<UserViewModel>(userClaimValue);
        }
        public void ResetSessionVariables()
        {
            user = default;
            jwtToken = string.Empty;
        }
        public UserViewModel GetUser()
        {
            return user!;
        }
        public string GetJwtToken()
        {
            return jwtToken;
        }
    }
}
