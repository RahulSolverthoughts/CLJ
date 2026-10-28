using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Infrastructure.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public PermissionService(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<string> GetEffectivePermissionAsync(Guid userId, string screenCode)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                .Include(u => u.ScreenOverrides)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return "none";

            // Administrator role always has full admin rights
            if (user.Role?.RoleCode == "admin" || string.Equals(user.Role?.RoleName, "Administrator", StringComparison.OrdinalIgnoreCase))
            {
                return "admin";
            }

            var screen = await _context.Screens
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ScreenCode == screenCode);

            if (screen == null) return "none";

            // User-level override takes precedence
            var userOverride = user.ScreenOverrides
                .FirstOrDefault(o => o.ScreenId == screen.ScreenId && (!o.ValidUntil.HasValue || o.ValidUntil.Value > DateTime.UtcNow));

            if (userOverride != null && !string.IsNullOrEmpty(userOverride.PermLevel))
            {
                return userOverride.PermLevel.ToLowerInvariant();
            }

            // Role-level default
            var rolePerm = user.Role?.Permissions
                .FirstOrDefault(p => p.ScreenId == screen.ScreenId)?.PermLevel;

            return (rolePerm ?? "none").ToLowerInvariant();
        }

        public async Task<bool> CanCurrentUserEditScreenAsync(string screenCode)
        {
            var roleCode = _currentUserService.RoleCode;
            if (roleCode == "admin" || string.Equals(_currentUserService.RoleName, "Administrator", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var userId = _currentUserService.UserId;
            if (!userId.HasValue || userId.Value == Guid.Empty)
            {
                // Fallback: If not admin and no user ID resolved, deny editing
                return false;
            }

            var perm = await GetEffectivePermissionAsync(userId.Value, screenCode);
            // Only 'edit', 'approve', and 'admin' can make changes.
            // 'view' or 'none' are strictly READ ONLY!
            return perm == "edit" || perm == "approve" || perm == "admin";
        }
    }
}
