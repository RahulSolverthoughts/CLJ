using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Persistence.Configurations
{
    public class ScreenConfiguration : IEntityTypeConfiguration<Screen>
    {
        public void Configure(EntityTypeBuilder<Screen> builder)
        {
            builder.ToTable("screens");
            builder.HasKey(s => s.ScreenId);
            builder.Property(s => s.ScreenId).HasColumnName("screen_id");
            builder.Property(s => s.ScreenCode).HasColumnName("screen_code").HasMaxLength(80).IsRequired();
            builder.Property(s => s.ScreenName).HasColumnName("screen_name").HasMaxLength(120).IsRequired();
            builder.Property(s => s.ScreenGroup).HasColumnName("screen_group").HasMaxLength(80).IsRequired();
            builder.Property(s => s.IconName).HasColumnName("icon_name").HasMaxLength(60);
            builder.Property(s => s.RoutePath).HasColumnName("route_path").HasMaxLength(255);
            builder.Property(s => s.Description).HasColumnName("description");
            builder.Property(s => s.SortOrder).HasColumnName("sort_order");
            builder.Property(s => s.IsActive).HasColumnName("is_active");

            builder.Property(s => s.CreatedAt).HasColumnName("created_at");
            builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
            builder.Property(s => s.CreatedBy).HasColumnName("created_by");
            builder.Property(s => s.UpdatedBy).HasColumnName("updated_by");
        }
    }
}
