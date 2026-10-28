using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Features.Departments.DTOs;
using XMLAutoTagger.Application.Features.Departments.Queries;
using XMLAutoTagger.Application.Features.Roles.DTOs;
using XMLAutoTagger.Application.Features.Roles.Queries;
using XMLAutoTagger.Application.Features.Users.Commands;
using XMLAutoTagger.Application.Features.Users.DTOs;
using XMLAutoTagger.Application.Features.Users.Queries;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.WebAPI.Pages.Users
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;
        private readonly XMLAutoTagger.Application.Common.Interfaces.IPermissionService _permissionService;

        public IndexModel(
            IMediator mediator,
            IApplicationDbContext context,
            XMLAutoTagger.Application.Common.Interfaces.IPermissionService permissionService)
        {
            _mediator = mediator;
            _context = context;
            _permissionService = permissionService;
        }

        public List<UserDto> Users { get; set; } = new();
        public List<RoleDto> Roles { get; set; } = new();
        public List<DepartmentDto> Departments { get; set; } = new();
        public List<Screen> Screens { get; set; } = new();
        public bool CanEdit { get; set; } = true;

        [BindProperty]
        public CreateUserCommand FormInput { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["ActivePage"] = "Users";
            CanEdit = await _permissionService.CanCurrentUserEditScreenAsync("user_management");

            // Direct in-process MediatR queries for maximum speed and reliability
            Users = await _mediator.Send(new GetUsersQuery());
            Roles = await _mediator.Send(new GetRolesQuery());
            Departments = await _mediator.Send(new GetDepartmentsQuery());
            Screens = await _context.Screens.Where(s => s.IsActive).OrderBy(s => s.SortOrder).ToListAsync();

            return Page();
        }

        // Handler for creating user (from wizard)
        public async Task<IActionResult> OnPostCreateUserJsonAsync([FromBody] CreateUserCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return new JsonResult(new { success = false, message = "Access denied: You have view-only access to user management." }) { StatusCode = 403 };
            }

            if (command == null || string.IsNullOrWhiteSpace(command.FirstName) || string.IsNullOrWhiteSpace(command.Email))
            {
                return BadRequest(new { message = "First name and email are required." });
            }

            try
            {
                var userId = await _mediator.Send(command);
                return new JsonResult(new { success = true, userId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Handler for editing user
        public async Task<IActionResult> OnPostUpdateUserJsonAsync([FromBody] UpdateUserCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return new JsonResult(new { success = false, message = "Access denied: You have view-only access to user management." }) { StatusCode = 403 };
            }

            if (command == null || command.UserId == Guid.Empty || string.IsNullOrWhiteSpace(command.FirstName) || string.IsNullOrWhiteSpace(command.Email))
            {
                return BadRequest(new { message = "Valid user ID, first name and email are required." });
            }

            try
            {
                var success = await _mediator.Send(command);
                return new JsonResult(new { success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public record ToggleUserStatusRequest(Guid UserId);

        // Handler for toggling status (Active <-> Locked)
        public async Task<IActionResult> OnPostToggleUserStatusJsonAsync([FromBody] ToggleUserStatusRequest req)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return new JsonResult(new { success = false, message = "Access denied: You have view-only access to user management." }) { StatusCode = 403 };
            }

            if (req == null || req.UserId == Guid.Empty)
            {
                return BadRequest(new { message = "Valid user ID is required." });
            }

            try
            {
                var newStatus = await _mediator.Send(new ToggleUserStatusCommand(req.UserId));
                return new JsonResult(new { success = true, status = newStatus });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public record AdminResetPasswordRequest(Guid UserId, string NewPassword, bool MustChange = false);

        // Handler for resetting user password directly
        public async Task<IActionResult> OnPostAdminResetPasswordJsonAsync([FromBody] AdminResetPasswordRequest req)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return new JsonResult(new { success = false, message = "Access denied: You have view-only access to user management." }) { StatusCode = 403 };
            }

            if (req == null || req.UserId == Guid.Empty || string.IsNullOrWhiteSpace(req.NewPassword))
            {
                return BadRequest(new { message = "User ID and new password are required." });
            }

            try
            {
                var success = await _mediator.Send(new AdminResetPasswordCommand
                {
                    UserId = req.UserId,
                    NewPassword = req.NewPassword,
                    MustChange = req.MustChange
                });
                return new JsonResult(new { success = true, message = "Password updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
