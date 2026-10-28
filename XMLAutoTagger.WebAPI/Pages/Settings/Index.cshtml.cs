using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Application.Features.Profile.DTOs;
using XMLAutoTagger.Application.Features.Users.DTOs;
using XMLAutoTagger.WebAPI.Services.ApiClients;

namespace XMLAutoTagger.WebAPI.Pages.Settings
{
    public class IndexModel : PageModel
    {
        private readonly IUserManagementApiClient _apiClient;
        private readonly ICurrentUserService _currentUserService;

        public IndexModel(IUserManagementApiClient apiClient, ICurrentUserService currentUserService)
        {
            _apiClient = apiClient;
            _currentUserService = currentUserService;
        }

        public UserDto? CurrentUser { get; set; }
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["Title"] = "Settings — My Profile";
            ViewData["ActivePage"] = "Settings";
            ViewData["PageHeading"] = "Account settings";
            ViewData["PageSubtitle"] = "Manage your personal profile and communication details";

            var token = Request.Cookies["jwt_token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                CurrentUser = await _apiClient.GetMyProfileAsync();
            }
            catch
            {
                // In case of token expiry or error
            }

            if (CurrentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdatePhoneJsonAsync([FromBody] UpdateProfileRequestDto request)
        {
            var token = Request.Cookies["jwt_token"];
            if (string.IsNullOrEmpty(token))
            {
                return StatusCode(401, new { message = "You must be signed in to update your profile." });
            }

            try
            {
                var success = await _apiClient.UpdateMyProfileAsync(request?.Phone, request?.PreferredLanguage);
                return new JsonResult(new { success, message = "Profile details updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
