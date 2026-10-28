using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Persistence.Configurations
{
    public class WhatsAppSettingConfiguration : IEntityTypeConfiguration<WhatsAppSetting>
    {
        public void Configure(EntityTypeBuilder<WhatsAppSetting> builder)
        {
            builder.ToTable("whatsapp_settings");
            builder.HasKey(w => w.WhatsAppSettingId);
            builder.Property(w => w.WhatsAppSettingId).HasColumnName("whatsapp_setting_id");
            builder.Property(w => w.Provider).HasColumnName("provider_name").HasMaxLength(50).IsRequired();
            builder.Property(w => w.ApiUrl).HasColumnName("api_endpoint_url").HasMaxLength(500);
            builder.Property(w => w.PhoneNumberId).HasColumnName("phone_number_id").HasMaxLength(100).IsRequired();
            builder.Property(w => w.AccessToken).HasColumnName("access_token_encrypted").IsRequired();
            builder.Property(w => w.BusinessAccountId).HasColumnName("business_account_id").HasMaxLength(100);
            builder.Property(w => w.IsActive).HasColumnName("is_active");
            builder.Property(w => w.IsDefault).HasColumnName("is_default");
            builder.Property(w => w.CreatedAt).HasColumnName("created_at");
            builder.Property(w => w.UpdatedAt).HasColumnName("updated_at");
            builder.Property(w => w.CreatedBy).HasColumnName("created_by");
            builder.Property(w => w.UpdatedBy).HasColumnName("updated_by");
        }
    }
}
