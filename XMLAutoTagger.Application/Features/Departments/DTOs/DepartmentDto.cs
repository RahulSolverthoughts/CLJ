using System;
using System.Collections.Generic;

namespace XMLAutoTagger.Application.Features.Departments.DTOs
{
    public class DepartmentMemberDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string AvatarInitials { get; set; } = string.Empty;
    }

    public class DepartmentDto
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string Icon { get; set; } = "ti-building";
        public string ColorHex { get; set; } = "#185FA5";
        public List<string> DocTypes { get; set; } = new();
        public Guid? HeadUserId { get; set; }
        public string HeadUserName { get; set; } = "Unassigned";
        public int MemberCount { get; set; }
        public string DocumentType => DocTypes.Count > 0 ? string.Join(", ", DocTypes) : "General";
        public List<DepartmentMemberDto> Members { get; set; } = new();

        // Aliases for compatibility
        public Guid Department_Id => DepartmentId;
        public string Department_Name => DepartmentName;
        public string Department_Code => DepartmentCode;
    }
}
