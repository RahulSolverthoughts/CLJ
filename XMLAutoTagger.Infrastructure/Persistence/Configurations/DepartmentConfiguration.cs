using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Persistence.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("departments");
            builder.HasKey(d => d.DepartmentId);
            builder.Property(d => d.DepartmentId).HasColumnName("department_id");
            builder.Property(d => d.DepartmentCode).HasColumnName("department_code").HasMaxLength(20).IsRequired();
            builder.Property(d => d.DepartmentName).HasColumnName("department_name").HasMaxLength(120).IsRequired();
            builder.Property(d => d.Description).HasColumnName("description");
            builder.Property(d => d.IconName).HasColumnName("icon_name").HasMaxLength(60);
            builder.Property(d => d.ColourHex).HasColumnName("colour_hex").HasMaxLength(7);
            builder.Property(d => d.IsActive).HasColumnName("is_active");
            builder.Property(d => d.DocTypeSpecialisations).HasColumnName("doc_type_specialisations");
            builder.Property(d => d.HeadUserId).HasColumnName("head_user_id");

            builder.Property(d => d.CreatedAt).HasColumnName("created_at");
            builder.Property(d => d.UpdatedAt).HasColumnName("updated_at");
            builder.Property(d => d.CreatedBy).HasColumnName("created_by");
            builder.Property(d => d.UpdatedBy).HasColumnName("updated_by");

            builder.HasOne(d => d.HeadUser)
                .WithMany()
                .HasForeignKey(d => d.HeadUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(d => d.Members)
                .WithOne(u => u.Department)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
