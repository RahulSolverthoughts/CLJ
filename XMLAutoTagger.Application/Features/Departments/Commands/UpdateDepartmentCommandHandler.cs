using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Departments.Commands
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateDepartmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var dept = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == request.DepartmentId, cancellationToken);

            if (dept == null)
            {
                return false;
            }

            dept.DepartmentCode = request.DepartmentCode.Trim().ToUpperInvariant();
            dept.DepartmentName = request.DepartmentName.Trim();
            dept.Description = request.Description?.Trim();
            dept.IconName = string.IsNullOrWhiteSpace(request.IconName) ? "ti-building" : request.IconName.Trim();
            dept.ColourHex = string.IsNullOrWhiteSpace(request.ColourHex) ? "#185FA5" : request.ColourHex.Trim();
            dept.IsActive = request.IsActive;
            dept.DocTypeSpecialisations = request.DocTypes ?? new();
            dept.HeadUserId = request.HeadUserId;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
