using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Domain.Enums;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Users.Commands
{
    public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, string>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICommunicationService _communicationService;
        private readonly ILogger<ToggleUserStatusCommandHandler> _logger;

        public ToggleUserStatusCommandHandler(
            IApplicationDbContext context,
            ICommunicationService communicationService,
            ILogger<ToggleUserStatusCommandHandler> logger)
        {
            _context = context;
            _communicationService = communicationService;
            _logger = logger;
        }

        public async Task<string> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);
            if (user == null) return "not_found";

            bool wasLocked = false;
            if (user.AccountStatus == "active")
            {
                user.AccountStatus = "locked";
                user.LockedAt = DateTime.UtcNow;
                user.LockedReason = "Manually locked by administrator";
                wasLocked = true;
            }
            else
            {
                user.AccountStatus = "active";
                user.LockedAt = null;
                user.LockedReason = null;
                user.FailedLoginCount = 0;
            }

            await _context.SaveChangesAsync(cancellationToken);

            if (wasLocked)
            {
                try
                {
                    var templateData = new Dictionary<string, string>
                    {
                        { "FullName", user.FullName },
                        { "Email", user.Email },
                        { "LockedReason", user.LockedReason ?? "Administrative action" },
                        { "LockedAt", user.LockedAt?.ToString("dd MMM yyyy, HH:mm") ?? DateTime.UtcNow.ToString("dd MMM yyyy, HH:mm") },
                        { "AppName", "LegalXML" }
                    };

                    _logger.LogInformation("Sending account locked notification to {Email} via Event 4 (UserAccountLocked)", user.Email);
                    await _communicationService.SendEventNotificationAsync(
                        CommunicationEvent.UserAccountLocked,
                        user.Email,
                        templateData,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send account lockout email to {Email}", user.Email);
                }
            }

            return user.AccountStatus;
        }
    }
}
