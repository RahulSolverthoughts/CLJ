using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Roles.Commands
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.RoleId == request.RoleId, cancellationToken);

            if (role == null) return false;

            // Only allow changing name if not a system role
            if (!role.IsSystem && !string.IsNullOrWhiteSpace(request.RoleName))
            {
                role.RoleName = request.RoleName.Trim();
                role.BaseRoleId = request.BaseRoleId;
            }

            role.Description = request.Description?.Trim();

            // Update permissions
            if (request.Permissions != null)
            {
                // Remove existing permissions
                _context.RoleScreenPermissions.RemoveRange(role.Permissions);

                // Add updated permissions
                foreach (var perm in request.Permissions)
                {
                    _context.RoleScreenPermissions.Add(new RoleScreenPermission
                    {
                        RspId = Guid.NewGuid(),
                        RoleId = role.RoleId,
                        ScreenId = perm.ScreenId,
                        PermLevel = string.IsNullOrWhiteSpace(perm.PermLevel) ? "none" : perm.PermLevel.ToLowerInvariant()
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
