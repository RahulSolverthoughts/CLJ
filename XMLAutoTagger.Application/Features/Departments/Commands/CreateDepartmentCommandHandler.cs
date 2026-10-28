using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Departments.Commands
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateDepartmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var dept = new Department
            {
                DepartmentId = Guid.NewGuid(),
                DepartmentCode = request.DepartmentCode.Trim().ToUpperInvariant(),
                DepartmentName = request.DepartmentName.Trim(),
                Description = request.Description?.Trim(),
                IconName = string.IsNullOrWhiteSpace(request.IconName) ? "ti-building" : request.IconName.Trim(),
                ColourHex = string.IsNullOrWhiteSpace(request.ColourHex) ? "#185FA5" : request.ColourHex.Trim(),
                IsActive = request.IsActive,
                DocTypeSpecialisations = request.DocTypes ?? new(),
                HeadUserId = request.HeadUserId
            };

            _context.Departments.Add(dept);
            await _context.SaveChangesAsync(cancellationToken);

            return dept.DepartmentId;
        }
    }
}
