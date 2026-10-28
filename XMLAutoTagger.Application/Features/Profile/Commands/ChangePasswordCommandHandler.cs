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

namespace XMLAutoTagger.Application.Features.Profile.Commands
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICommunicationService _communicationService;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(
            IApplicationDbContext context,
            ICommunicationService communicationService,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _context = context;
            _communicationService = communicationService;
            _logger = logger;
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
            {
                throw new ArgumentException("Valid user ID is required.");
            }

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                throw new ArgumentException("Current password is required.");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new ArgumentException("New password is required.");
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new ArgumentException("New password and confirmation password do not match.");
            }

            if (request.NewPassword.Length < 6)
            {
                throw new ArgumentException("New password must be at least 6 characters long.");
            }

            var user = await _context.Users
                .Include(u => u.Credential)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

            if (user == null)
            {
                throw new InvalidOperationException("User was not found.");
            }

            if (user.Credential == null || string.IsNullOrWhiteSpace(user.Credential.PasswordHash))
            {
                throw new InvalidOperationException("No local credential record found for this account.");
            }

            // Verify current password
            bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Credential.PasswordHash);
            if (!isCurrentPasswordValid)
            {
                throw new UnauthorizedAccessException("Current password is incorrect.");
            }

            // Hash new password
            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            if (user.Credential.PreviousHashes == null)
            {
                user.Credential.PreviousHashes = new List<string>();
            }

            user.Credential.PreviousHashes.Add(user.Credential.PasswordHash);
            user.Credential.PasswordHash = newHash;
            user.Credential.ChangedAt = DateTime.UtcNow;
            user.Credential.MustChange = false;

            await _context.SaveChangesAsync(cancellationToken);

            // Send dynamic password changed security alert based on Event 3 (PasswordChangedSelf)
            try
            {
                var templateData = new Dictionary<string, string>
                {
                    ["FullName"] = $"{user.FirstName} {user.LastName}".Trim(),
                    ["Email"] = user.Email,
                    ["ChangedAt"] = DateTime.UtcNow.ToString("dd MMM yyyy, HH:mm UTC")
                };

                _logger.LogInformation("Sending password changed security alert to {Email} via Event 3 (PasswordChangedSelf)", user.Email);
                await _communicationService.SendEventNotificationAsync(
                    CommunicationEvent.PasswordChangedSelf,
                    user.Email,
                    templateData,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password changed alert email to {Email}", user.Email);
            }

            return true;
        }
    }
}
