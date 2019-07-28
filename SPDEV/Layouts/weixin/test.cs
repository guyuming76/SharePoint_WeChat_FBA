using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace weixin
{
    public class test : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (SPContext.Current.Site.SystemAccount.ID.Equals(SPContext.Current.Web.CurrentUser.ID))
            {
                string method = context.Request["method"];
                switch (method)
                {
                    case "WeChatSignInAndRedirectToUrl":
                        string redirectTo = context.Request["redirectTo"];
                        string username = context.Request["username"];
                        context.Response.Write(WeChatSignIn.WeChatSignInAndRedirectToUrl(redirectTo, username));
                        break;
                    case "wol": new WakeOnLine().WOLMyDEV();
                        break;
                    default: break;
                }
            }
            else
            {
                context.Response.Write("Only SystemAccount can call this");
            }
        }
    }
}
