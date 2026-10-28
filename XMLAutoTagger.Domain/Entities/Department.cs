using System;
using System.Collections.Generic;
using XMLAutoTagger.Domain.Common;

namespace XMLAutoTagger.Domain.Entities
{
    public class Department : BaseEntity
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string IconName { get; set; } = "ti-building";
        public string ColourHex { get; set; } = "#185FA5";
        public bool IsActive { get; set; } = true;
        public List<string> DocTypeSpecialisations { get; set; } = new();
        public Guid? HeadUserId { get; set; }

        // Navigation Properties
        public User? HeadUser { get; set; }
        public ICollection<User> Members { get; set; } = new List<User>();
    }
}
