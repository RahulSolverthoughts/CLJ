namespace XMLAutoTagger.Application.Common.Models.Communication
{
    public class SmsRequest
    {
        /// <summary>
        /// Recipient mobile phone number (E.164 format, e.g. +60123456789)
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Optional subject/header
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Text message body
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Optional override for Sender ID or virtual phone number
        /// </summary>
        public string? SenderIdOverride { get; set; }
    }
}
