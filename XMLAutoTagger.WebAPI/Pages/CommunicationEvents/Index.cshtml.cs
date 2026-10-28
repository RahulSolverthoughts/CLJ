using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Features.CommunicationEvents.DTOs;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.WebAPI.Pages.CommunicationEvents
{
    public class IndexModel : PageModel
    {
        private readonly IApplicationDbContext _context;

        public IndexModel(IApplicationDbContext context)
        {
            _context = context;
        }

        public List<CommunicationEventDto> Events { get; set; } = new();

        public int TotalEvents { get; set; }
        public int ActiveEvents { get; set; }
        public int EmailEvents { get; set; }
        public DateTime? LastUpdated { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["Title"] = "Communication Events & Templates";
            ViewData["ActivePage"] = "CommunicationEvents";
            ViewData["PageHeading"] = "Communication Events";
            ViewData["PageSubtitle"] = "Manage automated notification triggers, channels, and dynamic database templates";

            var items = await _context.CommunicationEvents
                .OrderBy(e => e.EventId)
                .Select(e => new CommunicationEventDto
                {
                    EventId = e.EventId,
                    EventCode = e.EventCode,
                    EventName = e.EventName,
                    Description = e.Description,
                    Channel = e.Channel,
                    EmailSubject = e.EmailSubject,
                    EmailBody = e.EmailBody,
                    SmsBody = e.SmsBody,
                    WhatsAppBody = e.WhatsAppBody,
                    IsActive = e.IsActive,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();

            Events = items;
            TotalEvents = items.Count;
            ActiveEvents = items.Count(e => e.IsActive);
            EmailEvents = items.Count(e => string.Equals(e.Channel, "Email", StringComparison.OrdinalIgnoreCase));
            LastUpdated = items.Any() ? items.Max(e => e.UpdatedAt) : null;

            return Page();
        }
    }
}
