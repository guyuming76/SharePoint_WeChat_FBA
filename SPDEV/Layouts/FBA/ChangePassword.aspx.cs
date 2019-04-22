using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;

namespace Sharepoint.FormsBasedAuthentication
{
    /// <summary>
    /// Code behind for ChangePassword.aspx
    /// </summary>
    public partial class ChangePassword : LayoutsPageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            if (SPContext.Current.Web.CurrentUser.LoginName.Equals("i:0#.f|fbamember|guest",StringComparison.InvariantCultureIgnoreCase))
            {
                SPUtility.TransferToErrorPage(LocalizedString.GetGlobalString("MyResource", "CannotChangePasswordForBuiltInAccount"));
            }
        }
    }
}