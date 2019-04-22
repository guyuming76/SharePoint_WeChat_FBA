using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using Sharepoint.FormsBasedAuthentication;

namespace SharePoint.Helpers
{
    public static class MemebershipRoleProviderHelper
    {
        public static void AddUserToRolesIfNotInYet(this RoleProvider p,string username, List<string> roles)
        {
            string[] rsUserIn = p.GetRolesForUser(username);
            List<string> rsToAdd = new List<string>();

            if (rsUserIn != null)
            {
                foreach (string role in roles)
                {
                    if (!rsUserIn.Contains(role))
                    {
                        rsToAdd.Add(role);
                    }
                }
            }
            else
            {
                rsToAdd = roles;
            }

            if (rsToAdd.Count > 0)
            {
                Utils.BaseRoleProvider().AddUsersToRoles(new string[] { username }, rsToAdd.ToArray());
            }
        }

    }
}
