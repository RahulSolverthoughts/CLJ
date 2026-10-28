using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Application.Common.Models.Communication;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.WebAPI.Controllers.Api
{
    [ApiController]
    [Route("api/v1/communication")]
    [Produces("application/json")]
    public class CommunicationTestController : ControllerBase
    {
        private readonly ICommunicationService _communicationService;
        private readonly IApplicationDbContext _context;

        public CommunicationTestController(
            ICommunicationService communicationService,
            IApplicationDbContext context)
        {
            _communicationService = communicationService;
            _context = context;
        }

        /// <summary>
        /// Send a test Email with support for overriding To and Subject.
        /// </summary>
        [HttpPost("test/email")]
        [AllowAnonymous]
        public async Task<ActionResult<CommunicationResult>> TestEmail(
            [FromBody] EmailRequest request,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.To))
            {
                return BadRequest(new { message = "Recipient 'To' address is required." });
            }

            if (string.IsNullOrWhiteSpace(request.Subject))
            {
                request.Subject = "Test Email from LegalXML XMLAutoTagger";
            }

            if (string.IsNullOrWhiteSpace(request.Body))
            {
                request.Body = "<h2>LegalXML Notification Test</h2><p>This is a test email sent via the unified Communication Service.</p>";
            }

            var result = await _communicationService.SendEmailAsync(request, ct);
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Send a test SMS with support for overriding To and Subject/Message.
        /// </summary>
        [HttpPost("test/sms")]
        [AllowAnonymous]
        public async Task<ActionResult<CommunicationResult>> TestSms(
            [FromBody] SmsRequest request,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.To))
            {
                return BadRequest(new { message = "Recipient phone number ('To') is required." });
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                request.Message = "Test SMS notification from LegalXML XMLAutoTagger.";
            }

            var result = await _communicationService.SendSmsAsync(request, ct);
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Send a test WhatsApp message with support for overriding To and Message/Subject.
        /// </summary>
        [HttpPost("test/whatsapp")]
        [AllowAnonymous]
        public async Task<ActionResult<CommunicationResult>> TestWhatsApp(
            [FromBody] WhatsAppRequest request,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.To))
            {
                return BadRequest(new { message = "Recipient WhatsApp number ('To') is required." });
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                request.Message = "Hello from LegalXML XMLAutoTagger via Meta WhatsApp Cloud API!";
            }

            var result = await _communicationService.SendWhatsAppAsync(request, ct);
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Unified test endpoint dispatching to Email (1), Sms (2), or WhatsApp (3) with To and Subject overrides.
        /// </summary>
        [HttpPost("test/unified")]
        [AllowAnonymous]
        public async Task<ActionResult<CommunicationResult>> TestUnified(
            [FromBody] UnifiedCommunicationRequest request,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.To))
            {
                return BadRequest(new { message = "Recipient 'To' address or phone is required." });
            }

            var result = await _communicationService.SendAsync(request, ct);
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieve current communication channel configurations from separate database tables (sensitive values masked).
        /// </summary>
        [HttpGet("settings")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSettings(CancellationToken ct)
        {
            var emailSetting = await _context.EmailSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IsActive && s.IsDefault, ct);

            var smsSetting = await _context.SmsSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IsActive && s.IsDefault, ct);

            var whatsAppSetting = await _context.WhatsAppSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IsActive && s.IsDefault, ct);

            return Ok(new
            {
                Email = emailSetting != null ? new
                {
                    emailSetting.EmailSettingId,
                    emailSetting.Provider,
                    emailSetting.SmtpHost,
                    emailSetting.SmtpPort,
                    emailSetting.EnableSsl,
                    emailSetting.SenderEmail,
                    emailSetting.SenderDisplayName,
                    emailSetting.Username,
                    HasPasswordConfigured = !string.IsNullOrWhiteSpace(emailSetting.Password),
                    emailSetting.IsActive,
                    emailSetting.IsDefault
                } : null,
                Sms = smsSetting != null ? new
                {
                    smsSetting.SmsSettingId,
                    smsSetting.Provider,
                    smsSetting.ApiUrl,
                    smsSetting.AccountSid,
                    smsSetting.SenderId,
                    HasAuthTokenConfigured = !string.IsNullOrWhiteSpace(smsSetting.AuthToken),
                    smsSetting.IsActive,
                    smsSetting.IsDefault
                } : null,
                WhatsApp = whatsAppSetting != null ? new
                {
                    whatsAppSetting.WhatsAppSettingId,
                    whatsAppSetting.Provider,
                    whatsAppSetting.ApiUrl,
                    whatsAppSetting.PhoneNumberId,
                    whatsAppSetting.BusinessAccountId,
                    HasAccessTokenConfigured = !string.IsNullOrWhiteSpace(whatsAppSetting.AccessToken),
                    whatsAppSetting.IsActive,
                    whatsAppSetting.IsDefault
                } : null
            });
        }

        /// <summary>
        /// Update Email SMTP configuration in email_settings table.
        /// </summary>
        [HttpPut("settings/email")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateEmailSettings(
            [FromBody] UpdateEmailSettingDto dto,
            CancellationToken ct)
        {
            var setting = await _context.EmailSettings
                .FirstOrDefaultAsync(s => s.IsActive && s.IsDefault, ct)
                ?? await _context.EmailSettings.FirstOrDefaultAsync(ct);

            if (setting == null)
            {
                setting = new XMLAutoTagger.Domain.Entities.EmailSetting
                {
                    EmailSettingId = Guid.NewGuid(),
                    IsActive = true,
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.EmailSettings.Add(setting);
            }

            setting.Provider = string.IsNullOrWhiteSpace(dto.Provider) ? "SMTP" : dto.Provider;
            setting.SmtpHost = dto.SmtpHost;
            setting.SmtpPort = dto.SmtpPort > 0 ? dto.SmtpPort : 587;
            setting.EnableSsl = dto.EnableSsl;
            setting.SenderEmail = dto.SenderEmail;
            setting.SenderDisplayName = string.IsNullOrWhiteSpace(dto.SenderDisplayName) ? dto.SenderEmail : dto.SenderDisplayName;
            setting.Username = dto.Username;
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                setting.Password = dto.Password;
            }
            setting.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Ok(new
            {
                message = "Email settings updated successfully.",
                settingId = setting.EmailSettingId,
                host = setting.SmtpHost,
                port = setting.SmtpPort,
                senderEmail = setting.SenderEmail,
                senderName = setting.SenderDisplayName,
                username = setting.Username,
                hasPassword = !string.IsNullOrWhiteSpace(setting.Password)
            });
        }

        /// <summary>
        /// Update SMS configuration in sms_settings table.
        /// </summary>
        [HttpPut("settings/sms")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateSmsSettings(
            [FromBody] UpdateSmsSettingDto dto,
            CancellationToken ct)
        {
            var setting = await _context.SmsSettings
                .FirstOrDefaultAsync(s => s.IsActive && s.IsDefault, ct)
                ?? await _context.SmsSettings.FirstOrDefaultAsync(ct);

            if (setting == null)
            {
                setting = new XMLAutoTagger.Domain.Entities.SmsSetting
                {
                    SmsSettingId = Guid.NewGuid(),
                    IsActive = true,
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.SmsSettings.Add(setting);
            }

            setting.Provider = string.IsNullOrWhiteSpace(dto.Provider) ? "Twilio" : dto.Provider;
            setting.ApiUrl = dto.ApiUrl;
            setting.AccountSid = dto.AccountSid;
            if (!string.IsNullOrWhiteSpace(dto.AuthToken))
            {
                setting.AuthToken = dto.AuthToken;
            }
            if (!string.IsNullOrWhiteSpace(dto.ApiKey))
            {
                setting.ApiKey = dto.ApiKey;
            }
            setting.SenderId = dto.SenderId;
            setting.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Ok(new
            {
                message = "SMS settings updated successfully.",
                settingId = setting.SmsSettingId,
                provider = setting.Provider,
                apiUrl = setting.ApiUrl,
                accountSid = setting.AccountSid,
                senderId = setting.SenderId,
                hasAuthToken = !string.IsNullOrWhiteSpace(setting.AuthToken)
            });
        }

        /// <summary>
        /// Update WhatsApp configuration in whatsapp_settings table.
        /// </summary>
        [HttpPut("settings/whatsapp")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateWhatsAppSettings(
            [FromBody] UpdateWhatsAppSettingDto dto,
            CancellationToken ct)
        {
            var setting = await _context.WhatsAppSettings
                .FirstOrDefaultAsync(s => s.IsActive && s.IsDefault, ct)
                ?? await _context.WhatsAppSettings.FirstOrDefaultAsync(ct);

            if (setting == null)
            {
                setting = new XMLAutoTagger.Domain.Entities.WhatsAppSetting
                {
                    WhatsAppSettingId = Guid.NewGuid(),
                    IsActive = true,
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.WhatsAppSettings.Add(setting);
            }

            setting.Provider = string.IsNullOrWhiteSpace(dto.Provider) ? "MetaCloudApi" : dto.Provider;
            setting.ApiUrl = string.IsNullOrWhiteSpace(dto.ApiUrl) ? "https://graph.facebook.com/v19.0" : dto.ApiUrl;
            setting.PhoneNumberId = dto.PhoneNumberId;
            if (!string.IsNullOrWhiteSpace(dto.AccessToken))
            {
                setting.AccessToken = dto.AccessToken;
            }
            setting.BusinessAccountId = dto.BusinessAccountId;
            setting.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Ok(new
            {
                message = "WhatsApp settings updated successfully.",
                settingId = setting.WhatsAppSettingId,
                provider = setting.Provider,
                apiUrl = setting.ApiUrl,
                phoneNumberId = setting.PhoneNumberId,
                businessAccountId = setting.BusinessAccountId,
                hasAccessToken = !string.IsNullOrWhiteSpace(setting.AccessToken)
            });
        }
    }

    public class UpdateEmailSettingDto
    {
        public string? Provider { get; set; } = "SMTP";
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderDisplayName { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class UpdateSmsSettingDto
    {
        public string? Provider { get; set; } = "Twilio";
        public string? ApiUrl { get; set; }
        public string? AccountSid { get; set; }
        public string? AuthToken { get; set; }
        public string? ApiKey { get; set; }
        public string? SenderId { get; set; }
    }

    public class UpdateWhatsAppSettingDto
    {
        public string? Provider { get; set; } = "MetaCloudApi";
        public string? ApiUrl { get; set; } = "https://graph.facebook.com/v19.0";
        public string PhoneNumberId { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string? BusinessAccountId { get; set; }
    }
}
