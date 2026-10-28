using System;
using MediatR;

namespace XMLAutoTagger.Application.Features.Profile.Commands
{
    public class UpdateProfileCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string? Phone { get; set; }
        public string? PreferredLanguage { get; set; }

        public UpdateProfileCommand(Guid userId, string? phone, string? preferredLanguage = null)
        {
            UserId = userId;
            Phone = phone;
            PreferredLanguage = preferredLanguage;
        }
    }
}
