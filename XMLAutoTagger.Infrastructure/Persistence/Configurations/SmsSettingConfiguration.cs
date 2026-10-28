using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Persistence.Configurations
{
    public class SmsSettingConfiguration : IEntityTypeConfiguration<SmsSetting>
    {
        public void Configure(EntityTypeBuilder<SmsSetting> builder)
        {
            builder.ToTable("sms_settings");
            builder.HasKey(s => s.SmsSettingId);
            builder.Property(s => s.SmsSettingId).HasColumnName("sms_setting_id");
            builder.Property(s => s.Provider).HasColumnName("provider_name").HasMaxLength(50).IsRequired();
            builder.Property(s => s.ApiUrl).HasColumnName("api_endpoint_url").HasMaxLength(500);
            builder.Property(s => s.AccountSid).HasColumnName("account_sid").HasMaxLength(150);
            builder.Property(s => s.AuthToken).HasColumnName("auth_token_encrypted");
            builder.Property(s => s.ApiKey).HasColumnName("api_key_encrypted");
            builder.Property(s => s.SenderId).HasColumnName("sender_id").HasMaxLength(50);
            builder.Property(s => s.IsActive).HasColumnName("is_active");
            builder.Property(s => s.IsDefault).HasColumnName("is_default");
            builder.Property(s => s.CreatedAt).HasColumnName("created_at");
            builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
            builder.Property(s => s.CreatedBy).HasColumnName("created_by");
            builder.Property(s => s.UpdatedBy).HasColumnName("updated_by");
        }
    }
}
