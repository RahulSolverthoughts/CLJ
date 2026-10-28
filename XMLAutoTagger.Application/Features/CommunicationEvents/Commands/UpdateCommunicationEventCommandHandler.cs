using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.CommunicationEvents.Commands
{
    public class UpdateCommunicationEventCommandHandler : IRequestHandler<UpdateCommunicationEventCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateCommunicationEventCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UpdateCommunicationEventCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.CommunicationEvents
                .FirstOrDefaultAsync(e => e.EventId == request.EventId, cancellationToken);

            if (entity == null)
            {
                throw new InvalidOperationException($"Communication event with ID {request.EventId} was not found.");
            }

            entity.EventName = request.EventName.Trim();
            entity.Description = request.Description?.Trim();
            entity.Channel = string.IsNullOrWhiteSpace(request.Channel) ? "Email" : request.Channel.Trim();
            entity.EmailSubject = request.EmailSubject.Trim();
            entity.EmailBody = request.EmailBody.Trim();
            entity.SmsBody = request.SmsBody?.Trim();
            entity.WhatsAppBody = request.WhatsAppBody?.Trim();
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUserService.UserId;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
