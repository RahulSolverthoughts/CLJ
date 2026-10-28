using System;
using XMLAutoTagger.Domain.Common;

namespace XMLAutoTagger.Domain.Entities
{
    public class RoleScreenPermission : BaseEntity
    {
        public Guid RspId { get; set; }
        public Guid RoleId { get; set; }
        public Guid ScreenId { get; set; }
        public string PermLevel { get; set; } = "none"; // 'none', 'view', 'edit', 'approve', 'admin'

        // Navigation Properties
        public Role Role { get; set; } = null!;
        public Screen Screen { get; set; } = null!;
    }
}
