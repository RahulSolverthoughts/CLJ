using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Enums;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Users.Commands
{
    public class AdminResetPasswordCommandHandler : IRequestHandler<AdminResetPasswordCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICommunicationService _communicationService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminResetPasswordCommandHandler> _logger;

        public AdminResetPasswordCommandHandler(
            IApplicationDbContext context,
            ICommunicationService communicationService,
            IConfiguration configuration,
            ILogger<AdminResetPasswordCommandHandler> logger)
        {
            _context = context;
            _communicationService = communicationService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> Handle(AdminResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
            {
                throw new ArgumentException("Valid user ID is required.");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new ArgumentException("New password is required.");
            }

            if (request.NewPassword.Length < 6)
            {
                throw new ArgumentException("Password must be at least 6 characters long.");
            }

            var user = await _context.Users
                .Include(u => u.Credential)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

            if (user == null)
            {
                throw new InvalidOperationException("User was not found.");
            }

            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            if (user.Credential == null)
            {
                _context.UserCredentials.Add(new UserCredential
                {
                    UserCredId = Guid.NewGuid(),
                    UserId = user.UserId,
                    PasswordHash = newHash,
                    HashAlgorithm = "bcrypt",
                    MustChange = request.MustChange,
                    ChangedAt = DateTime.UtcNow,
                    PreviousHashes = new List<string> { newHash }
                });
            }
            else
            {
                user.Credential.PreviousHashes ??= new List<string>();
                if (!string.IsNullOrEmpty(user.Credential.PasswordHash))
                {
                    user.Credential.PreviousHashes.Add(user.Credential.PasswordHash);
                }
                user.Credential.PasswordHash = newHash;
                user.Credential.ChangedAt = DateTime.UtcNow;
                user.Credential.MustChange = request.MustChange;
            }

            // Ensure user is unlocked and active
            if (user.AccountStatus == "locked")
            {
                user.AccountStatus = "active";
                user.LockedAt = null;
                user.LockedReason = null;
                user.FailedLoginCount = 0;
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Send dynamic password reset email notification based on Event 2 (PasswordResetAdmin)
            try
            {
                var baseUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') ?? "http://localhost:5079";
                var loginUrl = $"{baseUrl}/Account/Login";

                var templateData = new Dictionary<string, string>
                {
                    ["FullName"] = $"{user.FirstName} {user.LastName}".Trim(),
                    ["Email"] = user.Email,
                    ["TemporaryPassword"] = request.NewPassword,
                    ["MustChangeNotice"] = request.MustChange ? "You are required to set a new password upon signing in." : "You may use this password to sign in immediately.",
                    ["LoginUrl"] = loginUrl
                };

                _logger.LogInformation("Sending password reset email to {Email} via Event 2 (PasswordResetAdmin)", user.Email);
                await _communicationService.SendEventNotificationAsync(
                    CommunicationEvent.PasswordResetAdmin,
                    user.Email,
                    templateData,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset notification email to {Email}", user.Email);
            }

            return true;
        }
    }
}
