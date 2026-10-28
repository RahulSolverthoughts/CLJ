using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Roles.Commands
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var roleId = Guid.NewGuid();
            var roleCode = string.IsNullOrWhiteSpace(request.RoleCode)
                ? request.RoleName.Trim().ToLowerInvariant().Replace(" ", "_")
                : request.RoleCode.Trim().ToLowerInvariant().Replace(" ", "_");

            var role = new Role
            {
                RoleId = roleId,
                RoleCode = roleCode,
                RoleName = request.RoleName.Trim(),
                Description = request.Description?.Trim(),
                IsSystem = false,
                BaseRoleId = request.BaseRoleId,
                IsActive = true
            };

            _context.Roles.Add(role);

            if (request.Permissions != null && request.Permissions.Count > 0)
            {
                foreach (var perm in request.Permissions)
                {
                    _context.RoleScreenPermissions.Add(new RoleScreenPermission
                    {
                        RspId = Guid.NewGuid(),
                        RoleId = roleId,
                        ScreenId = perm.ScreenId,
                        PermLevel = string.IsNullOrWhiteSpace(perm.PermLevel) ? "none" : perm.PermLevel.ToLowerInvariant()
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return roleId;
        }
    }
}
