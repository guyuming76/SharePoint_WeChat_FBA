using System;
using Microsoft.SharePoint.IdentityModel.Pages;
using System.Web.UI.WebControls;
using System.Text;

namespace Sharepoint.FormsBasedAuthentication
{
    public partial class LoginForm1 : FormsSignInPage
    {
        protected HyperLink registerNewUser;
        protected HyperLink IforgetMyPassword;

        protected void Page_Load(object sender, EventArgs e)
        {

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
