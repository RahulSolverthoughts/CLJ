using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Departments.Commands
{
    public class RemoveDepartmentMemberCommandHandler : IRequestHandler<RemoveDepartmentMemberCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public RemoveDepartmentMemberCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RemoveDepartmentMemberCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == request.UserId && u.DepartmentId == request.DepartmentId, cancellationToken);

            if (user == null)
            {
                return false;
            }

            user.DepartmentId = null;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
