using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XMLAutoTagger.Application.Features.Auth.Commands;
using XMLAutoTagger.Application.Features.Auth.DTOs;

namespace XMLAutoTagger.WebAPI.Controllers.Api
{
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Authenticate user credentials and retrieve a JWT Bearer token.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var command = new LoginCommand
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred during login.", details = ex.Message });
            }
        }

        /// <summary>
        /// Retrieve profile information of the currently authenticated user.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var roleCode = User.FindFirstValue("role_code");
            var departmentId = User.FindFirstValue("department_id");
            var departmentName = User.FindFirstValue("department_name");
            var avatarInitials = User.FindFirstValue("avatar_initials");
            var avatarColourHex = User.FindFirstValue("avatar_colour_hex");

            return Ok(new
            {
                userId = userId != null ? Guid.Parse(userId) : (Guid?)null,
                email,
                name,
                fullName = name,
                role,
                roleCode,
                departmentId = !string.IsNullOrEmpty(departmentId) ? Guid.Parse(departmentId) : (Guid?)null,
                departmentName,
                avatarInitials,
                avatarColourHex
            });
        }
    }
}
