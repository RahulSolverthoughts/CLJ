using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Departments.Commands
{
    public class AssignDepartmentMemberCommandHandler : IRequestHandler<AssignDepartmentMemberCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public AssignDepartmentMemberCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AssignDepartmentMemberCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

            if (user == null)
            {
                return false;
            }

            user.DepartmentId = request.DepartmentId;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
