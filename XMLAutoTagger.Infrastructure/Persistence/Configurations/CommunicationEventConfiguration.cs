using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Persistence.Configurations
{
    public class CommunicationEventConfiguration : IEntityTypeConfiguration<CommunicationEventItem>
    {
        public void Configure(EntityTypeBuilder<CommunicationEventItem> builder)
        {
            builder.ToTable("communication_events");
            builder.HasKey(e => e.EventId);
            builder.Property(e => e.EventId).HasColumnName("event_id").ValueGeneratedNever();
            builder.Property(e => e.EventCode).HasColumnName("event_code").HasMaxLength(200).IsRequired();
            builder.Property(e => e.EventName).HasColumnName("event_name").HasMaxLength(200).IsRequired();
            builder.Property(e => e.Description).HasColumnName("description");
            builder.Property(e => e.Channel).HasColumnName("channel").HasMaxLength(50).IsRequired().HasDefaultValue("Email");
            builder.Property(e => e.EmailSubject).HasColumnName("email_subject").HasMaxLength(500).IsRequired();
            builder.Property(e => e.EmailBody).HasColumnName("email_body").IsRequired();
            builder.Property(e => e.SmsBody).HasColumnName("sms_body");
            builder.Property(e => e.WhatsAppBody).HasColumnName("whatsapp_body");
            builder.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            builder.Property(e => e.CreatedAt).HasColumnName("created_at");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            builder.Property(e => e.CreatedBy).HasColumnName("created_by");
            builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            builder.HasIndex(e => e.EventCode).IsUnique();
        }
    }
}
