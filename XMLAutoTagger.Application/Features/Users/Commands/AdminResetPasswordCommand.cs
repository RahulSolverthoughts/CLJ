using System;
using MediatR;

namespace XMLAutoTagger.Application.Features.Users.Commands
{
    public class AdminResetPasswordCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
        public bool MustChange { get; set; } = false;
    }
}
