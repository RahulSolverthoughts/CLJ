using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Domain.Common;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Interfaces;
using XMLAutoTagger.Infrastructure.Persistence.Extensions;

namespace XMLAutoTagger.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Screen> Screens => Set<Screen>();
        public DbSet<RoleScreenPermission> RoleScreenPermissions => Set<RoleScreenPermission>();
        public DbSet<UserScreenOverride> UserScreenOverrides => Set<UserScreenOverride>();
        public DbSet<UserCredential> UserCredentials => Set<UserCredential>();
        public DbSet<EmailSetting> EmailSettings => Set<EmailSetting>();
        public DbSet<SmsSetting> SmsSettings => Set<SmsSetting>();
        public DbSet<WhatsAppSetting> WhatsAppSettings => Set<WhatsAppSetting>();
        public DbSet<CommunicationEventItem> CommunicationEvents => Set<CommunicationEventItem>();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                var userId = _currentUserService.UserId;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = userId;
                        entry.Entity.UpdatedBy = userId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedBy = userId;
                        entry.Property(e => e.CreatedAt).IsModified = false;
                        entry.Property(e => e.CreatedBy).IsModified = false;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("user_mgmt");

            // Automatically discover and apply all entity configurations from Configurations/
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Seed reference & initial data
            modelBuilder.Seed();
        }
    }
}
