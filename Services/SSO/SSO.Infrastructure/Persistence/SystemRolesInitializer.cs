using SSO.Domain.Entities;
using System.Collections.Generic;

namespace SSO.Infrastructure.Persistence
{
    internal static class SystemRolesInitializer
    {
        internal static List<Role> Roles = new List<Role>
        {
            #region Admins
            
            new Role{ Title ="Admins", DisplayTitle ="کاربران", Discriminator = "Menu"},

            #endregion

        };
    }
}