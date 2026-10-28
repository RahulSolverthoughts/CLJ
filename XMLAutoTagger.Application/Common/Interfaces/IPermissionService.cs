using System;
using System.Threading.Tasks;

namespace XMLAutoTagger.Application.Common.Interfaces
{
    public interface IPermissionService
    {
        /// <summary>
        /// Retrieves the effective permission level for a given user on a screen ("none", "view", "edit", "approve", "admin").
        /// </summary>
        Task<string> GetEffectivePermissionAsync(Guid userId, string screenCode);

        /// <summary>
        /// Checks whether the currently logged-in user has permission to make changes (edit, approve, or admin) on a screen.
        /// Returns false if the user only has "view" or "none" permission.
        /// </summary>
        Task<bool> CanCurrentUserEditScreenAsync(string screenCode);
    }
}
