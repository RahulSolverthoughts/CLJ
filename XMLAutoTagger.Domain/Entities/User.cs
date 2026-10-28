using System;
using System.Collections.Generic;
using XMLAutoTagger.Domain.Common;

namespace XMLAutoTagger.Domain.Entities
{
    public class User : BaseEntity
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
        public Guid? DepartmentId { get; set; }
        public List<string> DocTypeSpecialisations { get; set; } = new();

        public string AuthProvider { get; set; } = "local";
        public string? SsoExternalId { get; set; }
        public string AccountStatus { get; set; } = "active";
        public short FailedLoginCount { get; set; } = 0;
        public DateTime? LockedAt { get; set; }
        public string? LockedReason { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? AccessExpiresAt { get; set; }
        public bool MfaEnabled { get; set; } = false;

        // Navigation Properties
        public Role Role { get; set; } = null!;
        public Department? Department { get; set; }
        public UserCredential? Credential { get; set; }
        public ICollection<UserScreenOverride> ScreenOverrides { get; set; } = new List<UserScreenOverride>();
    }
}
