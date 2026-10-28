using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Application.Features.Profile.Commands;
using XMLAutoTagger.Application.Features.Profile.DTOs;
using XMLAutoTagger.Application.Features.Profile.Queries;
using XMLAutoTagger.Application.Features.Users.DTOs;

namespace XMLAutoTagger.WebAPI.Controllers.Api
{
    [ApiController]
    [Route("api/v1/profile")]
    [Produces("application/json")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IMediator mediator, ICurrentUserService currentUserService, ILogger<ProfileController> logger)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        private Guid? GetEffectiveUserId()
        {
            if (_currentUserService.UserId.HasValue) return _currentUserService.UserId;

            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(claim) && Guid.TryParse(claim, out var guid)) return guid;

            return null;
        }

        /// <summary>
        /// Retrieves the profile details of the currently logged-in user.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {
            var userId = GetEffectiveUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "You must be authenticated to access profile details." });
            }

            var profile = await _mediator.Send(new GetMyProfileQuery(userId.Value));
            if (profile == null)
            {
                return NotFound(new { message = "User profile was not found." });
            }

            return Ok(profile);
        }

        /// <summary>
        /// Updates the current user's communication details (phone number only).
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequestDto request)
        {
            var userId = GetEffectiveUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "You must be authenticated to update your profile." });
            }

            try
            {
                var command = new UpdateProfileCommand(userId.Value, request?.Phone, request?.PreferredLanguage);
                var success = await _mediator.Send(command);
                return Ok(new { success, message = "Profile updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", userId.Value);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the password for the currently logged-in user.
        /// </summary>
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var userId = GetEffectiveUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "You must be authenticated to change your password." });
            }

            if (request == null)
            {
                return BadRequest(new { message = "Password payload is required." });
            }

            try
            {
                var command = new ChangePasswordCommand
                {
                    UserId = userId.Value,
                    CurrentPassword = request.CurrentPassword,
                    NewPassword = request.NewPassword,
                    ConfirmPassword = request.ConfirmPassword
                };

                var success = await _mediator.Send(command);
                return Ok(new { success, message = "Password changed successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId.Value);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
