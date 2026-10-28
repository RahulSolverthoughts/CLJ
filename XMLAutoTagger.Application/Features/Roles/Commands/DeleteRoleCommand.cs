using System;
using MediatR;

namespace XMLAutoTagger.Application.Features.Roles.Commands
{
    public record DeleteRoleCommand(Guid RoleId) : IRequest<bool>;
}
