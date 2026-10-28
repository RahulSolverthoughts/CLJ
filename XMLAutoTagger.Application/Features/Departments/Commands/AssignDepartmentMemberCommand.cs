using System;
using MediatR;

namespace XMLAutoTagger.Application.Features.Departments.Commands
{
    public record AssignDepartmentMemberCommand(Guid DepartmentId, Guid UserId) : IRequest<bool>;
}
