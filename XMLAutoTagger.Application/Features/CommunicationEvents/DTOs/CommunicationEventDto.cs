using System;

namespace XMLAutoTagger.Application.Features.CommunicationEvents.DTOs
{
    public class CommunicationEventDto
    {
        public int EventId { get; set; }
        public string EventCode { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Channel { get; set; } = "Email";
        public string EmailSubject { get; set; } = string.Empty;
        public string EmailBody { get; set; } = string.Empty;
        public string? SmsBody { get; set; }
        public string? WhatsAppBody { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
