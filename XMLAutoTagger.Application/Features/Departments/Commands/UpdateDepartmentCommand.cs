using System;
using System.Collections.Generic;
using MediatR;

namespace XMLAutoTagger.Application.Features.Departments.Commands
{
    public class UpdateDepartmentCommand : IRequest<bool>
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string IconName { get; set; } = "ti-building";
        public string ColourHex { get; set; } = "#185FA5";
        public bool IsActive { get; set; } = true;
        public List<string> DocTypes { get; set; } = new();
        public Guid? HeadUserId { get; set; }
    }
}
