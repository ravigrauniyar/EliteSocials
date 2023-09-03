using EliteSocials.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace EliteSocials.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly SessionHandler<UserViewModel> _sessionHandler;
        public BaseController(IHttpContextAccessor httpContextAccessor, SessionHandler<UserViewModel> sessionHandler)
        {
            _contextAccessor = httpContextAccessor;
            _sessionHandler = sessionHandler;
        }
        public void ClearSession()
        {
            _contextAccessor.HttpContext!.Session.Remove("JwtToken");
            _contextAccessor.HttpContext!.Session.Remove("Username");
            _contextAccessor.HttpContext!.Session.Remove("UserId");
            _contextAccessor.HttpContext!.Session.Remove("IsLoggedIn");
            _contextAccessor.HttpContext!.Session.Remove("IsTFAEnabled");

            _sessionHandler.ResetSessionVariables();
        }
        public void InitializeSession(HttpResponseMessage responseMessage)
        {
            _sessionHandler.SetSessionVariables(responseMessage);

            var jwtToken = AccessToken();

            // Set session variables upon successful login
            _contextAccessor.HttpContext!.Session.SetString("JwtToken", jwtToken);
        }
        public string AccessToken()
        {
            return _sessionHandler.GetJwtToken();
        }
        public UserViewModel AccessUser()
        {
            return _sessionHandler.GetUser();
        }
    }
}
