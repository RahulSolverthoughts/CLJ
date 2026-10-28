using System;
using System.Collections.Generic;

namespace XMLAutoTagger.Application.Features.Users.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? StaffId { get; set; }
        public string? Phone { get; set; }
        public string PreferredLanguage { get; set; } = "en";
        public string AvatarInitials { get; set; } = string.Empty;
        public string AvatarColourHex { get; set; } = "#185FA5";

        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public Guid? DepartmentId { get; set; }
        public string DepartmentName { get; set; } = "Unassigned";

        public List<string> DocTypeSpecialisations { get; set; } = new();
        public string AccountStatus { get; set; } = "active";
        public DateTime? LastLoginAt { get; set; }
        public DateTime? AccessExpiresAt { get; set; }
        public bool MfaEnabled { get; set; }

        public string SpecialisationText => DocTypeSpecialisations.Count > 0 ? string.Join(", ", DocTypeSpecialisations) : "General";
    }
}
