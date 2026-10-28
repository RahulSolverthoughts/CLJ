using System.Threading;
using System.Threading.Tasks;
using XMLAutoTagger.Application.Common.Models.Communication;

namespace XMLAutoTagger.Application.Common.Interfaces
{
    public interface ICommunicationService
    {
        /// <summary>
        /// Sends an Email using settings from email_settings table, with support for overriding To and Subject.
        /// </summary>
        Task<CommunicationResult> SendEmailAsync(EmailRequest request, CancellationToken ct = default);

        /// <summary>
        /// Sends an SMS using settings from sms_settings table, with support for overriding To and Message.
        /// </summary>
        Task<CommunicationResult> SendSmsAsync(SmsRequest request, CancellationToken ct = default);

        /// <summary>
        /// Sends a WhatsApp message using settings from whatsapp_settings table, with support for overriding To and Message.
        /// </summary>
        Task<CommunicationResult> SendWhatsAppAsync(WhatsAppRequest request, CancellationToken ct = default);

        /// <summary>
        /// Unified dispatcher that automatically routes to Email, SMS, or WhatsApp with To and Subject overrides.
        /// </summary>
        Task<CommunicationResult> SendAsync(UnifiedCommunicationRequest request, CancellationToken ct = default);

        /// <summary>
        /// Dynamically resolves the template from user_mgmt.communication_events by event enum ID,
        /// performs dynamic {{Placeholder}} token replacement, and sends the notification.
        /// </summary>
        Task<CommunicationResult> SendEventNotificationAsync(
            XMLAutoTagger.Domain.Enums.CommunicationEvent eventType,
            string recipient,
            System.Collections.Generic.IDictionary<string, string> templateData,
            CancellationToken ct = default);
    }
}
