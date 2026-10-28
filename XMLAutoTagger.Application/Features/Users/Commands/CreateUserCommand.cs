using System;
using System.Collections.Generic;
using MediatR;

namespace XMLAutoTagger.Application.Features.Users.Commands
{
    public class CreateUserCommand : IRequest<Guid>
    {
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

        public string Password { get; set; } = "TempPass2026!";
        public bool SendWelcomeEmail { get; set; } = true;
        public List<ScreenOverrideInput> ScreenOverrides { get; set; } = new();
    }

    public class ScreenOverrideInput
    {
        public Guid ScreenId { get; set; }
        public string PermLevel { get; set; } = "none";
    }
}
