using System.Collections.Generic;
using MediatR;
using XMLAutoTagger.Application.Features.Roles.DTOs;

namespace XMLAutoTagger.Application.Features.Roles.Queries
{
    public record GetRolesQuery : IRequest<List<RoleDto>>;
}
