using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Features.Departments.DTOs;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Departments.Queries
{
    public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, List<DepartmentDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetDepartmentsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Departments
                .OrderBy(d => d.DepartmentName)
                .Select(d => new DepartmentDto
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    DepartmentCode = d.DepartmentCode,
                    Description = d.Description ?? "",
                    IsActive = d.IsActive,
                    Icon = d.IconName,
                    ColorHex = d.ColourHex,
                    DocTypes = d.DocTypeSpecialisations,
                    HeadUserId = d.HeadUserId,
                    HeadUserName = d.HeadUser != null ? d.HeadUser.FullName : "Unassigned",
                    MemberCount = d.Members.Count,
                    Members = d.Members.Select(m => new DepartmentMemberDto
                    {
                        UserId = m.UserId,
                        FullName = m.FullName,
                        Email = m.Email,
                        RoleName = m.Role != null ? m.Role.RoleName : "User",
                        AvatarInitials = m.AvatarInitials
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
