using EliteSocials.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Net.Http.Headers;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EliteSocials.Controllers
{
    public class EliteSocialsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        readonly string baseURL = "http://localhost:5048/api/";

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            HttpContext.Session.Remove("IsLoggedIn");
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("UserId");

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginSignUpViewModel viewModel)
        {
            var loginModel = new LoginViewModel
            {
                username = viewModel.username,
                password = viewModel.password,
            };
            using var client = new HttpClient();

            HttpResponseMessage getData = await client.PostAsJsonAsync(baseURL + "User/login", loginModel);
            if (getData.IsSuccessStatusCode)
            {
                var jsonResponse = await getData.Content.ReadAsStringAsync();
                ServiceResult<string> tokenResponse = JsonSerializer.Deserialize<ServiceResult<string>>(jsonResponse)!;
                var jwtToken = tokenResponse.data.value;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                HttpResponseMessage profileResponse = await client
                                                    .GetAsync(baseURL + "User/profile");

                if (profileResponse.IsSuccessStatusCode)
                {
                    var profileJsonResponse = await profileResponse.Content.ReadAsStringAsync();
                    ServiceResult<UserViewModel> profileResult = JsonSerializer.Deserialize<ServiceResult<UserViewModel>>(profileJsonResponse)!;
                    var profile = profileResult.data.value;

                    HttpContext.Session.SetString("JwtToken", jwtToken);

                    if (profile.isTFAEnabled)
                    {
                        return RedirectToAction("TFA");
                    }

                    // Set session variables upon successful login
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("Username", profile.username);
                    HttpContext.Session.SetString("UserId", profile.userId.ToString());

                    return RedirectToAction("Index");
                }
                ViewData["LoginErrorMessage"] = "User not found!";
                return View();
            }
            ViewData["LoginErrorMessage"] = "Failed to login!";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(LoginSignUpViewModel viewModel)
        {
            var registerModel = new SignUpViewModel
            {
                username = viewModel.username,
                password = viewModel.password,
                rePassword = viewModel.rePassword,
                email = viewModel.email,
                fullName = viewModel.fullName
            };

            using var client = new HttpClient();

            HttpResponseMessage getData = await client.PostAsJsonAsync(baseURL + "User/register", registerModel);
            if (getData.IsSuccessStatusCode)
            {
                return await Login(viewModel);
            }
            else
            {
                ViewData["SignUpErrorMessage"] = "Invalid registration details!";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var authToken = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(authToken))
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                HttpResponseMessage profileResponse = await client
                                                    .GetAsync(baseURL + "User/profile");

                if (profileResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await profileResponse.Content.ReadAsStringAsync();
                    ServiceResult<UserViewModel> profileResult = JsonSerializer.Deserialize<ServiceResult<UserViewModel>>(jsonResponse)!;
                    var profile = profileResult.data.value;

                    return View(profile);
                }

                ViewData["ProfileMessage"] = "User not found!";
                return View();
            }
            else
            {
                ViewData["ProfileMessage"] = "User not authenticated!";
                return View();
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Update()
        {
            var authToken = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(authToken))
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                HttpResponseMessage profileResponse = await client
                                                    .GetAsync(baseURL + "User/profile");

                if (profileResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await profileResponse.Content.ReadAsStringAsync();
                    ServiceResult<UserViewModel> profileResult = JsonSerializer
                                    .Deserialize<ServiceResult<UserViewModel>>(jsonResponse)!;
                    
                    var profile = profileResult.data.value;
                    return View(profile);
                }
                ViewData["ProfileMessage"] = "User not found!";
                return View();
            }
            else
            {
                ViewData["ProfileMessage"] = "User not authenticated!";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserViewModel userViewModel)
        {
            var updateUserModel = new UpdateUserModel { 
                username = userViewModel.username,
                email = userViewModel.email,
                fullName = userViewModel.fullName
            };

            var authToken = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(authToken))
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                HttpResponseMessage getData = await client.PutAsJsonAsync(baseURL + "User/" + userViewModel.userId + "/update", updateUserModel);

                if (getData.IsSuccessStatusCode)
                {
                    var jsonResponse = await getData.Content.ReadAsStringAsync();
                    ServiceResult<UserViewModel> profileResult = JsonSerializer.Deserialize<ServiceResult<UserViewModel>>(jsonResponse)!;
                    var profile = profileResult.data.value;

                    return RedirectToAction("Profile");
                }
                else
                {
                    ViewData["UpdateMessage"] = getData.ReasonPhrase;
                    return View(userViewModel);
                }
            }
            else
            {
                ViewData["ProfileMessage"] = "User not authenticated!";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var authToken = HttpContext.Session.GetString("JwtToken");

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(authToken))
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                HttpResponseMessage getData = await client.DeleteAsync(baseURL + "User/" + userId + "/delete");
                if(getData.IsSuccessStatusCode)
                {
                    return Logout();
                }
                else
                {
                    ViewData["DeleteErrorMessage"] = "User not deleted!";
                    return RedirectToAction("Update");
                }
            }
            else
            {
                ViewData["DeleteErrorMessage"] = "User not authenticated!";
                return RedirectToAction("Update");
            }
        }

        [HttpGet]
        public async Task<IActionResult> TFA()
        {
            var authToken = HttpContext.Session.GetString("JwtToken");

            if (!string.IsNullOrEmpty(authToken))
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                HttpResponseMessage profileResponse = await client
                                                    .GetAsync(baseURL + "User/profile");

                if (profileResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await profileResponse.Content.ReadAsStringAsync();
                    ServiceResult<UserViewModel> profileResult = JsonSerializer.Deserialize<ServiceResult<UserViewModel>>(jsonResponse)!;
                    var profile = profileResult.data.value;

                    var isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");

                    if(profile.email == string.Empty)
                    {
                        ViewData["TFAMessage"] = "Email not set! Update profile.";
                        return View();
                    }

                    if (profile.isTFAEnabled && isLoggedIn == "true")
                    {
                        return RedirectToAction("Profile");
                    }
                }
                HttpResponseMessage getData = await client.GetAsync(baseURL + "User/EnableTFA");
                
                if (!getData.IsSuccessStatusCode)
                {
                    ViewData["TFAMessage"] = "Error! Two Factor Authentication not enabled!";
                }
                return View();
            }
            else
            {
                ViewData["TFAMessage"] = "User not authenticated!";
                return View();
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> TFA(string otp)
        {
            var authToken = HttpContext.Session.GetString("JwtToken");

            if (!string.IsNullOrEmpty(authToken))
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                HttpResponseMessage getData = await client.PostAsJsonAsync(baseURL + "User/VerifyOTP", otp);

                if (getData.IsSuccessStatusCode)
                {
                    HttpResponseMessage profileResponse = await client
                                                    .GetAsync(baseURL + "User/profile");

                    if (profileResponse.IsSuccessStatusCode)
                    {
                        var jsonResponse = await profileResponse.Content.ReadAsStringAsync();
                        ServiceResult<UserViewModel> profileResult = JsonSerializer.Deserialize<ServiceResult<UserViewModel>>(jsonResponse)!;
                        var profile = profileResult.data.value;

                        // Set session variables upon successful login
                        HttpContext.Session.SetString("IsLoggedIn", "true");
                        HttpContext.Session.SetString("Username", profile.username);
                        HttpContext.Session.SetString("UserId", profile.userId.ToString());

                        return RedirectToAction("Profile");
                    }
                }
                else
                {
                    ViewData["TFAMessage"] = "Error! OTP didn't match!";
                }
                return View();
            }
            else
            {
                ViewData["TFAMessage"] = "User not authenticated!";
                return View();
            }
        }
    }
}
