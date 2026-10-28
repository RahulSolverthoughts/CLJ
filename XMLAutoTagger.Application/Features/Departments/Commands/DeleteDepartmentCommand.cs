using System;
using MediatR;

namespace XMLAutoTagger.Application.Features.Departments.Commands
{
    public record DeleteDepartmentCommand(Guid DepartmentId) : IRequest<bool>;
}
