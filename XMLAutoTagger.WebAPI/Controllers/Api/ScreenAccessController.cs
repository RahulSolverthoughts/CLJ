using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XMLAutoTagger.Application.Features.ScreenAccess.Commands;
using XMLAutoTagger.Application.Features.ScreenAccess.DTOs;
using XMLAutoTagger.Application.Features.ScreenAccess.Queries;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.WebAPI.Controllers.Api
{
    [ApiController]
    [Route("api/v1/screen-access")]
    [Produces("application/json")]
    public class ScreenAccessController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;
        private readonly XMLAutoTagger.Application.Common.Interfaces.IPermissionService _permissionService;
        private readonly ILogger<ScreenAccessController> _logger;

        public ScreenAccessController(IMediator mediator, IApplicationDbContext context, XMLAutoTagger.Application.Common.Interfaces.IPermissionService permissionService, ILogger<ScreenAccessController> logger)
        {
            _mediator = mediator;
            _context = context;
            _permissionService = permissionService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the list of active screens available in the system.
        /// </summary>
        [HttpGet("screens")]
        [ProducesResponseType(typeof(List<Screen>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Screen>>> GetScreens()
        {
            var screens = await _context.Screens
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();

            return Ok(screens);
        }

        /// <summary>
        /// Retrieves user access details including effective permissions and base role overrides.
        /// </summary>
        [HttpGet("users/{userId:guid}")]
        [ProducesResponseType(typeof(UserAccessDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserAccessDetailDto>> GetUserAccess(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "Valid user ID is required." });
            }

            var detail = await _mediator.Send(new GetUserAccessQuery(userId));
            if (detail == null)
            {
                return NotFound(new { message = $"Access details for user {userId} were not found." });
            }

            return Ok(detail);
        }

        /// <summary>
        /// Saves custom screen permission overrides for a user.
        /// </summary>
        [HttpPost("users/{userId:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SaveUserAccess(Guid userId, [FromBody] SaveUserAccessCommand command)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only access and cannot modify screen permissions." });
            }

            if (userId == Guid.Empty || command == null)
            {
                return BadRequest(new { message = "Valid user ID and command payload are required." });
            }

            command.UserId = userId;

            try
            {
                var success = await _mediator.Send(command);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving screen access overrides for user {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Resets custom screen permission overrides for a user back to their base role defaults.
        /// </summary>
        [HttpPost("users/{userId:guid}/reset")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ResetUserAccess(Guid userId)
        {
            if (!await _permissionService.CanCurrentUserEditScreenAsync("user_management"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied: You have view-only access and cannot reset screen permissions." });
            }

            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "Valid user ID is required." });
            }

            try
            {
                var success = await _mediator.Send(new ResetUserAccessCommand(userId));
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting screen access for user {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
