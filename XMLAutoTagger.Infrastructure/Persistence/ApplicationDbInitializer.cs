using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XMLAutoTagger.Application.Common.Interfaces;

namespace XMLAutoTagger.Infrastructure.Persistence
{
    public class ApplicationDbInitializer : IApplicationDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationDbInitializer> _logger;

        public ApplicationDbInitializer(
            ApplicationDbContext context,
            ILogger<ApplicationDbInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Ensuring database tables and schema are initialized...");
                await _context.Database.EnsureCreatedAsync();

                // Ensure communication_events table exists with 200 char constraints
                await EnsureCommunicationEventsTableAsync();

                _logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database initialization.");
                throw;
            }
        }

        private async Task EnsureCommunicationEventsTableAsync()
        {
            const string sql = @"
                CREATE TABLE IF NOT EXISTS user_mgmt.communication_events (
                    event_id INTEGER PRIMARY KEY,
                    event_code VARCHAR(200) NOT NULL UNIQUE,
                    event_name VARCHAR(200) NOT NULL,
                    description TEXT,
                    channel VARCHAR(50) NOT NULL DEFAULT 'Email',
                    email_subject VARCHAR(500) NOT NULL,
                    email_body TEXT NOT NULL,
                    sms_body TEXT,
                    whatsapp_body TEXT,
                    is_active BOOLEAN NOT NULL DEFAULT TRUE,
                    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                    created_by UUID,
                    updated_by UUID
                );
            ";
            await _context.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
