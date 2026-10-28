using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Features.Roles.DTOs;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Roles.Queries
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetRolesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            // Auto-seed default permissions if none exist yet for built-in roles
            await EnsureDefaultRolePermissionsAsync(cancellationToken);

            return await _context.Roles
                .AsNoTracking()
                .OrderByDescending(r => r.IsSystem)
                .ThenBy(r => r.RoleName)
                .Select(r => new RoleDto
                {
                    RoleId = r.RoleId,
                    RoleCode = r.RoleCode,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    IsSystem = r.IsSystem,
                    BaseRoleId = r.BaseRoleId,
                    BaseRoleName = r.BaseRole != null ? r.BaseRole.RoleName : null,
                    UserCount = r.Users.Count,
                    Permissions = r.Permissions.Select(p => new RoleScreenPermissionDto
                    {
                        ScreenId = p.ScreenId,
                        ScreenCode = p.Screen.ScreenCode,
                        ScreenName = p.Screen.ScreenName,
                        ScreenGroup = p.Screen.ScreenGroup,
                        PermLevel = p.PermLevel
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }

        private async Task EnsureDefaultRolePermissionsAsync(CancellationToken cancellationToken)
        {
            var hasAnyPermissions = await _context.RoleScreenPermissions.AnyAsync(cancellationToken);
            if (hasAnyPermissions) return;

            var roles = await _context.Roles.ToListAsync(cancellationToken);
            var screens = await _context.Screens.Where(s => s.IsActive).ToListAsync(cancellationToken);

            if (!roles.Any() || !screens.Any()) return;

            foreach (var role in roles)
            {
                foreach (var screen in screens)
                {
                    string permLevel = "none";

                    if (role.RoleCode == "admin")
                    {
                        permLevel = "admin";
                    }
                    else if (role.RoleCode == "senior_editor")
                    {
                        permLevel = screen.ScreenGroup switch
                        {
                            "Pipeline" when screen.ScreenCode == "dashboard" => "view",
                            "Pipeline" when screen.ScreenCode == "intake_queue" => "edit",
                            "Pipeline" => "approve",
                            "Administration" when screen.ScreenCode == "tagging_rules" => "edit",
                            "Administration" when screen.ScreenCode == "user_management" => "view",
                            "Administration" => "view",
                            _ => "none"
                        };
                    }
                    else if (role.RoleCode == "editor")
                    {
                        permLevel = screen.ScreenGroup switch
                        {
                            "Pipeline" when screen.ScreenCode == "tag_review" => "edit",
                            "Pipeline" => "view",
                            "Administration" when screen.ScreenCode == "tagging_rules" => "view",
                            _ => "none"
                        };
                    }

                    _context.RoleScreenPermissions.Add(new RoleScreenPermission
                    {
                        RspId = Guid.NewGuid(),
                        RoleId = role.RoleId,
                        ScreenId = screen.ScreenId,
                        PermLevel = permLevel
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
