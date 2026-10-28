using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Application.Features.Roles.Commands;
using XMLAutoTagger.Application.Features.Roles.DTOs;
using XMLAutoTagger.Application.Features.Roles.Queries;

namespace XMLAutoTagger.WebAPI.Controllers.Api
{
    [ApiController]
    [Route("api/v1/roles")]
    [Produces("application/json")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RolesController> _logger;
        private readonly IPermissionService _permissionService;

        public RolesController(IMediator mediator, ILogger<RolesController> logger, IPermissionService permissionService)
        {
            _mediator = mediator;
            _logger = logger;
            _permissionService = permissionService;
        }

        /// <summary>
        /// Retrieves all roles with their screen permissions and member counts.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<RoleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RoleDto>>> GetRoles()
        {
            var roles = await _mediator.Send(new GetRolesQuery());
            return Ok(roles);
        }

        /// <summary>
        /// Retrieves a single role by its ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDto>> GetRoleById(Guid id)
        {
            var roles = await _mediator.Send(new GetRolesQuery());
            var role = roles.FirstOrDefault(r => r.RoleId == id);
            if (role == null)
            {
                return NotFound(new { message = $"Role with ID {id} was not found." });
            }
            return Ok(role);
        }

        /// <summary>
        /// Creates a new role.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
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
                var roleId = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetRoleById), new { id = roleId }, new { success = true, roleId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role {RoleName}", command.RoleName);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing role and its permissions.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot update roles." });
            }

            if (command == null)
            {
                return BadRequest(new { message = "Command payload is required." });
            }

            command.RoleId = id;

            if (string.IsNullOrWhiteSpace(command.RoleName))
            {
                return BadRequest(new { message = "Role name is required." });
            }

            try
            {
                var success = await _mediator.Send(command);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {RoleId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a non-system role.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot delete roles." });
            }

            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Valid role ID is required." });
            }

            try
            {
                var success = await _mediator.Send(new DeleteRoleCommand(id));
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
