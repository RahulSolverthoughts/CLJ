using System;
using MediatR;

namespace XMLAutoTagger.Application.Features.Users.Commands
{
    public record ToggleUserStatusCommand(Guid UserId) : IRequest<string>;
}
