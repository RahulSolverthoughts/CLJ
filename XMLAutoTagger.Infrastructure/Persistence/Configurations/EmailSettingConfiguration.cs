using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Persistence.Configurations
{
    public class EmailSettingConfiguration : IEntityTypeConfiguration<EmailSetting>
    {
        public void Configure(EntityTypeBuilder<EmailSetting> builder)
        {
            builder.ToTable("email_settings");
            builder.HasKey(e => e.EmailSettingId);
            builder.Property(e => e.EmailSettingId).HasColumnName("email_setting_id");
            builder.Property(e => e.Provider).HasColumnName("provider_name").HasMaxLength(50).IsRequired();
            builder.Property(e => e.SmtpHost).HasColumnName("smtp_host").HasMaxLength(255).IsRequired();
            builder.Property(e => e.SmtpPort).HasColumnName("smtp_port");
            builder.Property(e => e.EnableSsl).HasColumnName("enable_ssl");
            builder.Property(e => e.SenderEmail).HasColumnName("sender_email").HasMaxLength(255).IsRequired();
            builder.Property(e => e.SenderDisplayName).HasColumnName("sender_name").HasMaxLength(150);
            builder.Property(e => e.Username).HasColumnName("username").HasMaxLength(255);
            builder.Property(e => e.Password).HasColumnName("password_encrypted");
            builder.Property(e => e.ApiKey).HasColumnName("api_key_encrypted");
            builder.Property(e => e.IsActive).HasColumnName("is_active");
            builder.Property(e => e.IsDefault).HasColumnName("is_default");
            builder.Property(e => e.CreatedAt).HasColumnName("created_at");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            builder.Property(e => e.CreatedBy).HasColumnName("created_by");
            builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        }
    }
}
