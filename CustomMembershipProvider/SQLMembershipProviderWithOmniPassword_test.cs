using Sharepoint.FormsBasedAuthentication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Security;
using weixin;

namespace CustomMembershipProvider
{
    public partial class SQLMembershipProviderWithOmniPassword
    {
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Test, Microsoft.SharePoint.Administration.TraceSeverity.Verbose, string.Concat(new StackTrace().ToString() ));
            return base.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Test, Microsoft.SharePoint.Administration.TraceSeverity.Verbose, string.Concat(new StackTrace().ToString()));
            return base.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Test, Microsoft.SharePoint.Administration.TraceSeverity.Verbose, string.Concat(new StackTrace().ToString()));
            return base.GetAllUsers(pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Test, Microsoft.SharePoint.Administration.TraceSeverity.Verbose, string.Concat(new StackTrace().ToString()));
            return base.GetUser(providerUserKey, userIsOnline);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Test, Microsoft.SharePoint.Administration.TraceSeverity.Verbose, string.Concat(new StackTrace().ToString()));
            return base.GetUser(username, userIsOnline);
        }

        public override string GetUserNameByEmail(string email)
        {
            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Test, Microsoft.SharePoint.Administration.TraceSeverity.Verbose, string.Concat(new StackTrace().ToString()));
            return base.GetUserNameByEmail(email);
        }

    }
}
