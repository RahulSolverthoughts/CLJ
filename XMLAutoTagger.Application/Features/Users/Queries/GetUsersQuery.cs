using System.Collections.Generic;
using MediatR;
using XMLAutoTagger.Application.Features.Users.DTOs;

namespace XMLAutoTagger.Application.Features.Users.Queries
{
    public record GetUsersQuery : IRequest<List<UserDto>>;
}
