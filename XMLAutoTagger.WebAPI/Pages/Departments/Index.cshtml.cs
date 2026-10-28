using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Application.Features.Departments.Commands;
using XMLAutoTagger.Application.Features.Departments.DTOs;
using XMLAutoTagger.Application.Features.Users.DTOs;
using XMLAutoTagger.WebAPI.Services.ApiClients;

namespace XMLAutoTagger.WebAPI.Pages.Departments
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

        public List<DepartmentDto> Departments { get; set; } = new();
        public List<UserDto> Users { get; set; } = new();
        public int TotalMembers => Departments.Sum(d => d.MemberCount);
        public bool CanEdit { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["ActivePage"] = "Departments";

            CanEdit = await _permissionService.CanCurrentUserEditScreenAsync("user_management");

            // BFF call to REST Web API
            Departments = await _apiClient.GetDepartmentsAsync();
            Users = await _apiClient.GetUsersAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostCreateDepartmentJsonAsync([FromBody] CreateDepartmentCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot create departments." });
            }

            if (command == null || string.IsNullOrWhiteSpace(command.DepartmentName) || string.IsNullOrWhiteSpace(command.DepartmentCode))
            {
                return BadRequest(new { message = "Department name and code are required." });
            }

            try
            {
                var departmentId = await _apiClient.CreateDepartmentAsync(command);
                return new JsonResult(new { success = true, departmentId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdateDepartmentJsonAsync([FromBody] UpdateDepartmentCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot update departments." });
            }

            if (command == null || command.DepartmentId == Guid.Empty || string.IsNullOrWhiteSpace(command.DepartmentName))
            {
                return BadRequest(new { message = "Invalid department details." });
            }

            try
            {
                var success = await _apiClient.UpdateDepartmentAsync(command.DepartmentId, command);
                return new JsonResult(new { success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public record DeleteDepartmentRequest(Guid DepartmentId);

        public async Task<IActionResult> OnPostDeleteDepartmentJsonAsync([FromBody] DeleteDepartmentRequest request)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot delete departments." });
            }

            if (request == null || request.DepartmentId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid department ID." });
            }

            try
            {
                var success = await _apiClient.DeleteDepartmentAsync(request.DepartmentId);
                return new JsonResult(new { success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public record MemberActionRequest(Guid DepartmentId, Guid UserId);

        public async Task<IActionResult> OnPostAssignMemberJsonAsync([FromBody] MemberActionRequest request)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot assign department members." });
            }

            if (request == null || request.DepartmentId == Guid.Empty || request.UserId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid assignment parameters." });
            }

            try
            {
                var success = await _apiClient.AssignDepartmentMemberAsync(request.DepartmentId, request.UserId);
                return new JsonResult(new { success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostRemoveMemberJsonAsync([FromBody] MemberActionRequest request)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot remove department members." });
            }

            if (request == null || request.DepartmentId == Guid.Empty || request.UserId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid parameters." });
            }

            try
            {
                var success = await _apiClient.RemoveDepartmentMemberAsync(request.DepartmentId, request.UserId);
                return new JsonResult(new { success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
