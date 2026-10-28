using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Application.Features.Users.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateUserCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Credential)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

            if (user == null) return false;

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.Email = request.Email.Trim().ToLowerInvariant();
            user.StaffId = string.IsNullOrWhiteSpace(request.StaffId) ? null : request.StaffId.Trim();
            user.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
            user.PreferredLanguage = string.IsNullOrWhiteSpace(request.PreferredLanguage) ? "en" : request.PreferredLanguage;
            user.RoleId = request.RoleId;
            user.DepartmentId = request.DepartmentId == Guid.Empty ? null : request.DepartmentId;
            user.DocTypeSpecialisations = request.DocTypeSpecialisations ?? new List<string>();
            user.AccountStatus = string.IsNullOrWhiteSpace(request.AccountStatus) ? "active" : request.AccountStatus.ToLowerInvariant();
            user.AccessExpiresAt = request.AccessExpiresAt.HasValue
                ? DateTime.SpecifyKind(request.AccessExpiresAt.Value, DateTimeKind.Utc)
                : null;

            // Optional password update
            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, 12);
                if (user.Credential != null)
                {
                    user.Credential.PasswordHash = newHash;
                    user.Credential.ChangedAt = DateTime.UtcNow;
                    user.Credential.MustChange = false;
                    user.Credential.PreviousHashes ??= new List<string>();
                    user.Credential.PreviousHashes.Add(newHash);
                }
                else
                {
                    _context.UserCredentials.Add(new UserCredential
                    {
                        UserCredId = Guid.NewGuid(),
                        UserId = user.UserId,
                        PasswordHash = newHash,
                        HashAlgorithm = "bcrypt",
                        MustChange = false,
                        ChangedAt = DateTime.UtcNow,
                        PreviousHashes = new List<string> { newHash }
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
