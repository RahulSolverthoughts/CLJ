using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Interfaces;

namespace XMLAutoTagger.WebAPI.Pages.Settings
{
    public class CommunicationModel : PageModel
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permissionService;
        private readonly ICurrentUserService _currentUserService;

        public CommunicationModel(
            IApplicationDbContext context,
            IPermissionService permissionService,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _permissionService = permissionService;
            _currentUserService = currentUserService;
        }

        public EmailSetting? EmailSetting { get; set; }
        public SmsSetting? SmsSetting { get; set; }
        public WhatsAppSetting? WhatsAppSetting { get; set; }

        public string ActiveTab { get; set; } = "email";

        public bool CanViewEmail { get; set; } = true;
        public bool CanEditEmail { get; set; } = true;

        public bool CanViewSms { get; set; } = true;
        public bool CanEditSms { get; set; } = true;

        public bool CanViewWhatsApp { get; set; } = true;
        public bool CanEditWhatsApp { get; set; } = true;

        public async Task<IActionResult> OnGetAsync(string? tab)
        {
            ViewData["Title"] = "Settings — Communication Channels";
            ViewData["ActivePage"] = "Communication";
            ViewData["PageHeading"] = "Communication settings";
            ViewData["PageSubtitle"] = "Configure and test unified Email, SMS, and WhatsApp dispatch providers";

            if (!string.IsNullOrWhiteSpace(tab))
            {
                var lower = tab.ToLowerInvariant();
                if (lower == "sms" || lower == "whatsapp" || lower == "email")
                {
                    ActiveTab = lower;
                }
            }

            var userId = _currentUserService.UserId;
            if (userId.HasValue)
            {
                var emailPerm = await _permissionService.GetEffectivePermissionAsync(userId.Value, "email_settings");
                var smsPerm = await _permissionService.GetEffectivePermissionAsync(userId.Value, "sms_settings");
                var waPerm = await _permissionService.GetEffectivePermissionAsync(userId.Value, "whatsapp_settings");

                CanViewEmail = emailPerm != "none";
                CanEditEmail = emailPerm == "edit" || emailPerm == "approve" || emailPerm == "admin";

                CanViewSms = smsPerm != "none";
                CanEditSms = smsPerm == "edit" || smsPerm == "approve" || smsPerm == "admin";

                CanViewWhatsApp = waPerm != "none";
                CanEditWhatsApp = waPerm == "edit" || waPerm == "approve" || waPerm == "admin";
            }

            EmailSetting = await _context.EmailSettings
                .FirstOrDefaultAsync(s => s.IsActive && s.IsDefault)
                ?? await _context.EmailSettings.FirstOrDefaultAsync();

            SmsSetting = await _context.SmsSettings
                .FirstOrDefaultAsync(s => s.IsActive && s.IsDefault)
                ?? await _context.SmsSettings.FirstOrDefaultAsync();

            WhatsAppSetting = await _context.WhatsAppSettings
                .FirstOrDefaultAsync(s => s.IsActive && s.IsDefault)
                ?? await _context.WhatsAppSettings.FirstOrDefaultAsync();

            // Default to first accessible tab if active tab isn't viewable
            if (ActiveTab == "email" && !CanViewEmail)
            {
                ActiveTab = CanViewSms ? "sms" : (CanViewWhatsApp ? "whatsapp" : "email");
            }
            else if (ActiveTab == "sms" && !CanViewSms)
            {
                ActiveTab = CanViewEmail ? "email" : (CanViewWhatsApp ? "whatsapp" : "sms");
            }
            else if (ActiveTab == "whatsapp" && !CanViewWhatsApp)
            {
                ActiveTab = CanViewEmail ? "email" : (CanViewSms ? "sms" : "whatsapp");
            }

            return Page();
        }
    }
}
