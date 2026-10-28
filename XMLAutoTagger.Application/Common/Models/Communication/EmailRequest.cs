namespace XMLAutoTagger.Application.Common.Models.Communication
{
    public class EmailRequest
    {
        /// <summary>
        /// Recipient email address (can be specified or overridden per call)
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Subject line (can be specified or overridden per call)
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Email body content (HTML or Plain Text)
        /// </summary>
        public string Body { get; set; } = string.Empty;

        public bool IsHtml { get; set; } = true;

        /// <summary>
        /// Optional override for Sender Email
        /// </summary>
        public string? SenderEmailOverride { get; set; }

        /// <summary>
        /// Optional override for Sender Display Name
        /// </summary>
        public string? SenderNameOverride { get; set; }
    }
}
