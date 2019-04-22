using System;
using System.Web.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using weixin;

namespace Sharepoint.FormsBasedAuthentication
{
    /// <summary>
    /// Code behind for UserDelete.aspx
    /// </summary>
    public partial class UserResetPasswordWithToken : UnsecuredLayoutsPageBase    {

        //protected override bool RequireSiteAdministrator
        //{
        //    get { return true; }
        //}

        protected override void OnLoad(EventArgs e)
        {
            //this.CheckRights();
        
            // display error confirmation message
            string userName = Request.QueryString["USERNAME"];
            if (string.IsNullOrEmpty(userName))
            {
                SPUtility.TransferToErrorPage(LocalizedString.GetGlobalString("FBAPackWebPages", "UserNotFound"));
                return;
            }

            
            string linkExpireTime = Request.QueryString["linkExpireTime"];
            string token = string.Concat(userName.ToLower(), MyCustomMessageHandler.SecretGuid, linkExpireTime).GetHashCode().ToString();
            if (token != Request.QueryString["token"])
            {
                //Invalid Token
                SPUtility.TransferToErrorPage(LocalizedString.GetGlobalString("MyResource", "InvalidToken"));
            }
            else if (DateTime.UtcNow.Ticks > long.Parse(linkExpireTime))
            {
                //link expired
                SPUtility.TransferToErrorPage(LocalizedString.GetGlobalString("MyResource", "LinkExpired"));
            }



            //if (Utils.BaseMembershipProvider().RequiresQuestionAndAnswer || !Utils.BaseMembershipProvider().EnablePasswordReset)
            //{
            //    SPUtility.TransferToErrorPage(LocalizedString.GetGlobalString("FBAPackWebPages", "ResetPasswordUnavailable"));
            //    return;
            //}

            resetPasswordMsg.Text = string.Format(LocalizedString.GetGlobalString("FBAPackWebPages", "ResetPasswordMsg"), userName);

            lblNewPasswordError.Text = "";

            if (!this.Page.IsPostBack)
            {
                resetAutoPassword.Checked = true;
                resetSelectPassword.Checked = false;
                chkSendEmail.Checked = true;
                
            }

        }

        protected void OnResetPassword(object sender, EventArgs e)
        {
            string username = Request.QueryString["USERNAME"];

            bool sendEmail = true;

            string newPassword = null;

            if (resetSelectPassword.Checked)
            {
                newPassword = txtNewPassword.Text;
                sendEmail = chkSendEmail.Checked;
            }

            try
            {
                Utils.ResetUserPassword(username, newPassword, sendEmail, Web);
                MembershipUser user = Utils.BaseMembershipProvider().GetUser(username, false);
                if (!user.IsApproved)
                {
                    user.IsApproved = true;
                    Utils.BaseMembershipProvider().UpdateUser(user);
                }
            }
            catch (ArgumentException ex)
            {
                lblNewPasswordError.Text = ex.Message;
                return;
            }
            catch (Exception ex)
            {
                Utils.LogError(ex, true);
                SPUtility.TransferToErrorPage(LocalizedString.GetGlobalString("FBAPackWebPages", "UnexpectedError"));
                return;
            }

            SPUtility.Redirect("FBA/GetBackAccountsByEmail.aspx", SPRedirectFlags.RelativeToLayoutsPage | SPRedirectFlags.UseSource |SPRedirectFlags.DoNotEndResponse, this.Context);
        }

    }
}
