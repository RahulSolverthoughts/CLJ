using System;
using XMLAutoTagger.Domain.Common;

namespace XMLAutoTagger.Domain.Entities
{
    /// <summary>
    /// Consolidated communication event and dynamic template configuration.
    /// Maps 1-to-1 with user_mgmt.communication_events table.
    /// </summary>
    public class CommunicationEventItem : BaseEntity
    {
        /// <summary>
        /// Integer ID corresponding directly to CommunicationEvent enum value.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Event unique code (max 200 chars), e.g. 'USER_CREATED', 'PASSWORD_RESET_ADMIN', 'PASSWORD_CHANGED_SELF'.
        /// </summary>
        public string EventCode { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable event name (max 200 chars).
        /// </summary>
        public string EventName { get; set; } = string.Empty;

        /// <summary>
        /// Description of the event purpose and trigger conditions.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Primary channel, e.g. 'Email', 'Sms', 'WhatsApp'.
        /// </summary>
        public string Channel { get; set; } = "Email";

        /// <summary>
        /// Dynamic email subject template supporting {{Placeholder}} merge tags (max 500 chars).
        /// </summary>
        public string EmailSubject { get; set; } = string.Empty;

        /// <summary>
        /// Dynamic email body template in full responsive HTML with {{Placeholder}} merge tags.
        /// </summary>
        public string EmailBody { get; set; } = string.Empty;

        /// <summary>
        /// Optional SMS body template with {{Placeholder}} merge tags.
        /// </summary>
        public string? SmsBody { get; set; }

        /// <summary>
        /// Optional WhatsApp body template with {{Placeholder}} merge tags.
        /// </summary>
        public string? WhatsAppBody { get; set; }

        /// <summary>
        /// Indicates if notification dispatch is enabled for this event.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
