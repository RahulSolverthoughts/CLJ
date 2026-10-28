using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XMLAutoTagger.Application.Features.Users.Commands;
using XMLAutoTagger.Application.Features.Users.DTOs;
using XMLAutoTagger.Application.Features.Users.Queries;

namespace XMLAutoTagger.WebAPI.Controllers.Api
{
    [ApiController]
    [Route("api/v1/users")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly XMLAutoTagger.Application.Common.Interfaces.IPermissionService _permissionService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, XMLAutoTagger.Application.Common.Interfaces.IPermissionService permissionService, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _permissionService = permissionService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all users with optional filtering.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDto>>> GetUsers(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] Guid? departmentId = null,
            [FromQuery] Guid? roleId = null)
        {
            var users = await _mediator.Send(new GetUsersQuery());

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                users = users.Where(u => u.FullName.ToLower().Contains(s) || u.Email.ToLower().Contains(s) || (u.StaffId?.ToLower().Contains(s) ?? false)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "all")
            {
                users = users.Where(u => string.Equals(u.AccountStatus, status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (departmentId.HasValue && departmentId.Value != Guid.Empty)
            {
                users = users.Where(u => u.DepartmentId == departmentId.Value).ToList();
            }

            if (roleId.HasValue && roleId.Value != Guid.Empty)
            {
                users = users.Where(u => u.RoleId == roleId.Value).ToList();
            }

            return Ok(users);
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var users = await _mediator.Send(new GetUsersQuery());
            var user = users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} was not found." });
            }
            return Ok(user);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only access to user management." });
            }

            if (command == null || string.IsNullOrWhiteSpace(command.FirstName) || string.IsNullOrWhiteSpace(command.Email))
            {
                return BadRequest(new { message = "First name and email are required." });
            }

            try
            {
                var userId = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetUserById), new { id = userId }, new { success = true, userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email {Email}", command.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only access to user management." });
            }

            if (command == null)
            {
                return BadRequest(new { message = "Command payload is required." });
            }

            command.UserId = id;

            if (string.IsNullOrWhiteSpace(command.FirstName) || string.IsNullOrWhiteSpace(command.Email))
            {
                return BadRequest(new { message = "First name and email are required." });
            }

            try
            {
                var success = await _mediator.Send(command);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Toggles a user's active/suspended status.
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only access to user management." });
            }

            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Valid user ID is required." });
            }

            try
            {
                var newStatus = await _mediator.Send(new ToggleUserStatusCommand(id));
                return Ok(new { success = true, newStatus });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status for user {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        public record AdminResetPasswordDto(string NewPassword, bool MustChange = false);

        /// <summary>
        /// Resets a user's password directly as an administrator.
        /// </summary>
        [HttpPost("{id:guid}/reset-password")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] AdminResetPasswordDto dto)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only access to user management." });
            }

            if (id == Guid.Empty || dto == null || string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest(new { message = "User ID and new password are required." });
            }

            try
            {
                var success = await _mediator.Send(new AdminResetPasswordCommand
                {
                    UserId = id,
                    NewPassword = dto.NewPassword,
                    MustChange = dto.MustChange
                });
                return Ok(new { success, message = "Password updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
