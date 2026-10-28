using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");
            builder.HasKey(r => r.RoleId);
            builder.Property(r => r.RoleId).HasColumnName("role_id");
            builder.Property(r => r.RoleCode).HasColumnName("role_code").HasMaxLength(50).IsRequired();
            builder.Property(r => r.RoleName).HasColumnName("role_name").HasMaxLength(100).IsRequired();
            builder.Property(r => r.Description).HasColumnName("description");
            builder.Property(r => r.IsSystem).HasColumnName("is_system");
            builder.Property(r => r.BaseRoleId).HasColumnName("base_role_id");
            builder.Property(r => r.IsActive).HasColumnName("is_active");

            builder.Property(r => r.CreatedAt).HasColumnName("created_at");
            builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            builder.Property(r => r.CreatedBy).HasColumnName("created_by");
            builder.Property(r => r.UpdatedBy).HasColumnName("updated_by");

            builder.HasOne(r => r.BaseRole)
                .WithMany()
                .HasForeignKey(r => r.BaseRoleId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(r => r.Permissions)
                .WithOne(p => p.Role)
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId);
        }
    }
}
