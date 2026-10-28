using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Roles.Commands
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.RoleId == request.RoleId, cancellationToken);

            if (role == null) return false;

            // Cannot delete built-in system roles
            if (role.IsSystem)
            {
                throw new InvalidOperationException("Built-in system roles cannot be deleted.");
            }

            // Cannot delete roles if users are assigned
            if (role.Users.Count > 0)
            {
                throw new InvalidOperationException($"Cannot delete role because {role.Users.Count} user(s) are assigned to it.");
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
