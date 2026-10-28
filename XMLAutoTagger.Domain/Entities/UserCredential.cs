using System;
using System.Collections.Generic;

namespace XMLAutoTagger.Domain.Entities
{
    public class UserCredential
    {
        public Guid UserCredId { get; set; }
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string HashAlgorithm { get; set; } = "bcrypt";
        public bool MustChange { get; set; } = true;
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public List<string> PreviousHashes { get; set; } = new();

        // Navigation
        public User User { get; set; } = null!;
    }
}
