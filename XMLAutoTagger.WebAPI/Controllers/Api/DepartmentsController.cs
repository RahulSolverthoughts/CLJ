using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Application.Features.Departments.Commands;
using XMLAutoTagger.Application.Features.Departments.DTOs;
using XMLAutoTagger.Application.Features.Departments.Queries;

namespace XMLAutoTagger.WebAPI.Controllers.Api
{
    [ApiController]
    [Route("api/v1/departments")]
    [Produces("application/json")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DepartmentsController> _logger;
        private readonly IPermissionService _permissionService;

        public DepartmentsController(IMediator mediator, ILogger<DepartmentsController> logger, IPermissionService permissionService)
        {
            _mediator = mediator;
            _logger = logger;
            _permissionService = permissionService;
        }

        /// <summary>
        /// Retrieves all departments.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<DepartmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DepartmentDto>>> GetDepartments()
        {
            var departments = await _mediator.Send(new GetDepartmentsQuery());
            return Ok(departments);
        }

        /// <summary>
        /// Retrieves a department by its ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentDto>> GetDepartmentById(Guid id)
        {
            var departments = await _mediator.Send(new GetDepartmentsQuery());
            var department = departments.FirstOrDefault(d => d.DepartmentId == id);
            if (department == null)
            {
                return NotFound(new { message = $"Department with ID {id} was not found." });
            }
            return Ok(department);
        }

        /// <summary>
        /// Creates a new department.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentCommand command)
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
                var departmentId = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetDepartmentById), new { id = departmentId }, new { success = true, departmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department {DepartmentCode}", command.DepartmentCode);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing department.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot update departments." });
            }

            if (command == null)
            {
                return BadRequest(new { message = "Command payload is required." });
            }

            command.DepartmentId = id;

            if (string.IsNullOrWhiteSpace(command.DepartmentName) || string.IsNullOrWhiteSpace(command.DepartmentCode))
            {
                return BadRequest(new { message = "Department name and code are required." });
            }

            try
            {
                var success = await _mediator.Send(command);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department {DepartmentId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a department.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot delete departments." });
            }

            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Valid department ID is required." });
            }

            try
            {
                var success = await _mediator.Send(new DeleteDepartmentCommand(id));
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department {DepartmentId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        public record MemberAssignmentRequest(Guid UserId);

        /// <summary>
        /// Assigns a user to a department.
        /// </summary>
        [HttpPost("{id:guid}/members")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AssignMember(Guid id, [FromBody] MemberAssignmentRequest request)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot assign department members." });
            }

            if (id == Guid.Empty || request == null || request.UserId == Guid.Empty)
            {
                return BadRequest(new { message = "Valid department ID and user ID are required." });
            }

            try
            {
                var success = await _mediator.Send(new AssignDepartmentMemberCommand(id, request.UserId));
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning user {UserId} to department {DepartmentId}", request.UserId, id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Removes a user from a department.
        /// </summary>
        [HttpDelete("{id:guid}/members/{userId:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveMember(Guid id, Guid userId)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only permission and cannot remove department members." });
            }

            if (id == Guid.Empty || userId == Guid.Empty)
            {
                return BadRequest(new { message = "Valid department ID and user ID are required." });
            }

            try
            {
                var success = await _mediator.Send(new RemoveDepartmentMemberCommand(id, userId));
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user {UserId} from department {DepartmentId}", userId, id);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
