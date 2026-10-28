using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Application.Common.Models.Communication;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Enums;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.Infrastructure.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly IApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CommunicationService> _logger;

        public CommunicationService(
            IApplicationDbContext context,
            HttpClient httpClient,
            ILogger<CommunicationService> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CommunicationResult> SendEmailAsync(EmailRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.To))
                return CommunicationResult.Failed(CommunicationChannel.Email, request.To, "Recipient email address ('To') cannot be empty.", request.Subject);

            try
            {
                var setting = await _context.EmailSettings
                    .FirstOrDefaultAsync(s => s.IsActive && s.IsDefault, ct)
                    ?? await _context.EmailSettings.FirstOrDefaultAsync(s => s.IsActive, ct);

                if (setting == null)
                {
                    return CommunicationResult.Failed(CommunicationChannel.Email, request.To, "No active Email configuration found in email_settings table.", request.Subject);
                }

                var senderEmail = !string.IsNullOrWhiteSpace(request.SenderEmailOverride) ? request.SenderEmailOverride : setting.SenderEmail;
                var senderName = !string.IsNullOrWhiteSpace(request.SenderNameOverride) ? request.SenderNameOverride : setting.SenderDisplayName;

                _logger.LogInformation("Sending Email via {Provider} to {To} with Subject: {Subject}", setting.Provider, request.To, request.Subject);

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = request.Subject,
                    Body = request.Body,
                    IsBodyHtml = request.IsHtml
                };
                mailMessage.To.Add(request.To);

                using var smtpClient = new SmtpClient(setting.SmtpHost, setting.SmtpPort)
                {
                    EnableSsl = setting.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                if (!string.IsNullOrWhiteSpace(setting.Username) && !string.IsNullOrWhiteSpace(setting.Password))
                {
                    smtpClient.Credentials = new NetworkCredential(setting.Username, setting.Password);
                }

                await smtpClient.SendMailAsync(mailMessage, ct);

                var messageId = Guid.NewGuid().ToString("N");
                _logger.LogInformation("Email sent successfully to {To}. MessageId: {MessageId}", request.To, messageId);

                return CommunicationResult.Succeeded(CommunicationChannel.Email, request.To, messageId, request.Subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", request.To);
                return CommunicationResult.Failed(CommunicationChannel.Email, request.To, ex.Message, request.Subject);
            }
        }

        public async Task<CommunicationResult> SendSmsAsync(SmsRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.To))
                return CommunicationResult.Failed(CommunicationChannel.Sms, request.To, "Recipient phone number ('To') cannot be empty.", request.Subject);

            try
            {
                var setting = await _context.SmsSettings
                    .FirstOrDefaultAsync(s => s.IsActive && s.IsDefault, ct)
                    ?? await _context.SmsSettings.FirstOrDefaultAsync(s => s.IsActive, ct);

                if (setting == null)
                {
                    return CommunicationResult.Failed(CommunicationChannel.Sms, request.To, "No active SMS configuration found in sms_settings table.", request.Subject);
                }

                var senderId = !string.IsNullOrWhiteSpace(request.SenderIdOverride) ? request.SenderIdOverride : setting.SenderId ?? "LegalXML";
                var messageBody = !string.IsNullOrWhiteSpace(request.Subject)
                    ? $"[{request.Subject}] {request.Message}"
                    : request.Message;

                _logger.LogInformation("Sending SMS via {Provider} to {To} from {SenderId}", setting.Provider, request.To, senderId);

                if (setting.Provider.Equals("Twilio", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(setting.AccountSid) || string.IsNullOrWhiteSpace(setting.AuthToken))
                    {
                        return CommunicationResult.Failed(CommunicationChannel.Sms, request.To, "Twilio AccountSid or AuthToken is missing in sms_settings.", request.Subject);
                    }

                    var twilioUrl = $"{setting.ApiUrl?.TrimEnd('/') ?? "https://api.twilio.com/2010-04-01"}/Accounts/{setting.AccountSid}/Messages.json";
                    var formData = new Dictionary<string, string>
                    {
                        { "To", request.To },
                        { "From", senderId },
                        { "Body", messageBody }
                    };

                    using var httpRequest = new HttpRequestMessage(HttpMethod.Post, twilioUrl)
                    {
                        Content = new FormUrlEncodedContent(formData)
                    };

                    var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{setting.AccountSid}:{setting.AuthToken}"));
                    httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

                    var response = await _httpClient.SendAsync(httpRequest, ct);
                    var responseContent = await response.Content.ReadAsStringAsync(ct);

                    if (response.IsSuccessStatusCode)
                    {
                        using var doc = JsonDocument.Parse(responseContent);
                        var sid = doc.RootElement.TryGetProperty("sid", out var sidProp) ? sidProp.GetString() : Guid.NewGuid().ToString("N");
                        return CommunicationResult.Succeeded(CommunicationChannel.Sms, request.To, sid, request.Subject);
                    }
                    else
                    {
                        _logger.LogWarning("Twilio SMS failed with HTTP {Status}: {Body}", response.StatusCode, responseContent);
                        return CommunicationResult.Failed(CommunicationChannel.Sms, request.To, $"SMS Gateway Error ({response.StatusCode}): {responseContent}", request.Subject);
                    }
                }
                else
                {
                    // Generic HTTP Gateway
                    var payload = new
                    {
                        to = request.To,
                        from = senderId,
                        message = messageBody,
                        api_key = setting.ApiKey
                    };

                    var endpoint = setting.ApiUrl ?? "https://api.sms-gateway.example/send";
                    var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync(endpoint, jsonContent, ct);
                    var responseBody = await response.Content.ReadAsStringAsync(ct);

                    if (response.IsSuccessStatusCode)
                    {
                        return CommunicationResult.Succeeded(CommunicationChannel.Sms, request.To, Guid.NewGuid().ToString("N"), request.Subject);
                    }

                    return CommunicationResult.Failed(CommunicationChannel.Sms, request.To, $"SMS Gateway HTTP {response.StatusCode}: {responseBody}", request.Subject);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS to {To}", request.To);
                return CommunicationResult.Failed(CommunicationChannel.Sms, request.To, ex.Message, request.Subject);
            }
        }

        public async Task<CommunicationResult> SendWhatsAppAsync(WhatsAppRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.To))
                return CommunicationResult.Failed(CommunicationChannel.WhatsApp, request.To, "Recipient WhatsApp number ('To') cannot be empty.", request.Subject);

            try
            {
                var setting = await _context.WhatsAppSettings
                    .FirstOrDefaultAsync(s => s.IsActive && s.IsDefault, ct)
                    ?? await _context.WhatsAppSettings.FirstOrDefaultAsync(s => s.IsActive, ct);

                if (setting == null)
                {
                    return CommunicationResult.Failed(CommunicationChannel.WhatsApp, request.To, "No active WhatsApp configuration found in whatsapp_settings table.", request.Subject);
                }

                if (string.IsNullOrWhiteSpace(setting.PhoneNumberId) || string.IsNullOrWhiteSpace(setting.AccessToken))
                {
                    return CommunicationResult.Failed(CommunicationChannel.WhatsApp, request.To, "WhatsApp PhoneNumberId or AccessToken is missing in whatsapp_settings.", request.Subject);
                }

                // Format phone number (Meta requires digits only without leading + or spaces)
                var cleanRecipient = request.To.Replace("+", "").Replace(" ", "").Replace("-", "");
                var messageBody = !string.IsNullOrWhiteSpace(request.Subject)
                    ? $"*{request.Subject}*\n\n{request.Message}"
                    : request.Message;

                _logger.LogInformation("Sending WhatsApp message via Meta Cloud API to {To}", cleanRecipient);

                var baseUrl = setting.ApiUrl?.TrimEnd('/') ?? "https://graph.facebook.com/v19.0";
                var url = $"{baseUrl}/{setting.PhoneNumberId}/messages";

                object payload;
                if (!string.IsNullOrWhiteSpace(request.TemplateName))
                {
                    payload = new
                    {
                        messaging_product = "whatsapp",
                        recipient_type = "individual",
                        to = cleanRecipient,
                        type = "template",
                        template = new
                        {
                            name = request.TemplateName,
                            language = new { code = "en_US" }
                        }
                    };
                }
                else
                {
                    payload = new
                    {
                        messaging_product = "whatsapp",
                        recipient_type = "individual",
                        to = cleanRecipient,
                        type = "text",
                        text = new
                        {
                            preview_url = false,
                            body = messageBody
                        }
                    };
                }

                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
                };
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", setting.AccessToken);

                var response = await _httpClient.SendAsync(httpRequest, ct);
                var responseContent = await response.Content.ReadAsStringAsync(ct);

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    var msgId = Guid.NewGuid().ToString("N");
                    if (doc.RootElement.TryGetProperty("messages", out var messagesProp) && messagesProp.GetArrayLength() > 0)
                    {
                        msgId = messagesProp[0].GetProperty("id").GetString() ?? msgId;
                    }

                    _logger.LogInformation("WhatsApp message sent successfully to {To}. MessageId: {MessageId}", cleanRecipient, msgId);
                    return CommunicationResult.Succeeded(CommunicationChannel.WhatsApp, request.To, msgId, request.Subject);
                }
                else
                {
                    _logger.LogWarning("Meta WhatsApp API failed with HTTP {Status}: {Body}", response.StatusCode, responseContent);
                    return CommunicationResult.Failed(CommunicationChannel.WhatsApp, request.To, $"WhatsApp API Error ({response.StatusCode}): {responseContent}", request.Subject);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send WhatsApp message to {To}", request.To);
                return CommunicationResult.Failed(CommunicationChannel.WhatsApp, request.To, ex.Message, request.Subject);
            }
        }

        public async Task<CommunicationResult> SendAsync(UnifiedCommunicationRequest request, CancellationToken ct = default)
        {
            switch (request.Channel)
            {
                case CommunicationChannel.Email:
                    return await SendEmailAsync(new EmailRequest
                    {
                        To = request.To,
                        Subject = request.Subject ?? "Notification",
                        Body = request.Message,
                        IsHtml = request.IsHtml,
                        SenderEmailOverride = request.SenderOverride
                    }, ct);

                case CommunicationChannel.Sms:
                    return await SendSmsAsync(new SmsRequest
                    {
                        To = request.To,
                        Subject = request.Subject,
                        Message = request.Message,
                        SenderIdOverride = request.SenderOverride
                    }, ct);

                case CommunicationChannel.WhatsApp:
                    return await SendWhatsAppAsync(new WhatsAppRequest
                    {
                        To = request.To,
                        Subject = request.Subject,
                        Message = request.Message
                    }, ct);

                default:
                    return CommunicationResult.Failed(request.Channel, request.To, $"Unsupported communication channel: {request.Channel}", request.Subject);
            }
        }

        public async Task<CommunicationResult> SendEventNotificationAsync(
            XMLAutoTagger.Domain.Enums.CommunicationEvent eventType,
            string recipient,
            IDictionary<string, string> templateData,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(recipient))
            {
                return CommunicationResult.Failed(CommunicationChannel.Email, recipient, "Recipient address cannot be empty.", eventType.ToString());
            }

            try
            {
                var eventId = (int)eventType;
                var eventItem = await _context.CommunicationEvents
                    .FirstOrDefaultAsync(e => e.EventId == eventId && e.IsActive, ct);

                if (eventItem == null)
                {
                    _logger.LogWarning("No active communication event found in communication_events table for EventId {EventId} ({EventType})", eventId, eventType);
                    return CommunicationResult.Failed(CommunicationChannel.Email, recipient, $"Event {eventType} (ID {eventId}) is not configured or is inactive in communication_events table.", eventType.ToString());
                }

                // Default fallback variables
                var mergedData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["AppName"] = "LegalXML",
                    ["SupportEmail"] = "admin@legalxml.my",
                    ["RecipientEmail"] = recipient,
                    ["CurrentYear"] = DateTime.UtcNow.Year.ToString()
                };

                if (templateData != null)
                {
                    foreach (var kvp in templateData)
                    {
                        mergedData[kvp.Key] = kvp.Value ?? string.Empty;
                    }
                }

                // Replace {{Placeholder}} tokens dynamically from database template
                var resolvedSubject = ReplaceTokens(eventItem.EmailSubject, mergedData);
                var resolvedBody = ReplaceTokens(eventItem.EmailBody, mergedData);

                var emailReq = new EmailRequest
                {
                    To = recipient,
                    Subject = resolvedSubject,
                    Body = resolvedBody,
                    IsHtml = true
                };

                return await SendEmailAsync(emailReq, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event notification for {EventType} to {Recipient}", eventType, recipient);
                return CommunicationResult.Failed(CommunicationChannel.Email, recipient, ex.Message, eventType.ToString());
            }
        }

        private static string ReplaceTokens(string template, IDictionary<string, string> data)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;

            var sb = new StringBuilder(template);
            foreach (var kvp in data)
            {
                sb.Replace("{{" + kvp.Key + "}}", kvp.Value);
            }
            return sb.ToString();
        }
    }
}
