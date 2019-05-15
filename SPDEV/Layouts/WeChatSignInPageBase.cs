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

        protected string GetUserNameFromTK(string tk)
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
            return System.Web.HttpUtility.UrlEncode(EncryptTool.Encrypt(string.Concat("WeChatUserName:", username), MyCustomMessageHandler.SecretGuid, false));
        }

        protected override void OnLoad(EventArgs e)
        {
            //base.OnLoad(e);
            if (!Request.IsAuthenticated)
            {
                string tk = Request.QueryString[WeChatTokenQueryStringName];
                if (string.IsNullOrEmpty(tk))
                {
                    throw new Exception("未验证用户");
                }
                else
                {
                    SecurityToken stk = SPSecurityContext.SecurityTokenForFormsAuthentication(AppliesTo, Utils.BaseMembershipProvider().Name, Utils.BaseRoleProvider().Name, GetUserNameFromTK(tk), MyCustomMessageHandler.SecretGuid, false);
                    SPFederationAuthenticationModule spFedAuthModule = this.Context.ApplicationInstance.Modules["FederatedAuthentication"] as SPFederationAuthenticationModule;
                    SPSecurity.RunWithElevatedPrivileges(() => spFedAuthModule.SetPrincipalAndWriteSessionToken(stk, SPSessionTokenWriteType.WriteSessionCookie));
                }
            }
        }
    }
}
