namespace XMLAutoTagger.Application.Common.Models.Communication
{
    public class WhatsAppRequest
    {
        /// <summary>
        /// Recipient WhatsApp number (E.164 format, e.g. +60123456789)
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Optional subject or title (for message header/context)
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Text message content
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Optional template name if sending a pre-approved Meta WhatsApp template
        /// </summary>
        public string? TemplateName { get; set; }
    }
}
