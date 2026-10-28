using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using XMLAutoTagger.Application.Features.ScreenAccess.Commands;
using XMLAutoTagger.Application.Features.ScreenAccess.DTOs;
using XMLAutoTagger.Application.Features.Users.DTOs;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.WebAPI.Services.ApiClients;

namespace XMLAutoTagger.WebAPI.Pages.ScreenAccess
{
    public class IndexModel : PageModel
    {
        private readonly IUserManagementApiClient _apiClient;
        private readonly XMLAutoTagger.Application.Common.Interfaces.IPermissionService _permissionService;

        public IndexModel(IUserManagementApiClient apiClient, XMLAutoTagger.Application.Common.Interfaces.IPermissionService permissionService)
        {
            _apiClient = apiClient;
            _permissionService = permissionService;
        }

        public List<UserDto> Users { get; set; } = new();
        public List<Screen> Screens { get; set; } = new();
        public UserAccessDetailDto? InitialUserAccess { get; set; }
        public bool CanEdit { get; set; } = true;

        public async Task<IActionResult> OnGetAsync(Guid? userId)
        {
            ViewData["ActivePage"] = "ScreenAccess";
            CanEdit = await _permissionService.CanCurrentUserEditScreenAsync("user_management");

            // BFF calls to REST Web API
            Users = await _apiClient.GetUsersAsync();
            Screens = await _apiClient.GetScreensAsync();

            var targetUserId = userId ?? Users.FirstOrDefault()?.UserId;
            if (targetUserId.HasValue && targetUserId.Value != Guid.Empty)
            {
                InitialUserAccess = await _apiClient.GetUserAccessAsync(targetUserId.Value);
            }

            return Page();
        }

        public async Task<IActionResult> OnGetUserAccessJsonAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid user ID." });
            }

            var detail = await _apiClient.GetUserAccessAsync(userId);
            if (detail == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return new JsonResult(detail);
        }

        public async Task<IActionResult> OnPostSaveUserAccessJsonAsync([FromBody] SaveUserAccessCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return new JsonResult(new { success = false, message = "Access denied: You have view-only permission and cannot modify screen access." }) { StatusCode = 403 };
            }

            if (command == null || command.UserId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid command payload." });
            }

            try
            {
                var success = await _apiClient.SaveUserAccessAsync(command.UserId, command);
                return new JsonResult(new { success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostResetUserAccessJsonAsync([FromBody] ResetUserAccessCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return new JsonResult(new { success = false, message = "Access denied: You have view-only permission and cannot reset screen access." }) { StatusCode = 403 };
            }

            if (command == null || command.UserId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid command payload." });
            }

            try
            {
                var success = await _apiClient.ResetUserAccessAsync(command.UserId);
                return new JsonResult(new { success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
