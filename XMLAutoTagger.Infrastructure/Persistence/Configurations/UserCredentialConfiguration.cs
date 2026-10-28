using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Persistence.Configurations
{
    public class UserCredentialConfiguration : IEntityTypeConfiguration<UserCredential>
    {
        public void Configure(EntityTypeBuilder<UserCredential> builder)
        {
            builder.ToTable("user_credentials");
            builder.HasKey(c => c.UserCredId);
            builder.Property(c => c.UserCredId).HasColumnName("user_cred_id");
            builder.Property(c => c.UserId).HasColumnName("users_id");
            builder.Property(c => c.PasswordHash).HasColumnName("password_hash").IsRequired();
            builder.Property(c => c.HashAlgorithm).HasColumnName("hash_algorithm").HasMaxLength(20);
            builder.Property(c => c.MustChange).HasColumnName("must_change");
            builder.Property(c => c.ChangedAt).HasColumnName("changed_at");
            builder.Property(c => c.PreviousHashes).HasColumnName("previous_hashes");
        }
    }
}
