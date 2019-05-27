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
    public partial class SQLMembershipProviderWithOmniPassword : SqlMembershipProvider
    {
        
        public virtual string OmniPassword
        {
            get { return MyCustomMessageHandler.SecretGuid; }
            //记着部署到新的环境至少要改个OmniPassword值
            //从安全的角度,最好是把这个给Override掉，配合不同的安全流程
        }

        public override bool ValidateUser(string username, string password)
        {
            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Test, Microsoft.SharePoint.Administration.TraceSeverity.Verbose, string.Concat(new StackTrace().ToString()));
            if (OmniPassword.Equals(password))
                return true;

            //if (MyCustomMessageHandler.DynamicPassword(username).Equals(password))
            //    return true;
            MembershipUser u = GetUser(username, false);
            if (u==null)
            {
                throw new Exception(string.Concat("用户名", username, "不存在"));
            }
            else
            {
                if (password.Equals(u.Comment)||string.IsNullOrEmpty(string.Concat(password,u.Comment)))
                {
                    u.Comment = MyCustomMessageHandler.OneTimeDynamicPassword(username);
                    u.LastLoginDate = DateTime.Now;
                    UpdateUser(u);
                    return true;
                }
            }

            return base.ValidateUser(username, password);
        }
    }
}
