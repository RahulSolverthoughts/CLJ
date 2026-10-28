using System;
using System.Collections.Generic;

namespace XMLAutoTagger.Application.Features.ScreenAccess.DTOs
{
    public class ScreenAccessItemDto
    {
        public Guid ScreenId { get; set; }
        public string ScreenCode { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public string ScreenGroup { get; set; } = string.Empty;
        public string? IconName { get; set; }
        public string? RoutePath { get; set; }
        public string RolePermLevel { get; set; } = "none";
        public string? OverridePermLevel { get; set; }
        public string EffectivePermLevel => !string.IsNullOrEmpty(OverridePermLevel) ? OverridePermLevel : RolePermLevel;
        public bool IsOverridden => !string.IsNullOrEmpty(OverridePermLevel);
    }

    public class UserAccessDetailDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = "Unassigned";
        public string AvatarInitials { get; set; } = string.Empty;
        public List<ScreenAccessItemDto> Screens { get; set; } = new();
    }
}
