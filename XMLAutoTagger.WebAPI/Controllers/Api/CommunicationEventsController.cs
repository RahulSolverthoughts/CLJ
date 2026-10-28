using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Application.Common.Models.Communication;
using XMLAutoTagger.Application.Features.CommunicationEvents.Commands;
using XMLAutoTagger.Application.Features.CommunicationEvents.DTOs;
using XMLAutoTagger.Application.Features.CommunicationEvents.Queries;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.WebAPI.Controllers.Api
{
    [ApiController]
    [Route("api/v1/communication-events")]
    [Produces("application/json")]
    public class CommunicationEventsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;
        private readonly ICommunicationService _communicationService;
        private readonly ILogger<CommunicationEventsController> _logger;

        public CommunicationEventsController(
            IMediator mediator,
            IApplicationDbContext context,
            ICommunicationService communicationService,
            ILogger<CommunicationEventsController> logger)
        {
            _mediator = mediator;
            _context = context;
            _communicationService = communicationService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all communication events and their dynamic templates.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<CommunicationEventDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CommunicationEventDto>>> GetEvents()
        {
            var events = await _mediator.Send(new GetCommunicationEventsQuery());
            return Ok(events);
        }

        /// <summary>
        /// Retrieves a specific communication event by its Event ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CommunicationEventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommunicationEventDto>> GetEventById(int id)
        {
            var events = await _mediator.Send(new GetCommunicationEventsQuery());
            var item = events.Find(e => e.EventId == id);
            if (item == null)
            {
                return NotFound(new { message = $"Communication event with ID {id} was not found." });
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new communication event and dynamic template.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEvent([FromBody] CreateCommunicationEventCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.EventCode) || string.IsNullOrWhiteSpace(command.EventName))
            {
                return BadRequest(new { message = "Event code and event name are required." });
            }

            try
            {
                var eventId = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetEventById), new { id = eventId }, new { success = true, eventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating communication event {Code}", command.EventCode);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing communication event and its template.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateCommunicationEventCommand command)
        {
            if (command == null)
            {
                return BadRequest(new { message = "Payload is required." });
            }

            command.EventId = id;
            try
            {
                var success = await _mediator.Send(command);
                return Ok(new { success, message = "Communication event updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating communication event {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Toggles a communication event's active status.
        /// </summary>
        [HttpPatch("{id:int}/status")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var isActive = await _mediator.Send(new ToggleCommunicationEventStatusCommand(id));
                return Ok(new { success = true, isActive });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status for event {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        public record TestEventEmailDto(string RecipientEmail, Dictionary<string, string>? SampleTokens);

        /// <summary>
        /// Sends a test email dispatch using the event's dynamic template.
        /// </summary>
        [HttpPost("{id:int}/test")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendTestEmail(int id, [FromBody] TestEventEmailDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.RecipientEmail))
            {
                return BadRequest(new { message = "Recipient email is required." });
            }

            var evt = await _context.CommunicationEvents.FirstOrDefaultAsync(e => e.EventId == id);
            if (evt == null)
            {
                return NotFound(new { message = $"Event {id} not found." });
            }

            var tokens = dto.SampleTokens ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "FullName", "John Doe" },
                { "Email", dto.RecipientEmail },
                { "TemporaryPassword", "TempSecure123!" },
                { "MustChangeNotice", "For security compliance, you must change your password upon logging in." },
                { "LoginUrl", "http://localhost:5079/login" },
                { "ChangedAt", DateTime.UtcNow.ToString("dd MMM yyyy, HH:mm 'UTC'") }
            };

            var subject = ReplaceTokens(evt.EmailSubject, tokens);
            var body = ReplaceTokens(evt.EmailBody, tokens);

            var result = await _communicationService.SendEmailAsync(new EmailRequest
            {
                To = dto.RecipientEmail.Trim(),
                Subject = subject,
                Body = body,
                IsHtml = true
            });

            return Ok(new
            {
                success = result.Success,
                renderedSubject = subject,
                renderedBody = body,
                message = result.Success
                    ? $"Test email dispatched successfully to {dto.RecipientEmail}"
                    : $"Test email dispatch failed: {result.ErrorMessage ?? "Check SMTP credentials"}"
            });
        }

        private static string ReplaceTokens(string template, IDictionary<string, string> tokens)
        {
            if (string.IsNullOrEmpty(template) || tokens == null || tokens.Count == 0)
                return template;

            var result = template;
            foreach (var kvp in tokens)
            {
                result = result.Replace("{{" + kvp.Key + "}}", kvp.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }
    }
}
