using Microsoft.SharePoint;
using Microsoft.SharePoint.IdentityModel;
using Microsoft.SharePoint.IdentityModel.Pages;
using Microsoft.SharePoint.Utilities;
using Sharepoint.FormsBasedAuthentication;
using SharePoint.Helpers;
using System;
using System.IdentityModel.Tokens;

namespace weixin
{
    public class WeChatSignInPageBase:IdentityModelSignInPageBase
    {
        public const string WeChatTokenQueryStringName = "WeChatSignInTK";

        protected string GetUserNamePasswordFromTK(string tk)
        {
            string username1 = EncryptTool.Decrypt(tk, MyCustomMessageHandler.SecretGuid, false);
            if (username1.StartsWith("WeChatUserName:"))
            {
                return username1.Substring(15, username1.Length - 15);
            }
            throw new Exception(string.Concat("无效的", WeChatTokenQueryStringName));
        }

        public static string CreateTKForUserName(string username)
        {
            return System.Web.HttpUtility.UrlEncode(EncryptTool.Encrypt(string.Concat("WeChatUserName:", username, ":", MyCustomMessageHandler.DynamicPassword(username)), MyCustomMessageHandler.SecretGuid, false));
        }

        protected override void OnLoad(EventArgs e)
        {
            //base.OnLoad(e);

            try
            {
                string tk = Request.QueryString[WeChatTokenQueryStringName];

                if (Request.IsAuthenticated)
                {
                    if (!string.IsNullOrEmpty(tk))
                    {
                        string[] usernamePassword = GetUserNamePasswordFromTK(tk).Split(':');
                        //if (!SPContext.Current.Web.CurrentUser.LoginName.EndsWith(string.Concat("|", usernamePassword[0])))
                        {
                            SecurityToken stk = SPSecurityContext.SecurityTokenForFormsAuthentication(AppliesTo, Utils.BaseMembershipProvider().Name, Utils.BaseRoleProvider().Name, usernamePassword[0], usernamePassword[1], false);
                            if (stk == null) throw new Exception("生成的SecurityToken为null,可能是动态密码过期,请尝试刷新微信公众号命令，获取新的网站链接");
                            SPFederationAuthenticationModule spFedAuthModule = this.Context.ApplicationInstance.Modules["FederatedAuthentication"] as SPFederationAuthenticationModule;
                            SPSecurity.RunWithElevatedPrivileges(() => spFedAuthModule.SetPrincipalAndWriteSessionToken(stk, SPSessionTokenWriteType.WriteSessionCookie));
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(tk))
                    {
                        throw new Exception("WeChatTokenQueryStringName 参数为空");
                    }
                    else
                    {
                        string[] usernamePassword = GetUserNamePasswordFromTK(tk).Split(':');
                        SecurityToken stk = SPSecurityContext.SecurityTokenForFormsAuthentication(AppliesTo, Utils.BaseMembershipProvider().Name, Utils.BaseRoleProvider().Name, usernamePassword[0], usernamePassword[1], false);
                        if (stk == null) throw new Exception("生成的SecurityToken为null,可能是动态密码过期,请尝试刷新微信公众号命令，获取新的网站链接");
                        SPFederationAuthenticationModule spFedAuthModule = this.Context.ApplicationInstance.Modules["FederatedAuthentication"] as SPFederationAuthenticationModule;
                        SPSecurity.RunWithElevatedPrivileges(() => spFedAuthModule.SetPrincipalAndWriteSessionToken(stk, SPSessionTokenWriteType.WriteSessionCookie));
                    }
                }
            }
            catch(Exception ex)
            {
                MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.Message);
                MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.StackTrace);
                SPUtility.TransferToErrorPage(ex.Message);
            }
        }
    }
}
