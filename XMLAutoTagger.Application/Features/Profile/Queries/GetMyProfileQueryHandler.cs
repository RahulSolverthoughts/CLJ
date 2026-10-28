using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Features.Users.DTOs;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Profile.Queries
{
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, UserDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetMyProfileQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty) return null;

            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId);

            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                StaffId = user.StaffId,
                Phone = user.Phone,
                PreferredLanguage = user.PreferredLanguage,
                AvatarInitials = user.AvatarInitials,
                AvatarColourHex = user.AvatarColourHex,
                RoleId = user.RoleId,
                RoleName = user.Role?.RoleName ?? "User",
                DepartmentId = user.DepartmentId,
                DepartmentName = user.Department?.DepartmentName ?? "Unassigned",
                DocTypeSpecialisations = user.DocTypeSpecialisations ?? new System.Collections.Generic.List<string>(),
                AccountStatus = user.AccountStatus,
                LastLoginAt = user.LastLoginAt,
                AccessExpiresAt = user.AccessExpiresAt,
                MfaEnabled = user.MfaEnabled
            };
        }
    }
}
