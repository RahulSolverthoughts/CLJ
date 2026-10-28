using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Persistence.Extensions
{
    public static class DbSeedExtensions
    {
        // Deterministic GUIDs for Seed Data
        public static readonly Guid DeptCaseLawId = Guid.Parse("11111111-1111-1111-1111-111111111101");
        public static readonly Guid DeptLegislationId = Guid.Parse("11111111-1111-1111-1111-111111111102");
        public static readonly Guid DeptGazetteId = Guid.Parse("11111111-1111-1111-1111-111111111103");

        public static readonly Guid RoleAdminId = Guid.Parse("22222222-2222-2222-2222-222222222201");
        public static readonly Guid RoleSeniorEditorId = Guid.Parse("22222222-2222-2222-2222-222222222202");
        public static readonly Guid RoleEditorId = Guid.Parse("22222222-2222-2222-2222-222222222203");
        public static readonly Guid RoleQcSpecialistId = Guid.Parse("22222222-2222-2222-2222-222222222204");

        public static readonly Guid ScreenDashboardId = Guid.Parse("33333333-3333-3333-3333-333333333301");
        public static readonly Guid ScreenIntakeQueueId = Guid.Parse("33333333-3333-3333-3333-333333333302");
        public static readonly Guid ScreenTagReviewId = Guid.Parse("33333333-3333-3333-3333-333333333303");
        public static readonly Guid ScreenQcResultsId = Guid.Parse("33333333-3333-3333-3333-333333333304");
        public static readonly Guid ScreenPublicationId = Guid.Parse("33333333-3333-3333-3333-333333333305");
        public static readonly Guid ScreenTaggingRulesId = Guid.Parse("33333333-3333-3333-3333-333333333306");
        public static readonly Guid ScreenAuditLogId = Guid.Parse("33333333-3333-3333-3333-333333333307");
        public static readonly Guid ScreenUserManagementId = Guid.Parse("33333333-3333-3333-3333-333333333308");
        public static readonly Guid ScreenSettingsId = Guid.Parse("33333333-3333-3333-3333-333333333309");
        public static readonly Guid ScreenMlManagementId = Guid.Parse("33333333-3333-3333-3333-333333333310");
        public static readonly Guid ScreenVectorStoreId = Guid.Parse("33333333-3333-3333-3333-333333333311");
        public static readonly Guid ScreenEmailSettingsId = Guid.Parse("33333333-3333-3333-3333-333333333312");
        public static readonly Guid ScreenSmsSettingsId = Guid.Parse("33333333-3333-3333-3333-333333333313");
        public static readonly Guid ScreenWhatsAppSettingsId = Guid.Parse("33333333-3333-3333-3333-333333333314");

        public static readonly Guid AdminUserId = Guid.Parse("44444444-4444-4444-4444-444444444401");
        public static readonly Guid AdminUserCredId = Guid.Parse("55555555-5555-5555-5555-555555555501");

        public static void Seed(this ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // 1. Seed Departments
            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    DepartmentId = DeptCaseLawId,
                    DepartmentCode = "DEPT-CL",
                    DepartmentName = "Case law division",
                    Description = "Judgments from Federal Court, Court of Appeal and High Court",
                    IconName = "ti-gavel",
                    ColourHex = "#185FA5",
                    IsActive = true,
                    DocTypeSpecialisations = new List<string> { "case_law", "practice_dir" },
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Department
                {
                    DepartmentId = DeptLegislationId,
                    DepartmentCode = "DEPT-LEG",
                    DepartmentName = "Legislation division",
                    Description = "Primary and subsidiary legislation, bills and regulations",
                    IconName = "ti-book",
                    ColourHex = "#3C3489",
                    IsActive = true,
                    DocTypeSpecialisations = new List<string> { "legislation", "bill" },
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Department
                {
                    DepartmentId = DeptGazetteId,
                    DepartmentCode = "DEPT-GAZ",
                    DepartmentName = "Gazette and administration",
                    Description = "Government gazettes, ministerial orders and announcements",
                    IconName = "ti-news",
                    ColourHex = "#3B6D11",
                    IsActive = true,
                    DocTypeSpecialisations = new List<string> { "gazette", "order" },
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );

            // 2. Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    RoleId = RoleAdminId,
                    RoleCode = "admin",
                    RoleName = "Administrator",
                    Description = "Full system access including user management, settings, and all pipeline operations",
                    IsSystem = true,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Role
                {
                    RoleId = RoleSeniorEditorId,
                    RoleCode = "senior_editor",
                    RoleName = "Senior editor",
                    Description = "Can tag, review, run QC, approve and publish. Can view user list but not manage.",
                    IsSystem = true,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Role
                {
                    RoleId = RoleEditorId,
                    RoleCode = "editor",
                    RoleName = "Editor",
                    Description = "Can review and correct tags in assigned documents. View-only on all other screens.",
                    IsSystem = true,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Role
                {
                    RoleId = RoleQcSpecialistId,
                    RoleCode = "qc_specialist",
                    RoleName = "QC Specialist",
                    Description = "Quality control reviewer for tagged documents",
                    IsSystem = true,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );

            // 3. Seed Screens
            modelBuilder.Entity<Screen>().HasData(
                new Screen
                {
                    ScreenId = ScreenDashboardId,
                    ScreenCode = "dashboard",
                    ScreenName = "Dashboard",
                    ScreenGroup = "Pipeline",
                    IconName = "ti-layout-dashboard",
                    RoutePath = "/dashboard",
                    SortOrder = 10,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenIntakeQueueId,
                    ScreenCode = "intake_queue",
                    ScreenName = "Intake queue",
                    ScreenGroup = "Pipeline",
                    IconName = "ti-upload",
                    RoutePath = "/intake",
                    SortOrder = 20,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenTagReviewId,
                    ScreenCode = "tag_review",
                    ScreenName = "Tag review",
                    ScreenGroup = "Pipeline",
                    IconName = "ti-tags",
                    RoutePath = "/tag-review",
                    SortOrder = 30,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenQcResultsId,
                    ScreenCode = "qc_results",
                    ScreenName = "QC results",
                    ScreenGroup = "Pipeline",
                    IconName = "ti-shield-check",
                    RoutePath = "/qc",
                    SortOrder = 40,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenPublicationId,
                    ScreenCode = "publication",
                    ScreenName = "Publication",
                    ScreenGroup = "Pipeline",
                    IconName = "ti-world-upload",
                    RoutePath = "/publish",
                    SortOrder = 50,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenTaggingRulesId,
                    ScreenCode = "tagging_rules",
                    ScreenName = "Tagging rules",
                    ScreenGroup = "Administration",
                    IconName = "ti-adjustments",
                    RoutePath = "/rules",
                    SortOrder = 60,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenAuditLogId,
                    ScreenCode = "audit_log",
                    ScreenName = "Audit log",
                    ScreenGroup = "Administration",
                    IconName = "ti-history",
                    RoutePath = "/audit",
                    SortOrder = 70,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenUserManagementId,
                    ScreenCode = "user_management",
                    ScreenName = "User management",
                    ScreenGroup = "Administration",
                    IconName = "ti-users",
                    RoutePath = "/users",
                    SortOrder = 80,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenSettingsId,
                    ScreenCode = "settings",
                    ScreenName = "Settings",
                    ScreenGroup = "Administration",
                    IconName = "ti-settings",
                    RoutePath = "/settings",
                    SortOrder = 90,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenMlManagementId,
                    ScreenCode = "ml_management",
                    ScreenName = "ML model management",
                    ScreenGroup = "AI and ML",
                    IconName = "ti-brain",
                    RoutePath = "/ml",
                    SortOrder = 100,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenVectorStoreId,
                    ScreenCode = "vector_store",
                    ScreenName = "Vector store",
                    ScreenGroup = "AI and ML",
                    IconName = "ti-vector",
                    RoutePath = "/vector",
                    SortOrder = 110,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenEmailSettingsId,
                    ScreenCode = "email_settings",
                    ScreenName = "Email settings",
                    ScreenGroup = "Communication",
                    IconName = "ti-mail",
                    RoutePath = "/Settings/Communication?tab=email",
                    SortOrder = 120,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenSmsSettingsId,
                    ScreenCode = "sms_settings",
                    ScreenName = "SMS settings",
                    ScreenGroup = "Communication",
                    IconName = "ti-message-dots",
                    RoutePath = "/Settings/Communication?tab=sms",
                    SortOrder = 130,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Screen
                {
                    ScreenId = ScreenWhatsAppSettingsId,
                    ScreenCode = "whatsapp_settings",
                    ScreenName = "WhatsApp settings",
                    ScreenGroup = "Communication",
                    IconName = "ti-brand-whatsapp",
                    RoutePath = "/Settings/Communication?tab=whatsapp",
                    SortOrder = 140,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );

            // 4. Seed RoleScreenPermissions
            var screenIds = new[]
            {
                ScreenDashboardId, ScreenIntakeQueueId, ScreenTagReviewId, ScreenQcResultsId, ScreenPublicationId,
                ScreenTaggingRulesId, ScreenAuditLogId, ScreenUserManagementId, ScreenSettingsId, ScreenMlManagementId,
                ScreenVectorStoreId, ScreenEmailSettingsId, ScreenSmsSettingsId, ScreenWhatsAppSettingsId
            };

            var permissions = new List<RoleScreenPermission>();
            var index = 1;

            // Admin: 'admin' permission on all screens
            foreach (var screenId in screenIds)
            {
                permissions.Add(new RoleScreenPermission
                {
                    RspId = Guid.Parse($"a1000000-0000-0000-0001-{index:D12}"),
                    RoleId = RoleAdminId,
                    ScreenId = screenId,
                    PermLevel = "admin",
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                });
                index++;
            }

            // Senior Editor
            var seniorEditorPerms = new Dictionary<Guid, string>
            {
                { ScreenDashboardId, "view" },
                { ScreenIntakeQueueId, "edit" },
                { ScreenTagReviewId, "approve" },
                { ScreenQcResultsId, "approve" },
                { ScreenPublicationId, "approve" },
                { ScreenTaggingRulesId, "edit" },
                { ScreenAuditLogId, "view" },
                { ScreenUserManagementId, "view" },
                { ScreenSettingsId, "view" },
                { ScreenMlManagementId, "none" },
                { ScreenVectorStoreId, "none" },
                { ScreenEmailSettingsId, "view" },
                { ScreenSmsSettingsId, "view" },
                { ScreenWhatsAppSettingsId, "view" }
            };

            foreach (var kvp in seniorEditorPerms)
            {
                permissions.Add(new RoleScreenPermission
                {
                    RspId = Guid.Parse($"a1000000-0000-0000-0002-{index:D12}"),
                    RoleId = RoleSeniorEditorId,
                    ScreenId = kvp.Key,
                    PermLevel = kvp.Value,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                });
                index++;
            }

            // Editor
            var editorPerms = new Dictionary<Guid, string>
            {
                { ScreenDashboardId, "view" },
                { ScreenIntakeQueueId, "view" },
                { ScreenTagReviewId, "edit" },
                { ScreenQcResultsId, "view" },
                { ScreenPublicationId, "none" },
                { ScreenTaggingRulesId, "view" },
                { ScreenAuditLogId, "none" },
                { ScreenUserManagementId, "none" },
                { ScreenSettingsId, "none" },
                { ScreenMlManagementId, "none" },
                { ScreenVectorStoreId, "none" },
                { ScreenEmailSettingsId, "none" },
                { ScreenSmsSettingsId, "none" },
                { ScreenWhatsAppSettingsId, "none" }
            };

            foreach (var kvp in editorPerms)
            {
                permissions.Add(new RoleScreenPermission
                {
                    RspId = Guid.Parse($"a1000000-0000-0000-0003-{index:D12}"),
                    RoleId = RoleEditorId,
                    ScreenId = kvp.Key,
                    PermLevel = kvp.Value,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                });
                index++;
            }

            // QC Specialist
            var qcPerms = new Dictionary<Guid, string>
            {
                { ScreenDashboardId, "view" },
                { ScreenIntakeQueueId, "view" },
                { ScreenTagReviewId, "view" },
                { ScreenQcResultsId, "approve" },
                { ScreenPublicationId, "none" },
                { ScreenTaggingRulesId, "view" },
                { ScreenAuditLogId, "none" },
                { ScreenUserManagementId, "none" },
                { ScreenSettingsId, "none" },
                { ScreenMlManagementId, "none" },
                { ScreenVectorStoreId, "none" },
                { ScreenEmailSettingsId, "none" },
                { ScreenSmsSettingsId, "none" },
                { ScreenWhatsAppSettingsId, "none" }
            };

            foreach (var kvp in qcPerms)
            {
                permissions.Add(new RoleScreenPermission
                {
                    RspId = Guid.Parse($"a1000000-0000-0000-0004-{index:D12}"),
                    RoleId = RoleQcSpecialistId,
                    ScreenId = kvp.Key,
                    PermLevel = kvp.Value,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                });
                index++;
            }

            modelBuilder.Entity<RoleScreenPermission>().HasData(permissions);

            // 5. Seed Initial Admin User (Using anonymous object so store-generated columns are omitted)
            modelBuilder.Entity<User>().HasData(new
            {
                UserId = AdminUserId,
                Email = "admin@legalxml.my",
                FirstName = "System",
                LastName = "Administrator",
                StaffId = "STAFF-001",
                Phone = "+60 3-8888 0001",
                PreferredLanguage = "en",
                AvatarColourHex = "#185FA5",
                RoleId = RoleAdminId,
                DepartmentId = (Guid?)DeptCaseLawId,
                DocTypeSpecialisations = new List<string> { "case_law" },
                AuthProvider = "local",
                AccountStatus = "active",
                FailedLoginCount = (short)0,
                MfaEnabled = false,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            });

            // 6. Seed Admin Credential (BCrypt hash for "Admin@123")
            modelBuilder.Entity<UserCredential>().HasData(new UserCredential
            {
                UserCredId = AdminUserCredId,
                UserId = AdminUserId,
                PasswordHash = "$2a$12$6sVwGfZ1y0p5.2P3qf4BFeW8oZ0Qk9iYyY0mK1m9T4eE1qG.Vq8OW",
                HashAlgorithm = "bcrypt",
                MustChange = false,
                ChangedAt = seedDate,
                PreviousHashes = new List<string>()
            });
        }
    }
}
