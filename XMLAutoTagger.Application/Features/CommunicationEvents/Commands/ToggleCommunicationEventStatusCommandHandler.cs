using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.CommunicationEvents.Commands
{
    public class ToggleCommunicationEventStatusCommandHandler : IRequestHandler<ToggleCommunicationEventStatusCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ToggleCommunicationEventStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ToggleCommunicationEventStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.CommunicationEvents
                .FirstOrDefaultAsync(e => e.EventId == request.EventId, cancellationToken);

            if (entity == null)
            {
                throw new InvalidOperationException($"Communication event with ID {request.EventId} was not found.");
            }

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUserService.UserId;

            await _context.SaveChangesAsync(cancellationToken);
            return entity.IsActive;
        }
    }
}
