using System;
using System.Collections.Generic;
using XMLAutoTagger.Domain.Common;

namespace XMLAutoTagger.Domain.Entities
{
    public class Role : BaseEntity
    {
        public Guid RoleId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystem { get; set; } = false;
        public Guid? BaseRoleId { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Role? BaseRole { get; set; }
        public ICollection<RoleScreenPermission> Permissions { get; set; } = new List<RoleScreenPermission>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
