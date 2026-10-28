using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.ScreenAccess.Commands
{
    public class SaveUserAccessCommandHandler : IRequestHandler<SaveUserAccessCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public SaveUserAccessCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(SaveUserAccessCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                .Include(u => u.ScreenOverrides)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

            if (user == null) return false;

            // Remove existing overrides
            _context.UserScreenOverrides.RemoveRange(user.ScreenOverrides);

            // Add new overrides where different from role's base permission
            if (request.Overrides != null)
            {
                foreach (var ov in request.Overrides)
                {
                    var rolePerm = user.Role?.Permissions.FirstOrDefault(p => p.ScreenId == ov.ScreenId)?.PermLevel ?? "none";
                    var requestedPerm = string.IsNullOrWhiteSpace(ov.PermLevel) ? "none" : ov.PermLevel.ToLowerInvariant();

                    // Only save an override row if it differs from the inherited role permission
                    if (!string.Equals(requestedPerm, rolePerm, StringComparison.OrdinalIgnoreCase))
                    {
                        _context.UserScreenOverrides.Add(new UserScreenOverride
                        {
                            UspId = Guid.NewGuid(),
                            UserId = user.UserId,
                            ScreenId = ov.ScreenId,
                            PermLevel = requestedPerm,
                            ValidFrom = DateTime.UtcNow,
                            OverrideReason = "Screen access override by administrator",
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
