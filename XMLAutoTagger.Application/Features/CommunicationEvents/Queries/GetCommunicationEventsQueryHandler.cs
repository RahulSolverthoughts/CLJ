using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Features.CommunicationEvents.DTOs;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.CommunicationEvents.Queries
{
    public class GetCommunicationEventsQueryHandler : IRequestHandler<GetCommunicationEventsQuery, List<CommunicationEventDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetCommunicationEventsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommunicationEventDto>> Handle(GetCommunicationEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _context.CommunicationEvents
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
                .ToListAsync(cancellationToken);

            return events;
        }
    }
}
