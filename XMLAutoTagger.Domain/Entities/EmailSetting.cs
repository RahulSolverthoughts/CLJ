using System;
using XMLAutoTagger.Domain.Common;

namespace XMLAutoTagger.Domain.Entities
{
    public class EmailSetting : BaseEntity
    {
        public Guid EmailSettingId { get; set; }
        public string Provider { get; set; } = "SMTP"; // e.g. "SMTP", "SendGrid"
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderDisplayName { get; set; } = "LegalXML Notification";
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ApiKey { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = true;
    }
}
