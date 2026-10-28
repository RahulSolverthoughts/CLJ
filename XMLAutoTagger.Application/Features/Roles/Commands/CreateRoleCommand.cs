using System;
using System.Collections.Generic;
using MediatR;

namespace XMLAutoTagger.Application.Features.Roles.Commands
{
    public class ScreenPermissionInput
    {
        public Guid ScreenId { get; set; }
        public string PermLevel { get; set; } = "none";
    }

    public class CreateRoleCommand : IRequest<Guid>
    {
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? BaseRoleId { get; set; }
        public List<ScreenPermissionInput> Permissions { get; set; } = new();
    }
}
