using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Features.ScreenAccess.DTOs;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.ScreenAccess.Queries
{
    public class GetUserAccessQueryHandler : IRequestHandler<GetUserAccessQuery, UserAccessDetailDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetUserAccessQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserAccessDetailDto?> Handle(GetUserAccessQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                .Include(u => u.Department)
                .Include(u => u.ScreenOverrides)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

            if (user == null) return null;

            var screens = await _context.Screens
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ToListAsync(cancellationToken);

            var result = new UserAccessDetailDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role != null ? user.Role.RoleName : "User",
                DepartmentName = user.Department != null ? user.Department.DepartmentName : "Unassigned",
                AvatarInitials = user.AvatarInitials
            };

            foreach (var s in screens)
            {
                var rolePerm = user.Role?.Permissions.FirstOrDefault(p => p.ScreenId == s.ScreenId)?.PermLevel ?? "none";
                var userOverride = user.ScreenOverrides.FirstOrDefault(o => o.ScreenId == s.ScreenId)?.PermLevel;

                result.Screens.Add(new ScreenAccessItemDto
                {
                    ScreenId = s.ScreenId,
                    ScreenCode = s.ScreenCode,
                    ScreenName = s.ScreenName,
                    ScreenGroup = s.ScreenGroup,
                    IconName = s.IconName,
                    RoutePath = s.RoutePath,
                    RolePermLevel = rolePerm,
                    OverridePermLevel = userOverride
                });
            }

            return result;
        }
    }
}
