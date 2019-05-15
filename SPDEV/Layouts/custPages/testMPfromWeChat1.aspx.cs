using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace weixin.custPages
{
    public partial class testMPfromWeChat1:WeChatSignInPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (Request.IsAuthenticated)
            {
                Response.Write(SPContext.Current.Web.CurrentUser.LoginName);
            }
            else
            {
                Response.Write("UnAuthenticated");
            }
        }
    }
}
