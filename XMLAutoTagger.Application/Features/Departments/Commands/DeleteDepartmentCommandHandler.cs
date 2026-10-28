using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Departments.Commands
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteDepartmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var dept = await _context.Departments
                .Include(d => d.Members)
                .FirstOrDefaultAsync(d => d.DepartmentId == request.DepartmentId, cancellationToken);

            if (dept == null)
            {
                return false;
            }

            // Unassign members before removing
            foreach (var member in dept.Members)
            {
                member.DepartmentId = null;
            }

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
