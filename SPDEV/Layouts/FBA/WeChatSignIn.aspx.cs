using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace weixin
{
    public partial class WeChatSignIn:WeChatSignInPageBase
    {
        public const string RedirectToQueryStringName = "RedirectToPage";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            //base.OnPreRender(e);
            string r = Request.QueryString[RedirectToQueryStringName];
            if (!string.IsNullOrEmpty(r))
            {
                SPUtility.Redirect(r, SPRedirectFlags.Default | SPRedirectFlags.Trusted, this.Context);
            }
        }

        public static string WeChatSignInAndRedirectToUrl(string redirectTo,string username)
        {
            if (string.IsNullOrEmpty(redirectTo))
            {
                return SPUtility.ConcatUrls(SPContext.Current.Web.Url, string.Concat("_layouts/FBA/WeChatSignIn.aspx?mobile=0&", WeChatSignInPageBase.WeChatTokenQueryStringName, "=", WeChatSignInPageBase.CreateTKForUserName(username)));
            }
            else
            {
                return SPUtility.ConcatUrls(SPContext.Current.Web.Url, string.Concat("_layouts/FBA/WeChatSignIn.aspx?mobile=0&", WeChatSignInPageBase.WeChatTokenQueryStringName, "=", WeChatSignInPageBase.CreateTKForUserName(username), "&", RedirectToQueryStringName, "=", System.Web.HttpUtility.UrlEncode(redirectTo)));
            }
        }
    }
}
