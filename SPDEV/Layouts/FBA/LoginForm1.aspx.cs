using System;
using Microsoft.SharePoint.IdentityModel.Pages;
using System.Web.UI.WebControls;
using System.Text;
using System.Web;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;
using System.Linq;

namespace Sharepoint.FormsBasedAuthentication
{
    public partial class LoginForm1 : FormsSignInPage
    {
        protected HyperLink registerNewUser;
        protected HyperLink IforgetMyPassword;
        protected Label QR;
        protected string qrUrl;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (SPMonitoredScope m = new SPMonitoredScope("LoginForm1.Page_Load", 5000))
            {
                string returnurl = System.Web.HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString["ReturnUrl"]);
                string absoluteReturnurl = SPUtility.ConcatUrls(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority).ToString(), returnurl);
                using (SPSite sitecollection = new SPSite(absoluteReturnurl))
                {
                    if (sitecollection.Features.Cast<SPFeature>().FirstOrDefault(_ => _.DefinitionId.Equals(new Guid("a72382ee-7a69-4a59-aa8d-86d47ebc5fd0"))) == null)
                    {
                        //no wechat public account
                        qrUrl = SPUtility.ConcatUrls(sitecollection.ServerRelativeUrl, "/_layouts/FBA/gg100.png");

                        //qrUrl = returnurl.Replace("/_layouts/authenticate.aspx", "/_layouts/FBA/gg100.png");

                        QR.Visible = false;
                    }
                    else
                    {
                        qrUrl = SPUtility.ConcatUrls(sitecollection.ServerRelativeUrl, "/Style%20Library/wechat100.png");
                        //qrUrl = returnurl.Replace("/_layouts/authenticate.aspx", "/Style%20Library/wechat100.png");
                        QR.Visible = true;
                    }
                }

               //QR.Text = string.Concat("start-end(mm:ss:ff): ",string.Format("{0:mm:ss:ff}", m.GetMonitor<SPExecutionTimeCounter>().StartTime), "-", string.Format("{0:mm:ss:ff}", m.GetMonitor<SPExecutionTimeCounter>().EndTime),", duration(ms):", m.GetMonitor<SPExecutionTimeCounter>().Value);
               // QR.Visible = true;
            }  

        //string atk = AccessTokenContainer.TryGetToken("wxee56a98aeb2690f4", "7f9dd19106d03102a57a4e05ede14f4d");
        //if (string.IsNullOrEmpty(atk))
        //{
        //    throw new Exception("Empty weixin AccessToken");
        //}
        //int scenId = (new Guid()).GetHashCode();
        //CreateQrCodeResult qr = QrCodeApi.Create(atk, 600, scenId);
        //WeixinQr.ImageUrl = QrCodeApi.GetShowQrCodeUrl(qr.ticket);
        

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            string source = Request.QueryString["source"];
            if (string.IsNullOrEmpty(source))
            {
                registerNewUser.NavigateUrl = "/_layouts/FBA/UserRegister.aspx?SignInUrl=" + Convert.ToBase64String(Encoding.Default.GetBytes(Request.RawUrl));
                IforgetMyPassword.NavigateUrl = "/_layouts/FBA/IForgetMyPassword.aspx?SignInUrl=" + Convert.ToBase64String(Encoding.Default.GetBytes(Request.RawUrl));
            }
            else
            {
                registerNewUser.NavigateUrl = "/_layouts/FBA/UserRegister.aspx?source=" + source + "&SignInUrl=" + Convert.ToBase64String(Encoding.Default.GetBytes(Request.RawUrl));
                IforgetMyPassword.NavigateUrl = "/_layouts/FBA/IForgetMyPassword.aspx?source=" + source + "&SignInUrl=" + Convert.ToBase64String(Encoding.Default.GetBytes(Request.RawUrl));
            }
        }
    }
}
