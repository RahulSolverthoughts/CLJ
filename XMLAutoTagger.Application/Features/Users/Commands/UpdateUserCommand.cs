using System;
using System.Collections.Generic;
using MediatR;

namespace XMLAutoTagger.Application.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? StaffId { get; set; }
        public string? Phone { get; set; }
        public string PreferredLanguage { get; set; } = "en";
        public Guid RoleId { get; set; }
        public Guid? DepartmentId { get; set; }
        public List<string> DocTypeSpecialisations { get; set; } = new();
        public string AccountStatus { get; set; } = "active";
        public DateTime? AccessExpiresAt { get; set; }
        public string? NewPassword { get; set; }
    }
}
