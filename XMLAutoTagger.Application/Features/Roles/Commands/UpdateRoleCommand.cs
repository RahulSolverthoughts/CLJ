using System;
using System.Collections.Generic;
using MediatR;

namespace XMLAutoTagger.Application.Features.Roles.Commands
{
    public class UpdateRoleCommand : IRequest<bool>
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? BaseRoleId { get; set; }
        public List<ScreenPermissionInput> Permissions { get; set; } = new();
    }
}
