using System;
using MediatR;

namespace XMLAutoTagger.Application.Features.Departments.Commands
{
    public record RemoveDepartmentMemberCommand(Guid DepartmentId, Guid UserId) : IRequest<bool>;
}
