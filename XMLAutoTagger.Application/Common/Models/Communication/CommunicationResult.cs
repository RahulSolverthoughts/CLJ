using System;
using XMLAutoTagger.Domain.Enums;

namespace XMLAutoTagger.Application.Common.Models.Communication
{
    public class CommunicationResult
    {
        public bool Success { get; set; }
        public CommunicationChannel Channel { get; set; }
        public string Recipient { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string? MessageId { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static CommunicationResult Succeeded(CommunicationChannel channel, string recipient, string? messageId = null, string? subject = null)
        {
            return new CommunicationResult
            {
                Success = true,
                Channel = channel,
                Recipient = recipient,
                Subject = subject,
                MessageId = messageId
            };
        }

        public static CommunicationResult Failed(CommunicationChannel channel, string recipient, string errorMessage, string? subject = null)
        {
            return new CommunicationResult
            {
                Success = false,
                Channel = channel,
                Recipient = recipient,
                Subject = subject,
                ErrorMessage = errorMessage
            };
        }
    }
}
