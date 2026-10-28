using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.CommunicationEvents.Commands
{
    public class CreateCommunicationEventCommandHandler : IRequestHandler<CreateCommunicationEventCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateCommunicationEventCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(CreateCommunicationEventCommand request, CancellationToken cancellationToken)
        {
            var code = request.EventCode.Trim().ToUpperInvariant();
            if (await _context.CommunicationEvents.AnyAsync(e => e.EventCode == code, cancellationToken))
            {
                throw new InvalidOperationException($"A communication event with code '{code}' already exists.");
            }

            int eventId;
            if (request.EventId.HasValue && request.EventId.Value > 0)
            {
                if (await _context.CommunicationEvents.AnyAsync(e => e.EventId == request.EventId.Value, cancellationToken))
                {
                    throw new InvalidOperationException($"An event with Event ID {request.EventId.Value} already exists.");
                }
                eventId = request.EventId.Value;
            }
            else
            {
                var maxId = await _context.CommunicationEvents.MaxAsync(e => (int?)e.EventId, cancellationToken) ?? 0;
                eventId = maxId + 1;
            }

            var entity = new CommunicationEventItem
            {
                EventId = eventId,
                EventCode = code,
                EventName = request.EventName.Trim(),
                Description = request.Description?.Trim(),
                Channel = string.IsNullOrWhiteSpace(request.Channel) ? "Email" : request.Channel.Trim(),
                EmailSubject = request.EmailSubject.Trim(),
                EmailBody = request.EmailBody.Trim(),
                SmsBody = request.SmsBody?.Trim(),
                WhatsAppBody = request.WhatsAppBody?.Trim(),
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserId,
                UpdatedBy = _currentUserService.UserId
            };

            _context.CommunicationEvents.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.EventId;
        }
    }
}
