using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Application.Features.Roles.Commands;
using XMLAutoTagger.Application.Features.Roles.DTOs;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.WebAPI.Services.ApiClients;

namespace XMLAutoTagger.WebAPI.Pages.Roles
{
    public class IndexModel : PageModel
    {
        private readonly IUserManagementApiClient _apiClient;
        private readonly IPermissionService _permissionService;

        public IndexModel(IUserManagementApiClient apiClient, IPermissionService permissionService)
        {
            _apiClient = apiClient;
            _permissionService = permissionService;
        }

        public List<RoleDto> Roles { get; set; } = new();
        public List<Screen> Screens { get; set; } = new();
        public int BuiltInRolesCount => Roles.Count(r => r.IsSystem);
        public int CustomRolesCount => Roles.Count(r => !r.IsSystem);
        public bool CanEdit { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["ActivePage"] = "Roles";

            CanEdit = await _permissionService.CanCurrentUserEditScreenAsync("user_management");

            // BFF calls to REST Web API
            Roles = await _apiClient.GetRolesAsync();
            Screens = await _apiClient.GetScreensAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostCreateRoleJsonAsync([FromBody] CreateRoleCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot create roles." });
            }

            if (command == null || string.IsNullOrWhiteSpace(command.RoleName))
            {
                return BadRequest(new { message = "Role name is required." });
            }

            try
            {
                var roleId = await _apiClient.CreateRoleAsync(command);
                return new JsonResult(new { success = true, roleId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdateRoleJsonAsync([FromBody] UpdateRoleCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot update roles." });
            }

            if (command == null || command.RoleId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid role ID." });
            }

            try
            {
                var success = await _apiClient.UpdateRoleAsync(command.RoleId, command);
                return new JsonResult(new { success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public record DeleteRoleRequest(Guid RoleId);

        public async Task<IActionResult> OnPostDeleteRoleJsonAsync([FromBody] DeleteRoleRequest request)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot delete roles." });
            }

            if (request == null || request.RoleId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid role ID." });
            }

            try
            {
                var success = await _apiClient.DeleteRoleAsync(request.RoleId);
                return new JsonResult(new { success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
