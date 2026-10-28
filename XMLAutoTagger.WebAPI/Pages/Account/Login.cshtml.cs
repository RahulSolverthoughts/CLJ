using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using XMLAutoTagger.Application.Features.Auth.DTOs;
using XMLAutoTagger.WebAPI.Services.ApiClients;

namespace XMLAutoTagger.WebAPI.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IUserManagementApiClient _apiClient;

        public LoginModel(IUserManagementApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool RememberMe { get; set; } = false;

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var loginResponse = await _apiClient.LoginAsync(new LoginRequestDto
                {
                    Email = Email,
                    Password = Password
                });

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    var expireTime = RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : loginResponse.ExpiresAt;

                    // Store token in secure cookie for browser navigation
                    Response.Cookies.Append("jwt_token", loginResponse.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false, // Set to true in production HTTPS
                        SameSite = SameSiteMode.Lax,
                        Expires = expireTime
                    });

                    Response.Cookies.Append("user_name", loginResponse.FullName, new CookieOptions { Expires = expireTime });
                    Response.Cookies.Append("user_role", loginResponse.RoleName, new CookieOptions { Expires = expireTime });
                    Response.Cookies.Append("user_initials", loginResponse.AvatarInitials, new CookieOptions { Expires = expireTime });

                    return RedirectToPage("/Users/Index");
                }

                ErrorMessage = "Unable to authenticate credentials.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }

        public IActionResult OnGetLogout()
        {
            Response.Cookies.Delete("jwt_token");
            Response.Cookies.Delete("user_name");
            Response.Cookies.Delete("user_role");
            Response.Cookies.Delete("user_initials");
            return RedirectToPage("/Account/Login");
        }

        public IActionResult OnPostLogout()
        {
            Response.Cookies.Delete("jwt_token");
            Response.Cookies.Delete("user_name");
            Response.Cookies.Delete("user_role");
            Response.Cookies.Delete("user_initials");
            return RedirectToPage("/Account/Login");
        }
    }
}
