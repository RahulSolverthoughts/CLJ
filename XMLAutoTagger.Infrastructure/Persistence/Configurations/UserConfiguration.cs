using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.UserId).HasColumnName("users_id");
            builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            builder.Property(u => u.FirstName).HasColumnName("first_name").HasMaxLength(80).IsRequired();
            builder.Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(80).IsRequired();
            builder.Property(u => u.FullName)
                .HasColumnName("full_name")
                .HasComputedColumnSql("first_name || ' ' || last_name", stored: true);
            builder.Property(u => u.StaffId).HasColumnName("staff_id").HasMaxLength(40);
            builder.Property(u => u.Phone).HasColumnName("phone").HasMaxLength(30);
            builder.Property(u => u.PreferredLanguage).HasColumnName("preferred_language").HasMaxLength(10);
            builder.Property(u => u.AvatarInitials)
                .HasColumnName("avatar_initials")
                .HasComputedColumnSql("UPPER(SUBSTRING(first_name FROM 1 FOR 1) || SUBSTRING(last_name FROM 1 FOR 1))", stored: true);
            builder.Property(u => u.AvatarColourHex).HasColumnName("avatar_colour_hex").HasMaxLength(7);

            builder.Property(u => u.RoleId).HasColumnName("role_id");
            builder.Property(u => u.DepartmentId).HasColumnName("department_id");
            builder.Property(u => u.DocTypeSpecialisations).HasColumnName("doc_type_specialisations");

            builder.Property(u => u.AuthProvider).HasColumnName("auth_provider").HasMaxLength(50);
            builder.Property(u => u.SsoExternalId).HasColumnName("sso_external_id").HasMaxLength(255);
            builder.Property(u => u.AccountStatus).HasColumnName("account_status").HasMaxLength(50);
            builder.Property(u => u.FailedLoginCount).HasColumnName("failed_login_count");
            builder.Property(u => u.LockedAt).HasColumnName("locked_at");
            builder.Property(u => u.LockedReason).HasColumnName("locked_reason");
            builder.Property(u => u.LastLoginAt).HasColumnName("last_login_at");
            builder.Property(u => u.AccessExpiresAt).HasColumnName("access_expires_at");
            builder.Property(u => u.MfaEnabled).HasColumnName("mfa_enabled");

            builder.Property(u => u.CreatedAt).HasColumnName("created_at");
            builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");
            builder.Property(u => u.CreatedBy).HasColumnName("created_by");
            builder.Property(u => u.UpdatedBy).HasColumnName("updated_by");

            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            builder.HasOne(u => u.Department)
                .WithMany(d => d.Members)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(u => u.Credential)
                .WithOne(c => c.User)
                .HasForeignKey<UserCredential>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.ScreenOverrides)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
