using System;
using System.Collections.Generic;

namespace XMLAutoTagger.Application.Features.Roles.DTOs
{
    public class RoleScreenPermissionDto
    {
        public Guid ScreenId { get; set; }
        public string ScreenCode { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public string ScreenGroup { get; set; } = string.Empty;
        public string PermLevel { get; set; } = "none";
    }

    public class RoleDto
    {
        public Guid RoleId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystem { get; set; }
        public Guid? BaseRoleId { get; set; }
        public string? BaseRoleName { get; set; }
        public int UserCount { get; set; }
        public List<RoleScreenPermissionDto> Permissions { get; set; } = new();
    }
}
