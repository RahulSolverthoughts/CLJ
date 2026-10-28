using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.ScreenAccess.Commands
{
    public class ResetUserAccessCommandHandler : IRequestHandler<ResetUserAccessCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public ResetUserAccessCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ResetUserAccessCommand request, CancellationToken cancellationToken)
        {
            var overrides = await _context.UserScreenOverrides
                .Where(o => o.UserId == request.UserId)
                .ToListAsync(cancellationToken);

            if (overrides.Count > 0)
            {
                _context.UserScreenOverrides.RemoveRange(overrides);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return true;
        }
    }
}
