using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Enums;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Users.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICommunicationService _communicationService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(
            IApplicationDbContext context,
            ICommunicationService communicationService,
            IConfiguration configuration,
            ILogger<CreateUserCommandHandler> logger)
        {
            _context = context;
            _communicationService = communicationService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim().ToLowerInvariant(),
                StaffId = string.IsNullOrWhiteSpace(request.StaffId) ? null : request.StaffId.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                PreferredLanguage = request.PreferredLanguage ?? "en",
                AvatarColourHex = "#185FA5",
                RoleId = request.RoleId,
                DepartmentId = request.DepartmentId == Guid.Empty ? null : request.DepartmentId,
                DocTypeSpecialisations = request.DocTypeSpecialisations ?? new List<string>(),
                AccountStatus = string.IsNullOrWhiteSpace(request.AccountStatus) ? "active" : request.AccountStatus.ToLowerInvariant(),
                AccessExpiresAt = request.AccessExpiresAt.HasValue
                    ? DateTime.SpecifyKind(request.AccessExpiresAt.Value, DateTimeKind.Utc)
                    : null,
                AuthProvider = "local",
                MfaEnabled = false
            };

            // Credential
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, 12);
            var credential = new UserCredential
            {
                UserCredId = Guid.NewGuid(),
                UserId = user.UserId,
                PasswordHash = passwordHash,
                HashAlgorithm = "bcrypt",
                MustChange = true,
                ChangedAt = DateTime.UtcNow,
                PreviousHashes = new List<string> { passwordHash }
            };

            _context.Users.Add(user);
            _context.UserCredentials.Add(credential);

            // Optional Screen Overrides
            if (request.ScreenOverrides != null)
            {
                foreach (var ov in request.ScreenOverrides)
                {
                    if (!string.IsNullOrWhiteSpace(ov.PermLevel) && ov.PermLevel.ToLowerInvariant() != "inherited")
                    {
                        _context.UserScreenOverrides.Add(new UserScreenOverride
                        {
                            UspId = Guid.NewGuid(),
                            UserId = user.UserId,
                            ScreenId = ov.ScreenId,
                            PermLevel = ov.PermLevel.ToLowerInvariant(),
                            ValidFrom = DateTime.UtcNow,
                            OverrideReason = "Assigned during user onboarding"
                        });
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Send dynamic welcome email notification based on Event 1 (UserCreated)
            if (request.SendWelcomeEmail)
            {
                try
                {
                    var baseUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') ?? "http://localhost:5079";
                    var loginUrl = $"{baseUrl}/Account/Login";

                    var templateData = new Dictionary<string, string>
                    {
                        ["FullName"] = $"{user.FirstName} {user.LastName}".Trim(),
                        ["Email"] = user.Email,
                        ["TemporaryPassword"] = request.Password,
                        ["LoginUrl"] = loginUrl
                    };

                    _logger.LogInformation("Sending welcome notification for new user {Email} via Event 1 (UserCreated)", user.Email);
                    await _communicationService.SendEventNotificationAsync(
                        CommunicationEvent.UserCreated,
                        user.Email,
                        templateData,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send welcome email for user {UserId} ({Email})", user.UserId, user.Email);
                }
            }

            return user.UserId;
        }
    }
}
