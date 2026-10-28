using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Features.Users.DTOs;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Users.Queries
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetUsersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = u.FullName,
                    StaffId = u.StaffId,
                    Phone = u.Phone,
                    PreferredLanguage = u.PreferredLanguage,
                    AvatarInitials = u.AvatarInitials,
                    AvatarColourHex = u.AvatarColourHex,
                    RoleId = u.RoleId,
                    RoleName = u.Role != null ? u.Role.RoleName : "No Role",
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department != null ? u.Department.DepartmentName : "Unassigned",
                    DocTypeSpecialisations = u.DocTypeSpecialisations,
                    AccountStatus = u.AccountStatus,
                    LastLoginAt = u.LastLoginAt,
                    AccessExpiresAt = u.AccessExpiresAt,
                    MfaEnabled = u.MfaEnabled
                })
                .ToListAsync(cancellationToken);
        }
    }
}
