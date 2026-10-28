using System;
using XMLAutoTagger.Domain.Common;

namespace XMLAutoTagger.Domain.Entities
{
    public class WhatsAppSetting : BaseEntity
    {
        public Guid WhatsAppSettingId { get; set; }
        public string Provider { get; set; } = "MetaCloudApi"; // e.g. "MetaCloudApi", "Twilio"
        public string? ApiUrl { get; set; } = "https://graph.facebook.com/v19.0";
        public string PhoneNumberId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string? BusinessAccountId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = true;
    }
}
