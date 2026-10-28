using System;
using System.Collections.Generic;
using System.Text;
using XMLAutoTagger.Domain.Entities;
using XMLAutoTagger.Domain.Common;

namespace XMLAutoTagger.Domain.Entities
{

    public class DepartmentMember : BaseEntity
    {
        public Guid Member_Id { get; set; }
        public Guid User_Id { get; set; }
        public string RoleIn_Department { get; set; } = "Member"; // e.g., "Senior Editor", "Member"

        // Navigation Properties
        public Department Department { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
