using System;
using System.Collections.Generic;
using System.Text;


namespace XMLAutoTagger.Application.Common.Interfaces
{
    
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Username { get; }
        string? RoleCode { get; }
        string? RoleName { get; }
        bool IsAuthenticated { get; }
    }
}
