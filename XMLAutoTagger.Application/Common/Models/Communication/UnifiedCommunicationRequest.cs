using XMLAutoTagger.Domain.Enums;

namespace XMLAutoTagger.Application.Common.Models.Communication
{
    public class UnifiedCommunicationRequest
    {
        /// <summary>
        /// Channel to route to: Email (1), Sms (2), or WhatsApp (3)
        /// </summary>
        public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;

        /// <summary>
        /// Target recipient (Email address or Mobile phone number)
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Subject line (supported on Email and prepended on SMS/WhatsApp if provided)
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Message content or body
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Whether message contains HTML (for Email)
        /// </summary>
        public bool IsHtml { get; set; } = true;

        /// <summary>
        /// Optional Sender Override (Sender Email/Name for Email, Sender ID for SMS)
        /// </summary>
        public string? SenderOverride { get; set; }
    }
}
