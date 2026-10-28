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
    public class ChangePasswordModel : PageModel
    {
        private readonly IUserManagementApiClient _apiClient;
        private readonly ICurrentUserService _currentUserService;

        public ChangePasswordModel(IUserManagementApiClient apiClient, ICurrentUserService currentUserService)
        {
            _apiClient = apiClient;
            _currentUserService = currentUserService;
        }

        public UserDto? CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["Title"] = "Settings — Change Password";
            ViewData["ActivePage"] = "Settings";
            ViewData["PageHeading"] = "Account settings";
            ViewData["PageSubtitle"] = "Update your credentials to maintain account security";

            var token = Request.Cookies["jwt_token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                CurrentUser = await _apiClient.GetMyProfileAsync();
            }
            catch { }

            if (CurrentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostChangePasswordJsonAsync([FromBody] ChangePasswordRequestDto request)
        {
            var token = Request.Cookies["jwt_token"];
            if (string.IsNullOrEmpty(token))
            {
                return StatusCode(401, new { message = "You must be signed in to change your password." });
            }

            if (request == null)
            {
                return BadRequest(new { message = "Password fields are required." });
            }

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                return BadRequest(new { message = "Please enter your current password." });
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(new { message = "Please enter a new password." });
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new { message = "New password and confirmation password do not match." });
            }

            if (request.NewPassword.Length < 6)
            {
                return BadRequest(new { message = "New password must be at least 6 characters long." });
            }

            try
            {
                var success = await _apiClient.ChangeMyPasswordAsync(request.CurrentPassword, request.NewPassword, request.ConfirmPassword);
                return new JsonResult(new { success, message = "Your password has been changed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
