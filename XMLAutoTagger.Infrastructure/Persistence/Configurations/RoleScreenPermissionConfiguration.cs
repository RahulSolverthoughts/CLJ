using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Persistence.Configurations
{
    public class RoleScreenPermissionConfiguration : IEntityTypeConfiguration<RoleScreenPermission>
    {
        public void Configure(EntityTypeBuilder<RoleScreenPermission> builder)
        {
            builder.ToTable("role_screen_permissions");
            builder.HasKey(p => p.RspId);
            builder.Property(p => p.RspId).HasColumnName("rsp_id");
            builder.Property(p => p.RoleId).HasColumnName("role_id");
            builder.Property(p => p.ScreenId).HasColumnName("screen_id");
            builder.Property(p => p.PermLevel).HasColumnName("perm_level").HasMaxLength(50);

            builder.Property(p => p.CreatedAt).HasColumnName("created_at");
            builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            builder.Property(p => p.CreatedBy).HasColumnName("created_by");
            builder.Property(p => p.UpdatedBy).HasColumnName("updated_by");

            builder.HasOne(p => p.Role)
                .WithMany(r => r.Permissions)
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Screen)
                .WithMany()
                .HasForeignKey(p => p.ScreenId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
