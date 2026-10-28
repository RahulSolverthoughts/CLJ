using System;

namespace XMLAutoTagger.Domain.Entities
{
    public class UserScreenOverride
    {
        public Guid UspId { get; set; }
        public Guid UserId { get; set; }
        public Guid ScreenId { get; set; }
        public string PermLevel { get; set; } = "none";
        public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
        public DateTime? ValidUntil { get; set; }
        public string? OverrideReason { get; set; }
        public Guid? GrantedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public User User { get; set; } = null!;
        public Screen Screen { get; set; } = null!;
    }
}
