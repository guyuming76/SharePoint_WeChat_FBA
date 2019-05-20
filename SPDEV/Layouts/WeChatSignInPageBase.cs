using Microsoft.SharePoint;
using Microsoft.SharePoint.IdentityModel;
using Microsoft.SharePoint.IdentityModel.Pages;
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

            string tk = Request.QueryString[WeChatTokenQueryStringName];

            if (Request.IsAuthenticated)
            {
                if(!string.IsNullOrEmpty(tk))
                {
                    string[] usernamePassword = GetUserNamePasswordFromTK(tk).Split(':');
                    //if (!SPContext.Current.Web.CurrentUser.LoginName.EndsWith(string.Concat("|", usernamePassword[0])))
                    {
                        SecurityToken stk = SPSecurityContext.SecurityTokenForFormsAuthentication(AppliesTo, Utils.BaseMembershipProvider().Name, Utils.BaseRoleProvider().Name, usernamePassword[0], usernamePassword[1], false);
                        SPFederationAuthenticationModule spFedAuthModule = this.Context.ApplicationInstance.Modules["FederatedAuthentication"] as SPFederationAuthenticationModule;
                        SPSecurity.RunWithElevatedPrivileges(() => spFedAuthModule.SetPrincipalAndWriteSessionToken(stk, SPSessionTokenWriteType.WriteSessionCookie));
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(tk))
                {
                    throw new Exception("未验证用户");
                }
                else
                {
                    string[] usernamePassword = GetUserNamePasswordFromTK(tk).Split(':');
                    SecurityToken stk = SPSecurityContext.SecurityTokenForFormsAuthentication(AppliesTo, Utils.BaseMembershipProvider().Name, Utils.BaseRoleProvider().Name, usernamePassword[0], usernamePassword[1], false);
                    SPFederationAuthenticationModule spFedAuthModule = this.Context.ApplicationInstance.Modules["FederatedAuthentication"] as SPFederationAuthenticationModule;
                    SPSecurity.RunWithElevatedPrivileges(() => spFedAuthModule.SetPrincipalAndWriteSessionToken(stk, SPSessionTokenWriteType.WriteSessionCookie));
                }
            }
        }
    }
}
