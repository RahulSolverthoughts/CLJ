using System;
using MediatR;

namespace XMLAutoTagger.Application.Features.ScreenAccess.Commands
{
    public record ResetUserAccessCommand(Guid UserId) : IRequest<bool>;
}
