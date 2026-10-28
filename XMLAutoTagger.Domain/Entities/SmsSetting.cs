using System;
using XMLAutoTagger.Domain.Common;

namespace XMLAutoTagger.Domain.Entities
{
    public class SmsSetting : BaseEntity
    {
        public Guid SmsSettingId { get; set; }
        public string Provider { get; set; } = "Twilio"; // e.g. "Twilio", "GenericHttp", "Msg91"
        public string? ApiUrl { get; set; }
        public string? AccountSid { get; set; }
        public string? AuthToken { get; set; }
        public string? ApiKey { get; set; }
        public string? SenderId { get; set; } // Phone number or alphanumeric sender ID
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = true;
    }
}
