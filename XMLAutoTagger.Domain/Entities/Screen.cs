using System;
using XMLAutoTagger.Domain.Common;

namespace XMLAutoTagger.Domain.Entities
{
    public class Screen : BaseEntity
    {
        public Guid ScreenId { get; set; }
        public string ScreenCode { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public string ScreenGroup { get; set; } = string.Empty; // "Pipeline", "Administration", "AI and ML"
        public string? IconName { get; set; }
        public string? RoutePath { get; set; }
        public string? Description { get; set; }
        public short SortOrder { get; set; } = 100;
        public bool IsActive { get; set; } = true;
    }
}
