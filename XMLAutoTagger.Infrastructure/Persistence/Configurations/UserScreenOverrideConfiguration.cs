using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Persistence.Configurations
{
    public class UserScreenOverrideConfiguration : IEntityTypeConfiguration<UserScreenOverride>
    {
        public void Configure(EntityTypeBuilder<UserScreenOverride> builder)
        {
            builder.ToTable("user_screen_permissions");
            builder.HasKey(o => o.UspId);
            builder.Property(o => o.UspId).HasColumnName("usp_id");
            builder.Property(o => o.UserId).HasColumnName("users_id");
            builder.Property(o => o.ScreenId).HasColumnName("screen_id");
            builder.Property(o => o.PermLevel).HasColumnName("perm_level").HasMaxLength(50);
            builder.Property(o => o.ValidFrom).HasColumnName("valid_from");
            builder.Property(o => o.ValidUntil).HasColumnName("valid_until");
            builder.Property(o => o.OverrideReason).HasColumnName("override_reason");
            builder.Property(o => o.GrantedBy).HasColumnName("granted_by");
            builder.Property(o => o.UpdatedAt).HasColumnName("updated_at");

            builder.HasOne(o => o.User)
                .WithMany(u => u.ScreenOverrides)
                .HasForeignKey(o => o.UserId);

            builder.HasOne(o => o.Screen)
                .WithMany()
                .HasForeignKey(o => o.ScreenId);
        }
    }
}
