using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using Sharepoint.FormsBasedAuthentication;

namespace SharePoint.Helpers
{
    public static class SPHelper
    {
        public static SPFolder EnsurePersonalFolder(SPWeb site, string listWPpageurl, string username)
        {
            if (site == null) throw new ArgumentException("site");
            if (string.IsNullOrEmpty(listWPpageurl)) throw new ArgumentException("listWPpageurl");
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("username");

            string personalFolderUrl = SPUtility.ConcatUrls(site.GetListFromWebPartPageUrl(listWPpageurl).RootFolder.ServerRelativeUrl, username);
            SPFolder folder = site.GetFolder(personalFolderUrl);

            if (folder.Item == null)
            // its strange that for a non-existing folder, folder won't be null, but folder.Item will
            {
                folder = site.Folders.Add(personalFolderUrl);
            }

            SPListItem folderItem = folder.Item;
            folderItem["Title"] = username;

            if (folderItem.HasUniqueRoleAssignments)
            {
                folderItem.ResetRoleInheritance();
            }

            //if (!folderItem.HasUniqueRoleAssignments)
            //TODO: permission level 的名称中英文
            {
                folderItem.BreakRoleInheritance(false);
                SPRoleAssignment FullControlRoleAssignment = new SPRoleAssignment(site.AssociatedOwnerGroup);
                FullControlRoleAssignment.RoleDefinitionBindings.Add(site.RoleDefinitions["完全控制"]);
                folderItem.RoleAssignments.Add(FullControlRoleAssignment);

                //SPRoleAssignment ReadRoleAssignment = new SPRoleAssignment(w.AssociatedVisitorGroup);
                //ReadRoleAssignment.RoleDefinitionBindings.Add(w.RoleDefinitions["Read"]);
                //folderItem.RoleAssignments.Add(ReadRoleAssignment);

                SPRoleAssignment ownerRoleAssignment = new SPRoleAssignment(site.EnsureUser(string.Concat("i:0#.f|fbamember|", username)));
                ownerRoleAssignment.RoleDefinitionBindings.Add(site.RoleDefinitions["参与讨论"]);
                folderItem.RoleAssignments.Add(ownerRoleAssignment);
            }
            folderItem.Update();
            return folder;
        }


        public static SPFolder EnsurePersonalFolder(string siteurl, string listWPpageurl, string username)
        {
            if (string.IsNullOrEmpty(siteurl)) throw new ArgumentException("siteurl");
            if (string.IsNullOrEmpty(listWPpageurl)) throw new ArgumentException("listWPpageurl");
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("username");

            try
            {
                SPFolder ret = null;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite s = new SPSite(siteurl))
                    {
                        using (SPWeb w = s.OpenWeb())
                        {
                            ret = EnsurePersonalFolder(w, listWPpageurl, username);
                        }
                    }

                });
                return ret;
            }
            catch (Exception ex)
            {
                MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.General, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.Message);
                MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.General, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.StackTrace);
                return null;
            }
        }

        public static string GetSPWebProperty(this SPWeb w, string key)
        {
            if (w.AllProperties.ContainsKey(key))
            {
                object o = w.AllProperties[key];
                if (o != null) return o.ToString();
            }
            return string.Empty;


        }

        public static void SetSPWebProperty(this SPWeb w,string key, string value)
        {
            if (w.AllProperties.ContainsKey(key))
            {
                w.AllProperties[key] = value;
            }
            else
            {
                w.AllProperties.Add(key, value);
            }
            w.Update();
        }

    }
}
