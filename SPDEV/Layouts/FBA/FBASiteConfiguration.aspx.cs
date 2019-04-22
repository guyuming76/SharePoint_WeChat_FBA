using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;
using System;
using System.Reflection;
using System.Diagnostics;
using weixin;
using Visigo.Sharepoint.FormsBasedAuthentication;

namespace Sharepoint.FormsBasedAuthentication
{
    /// <summary>
    /// Code behind for UsersDisp.aspx
    /// </summary>
    public partial class FBASiteConfiguration : LayoutsPageBase
    {

        protected override bool RequireSiteAdministrator
        {
            get { return true; }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                MembershipSettings settings = new MembershipSettings(SPContext.Current.Web);

                /* Set the options in the web properties */
                chkEnableRoles.Checked = settings.EnableRoles;
                chkReviewMembershipRequests.Checked = settings.ReviewMembershipRequests;

                /* bms Set the URL strings in the web properties */
                txtChangePasswordPage.Text = settings.ChangePasswordPage;
                txtPasswordQuestionPage.Text = settings.PasswordQuestionPage;
                txtThankYouPage.Text = settings.ThankYouPage;

                /* bms Set the XSLT location web properties */
                txtReplyTo.Text = settings.MembershipReplyToEmailAddress;
                txtMembershipApproved.Text = settings.MembershipApprovedEmail;
                txtMembershipPending.Text = settings.MembershipPendingEmail;
                txtMembershipRejected.Text = settings.MembershipRejectedEmail;
                txtPasswordRecovery.Text = settings.PasswordRecoveryEmail;
                txtResetPassword.Text = settings.ResetPasswordEmail;

                /* display the version */
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                lblVersion.Text = fvi.ProductName + " " + fvi.FileVersion;

                //Wechat section
                WeChatServiceUrl.Text = weixin.weixinHttpHandler.WeChatServiceUrl;
                WeChatServiceAppID.Text = weixin.weixinHttpHandler.WeChatServiceAppID;
                WeChatServiceToken.Text = weixin.weixinHttpHandler.WeChatServiceToken;
                WeChatServiceEncodingAESKey.Text = weixin.weixinHttpHandler.WeChatServiceEncodingAESKey;


            }
        }

        protected void BtnUpdateSiteFBAConfig_Click(object sender, EventArgs e)
        {
            MembershipSettings settings = new MembershipSettings(SPContext.Current.Web);

            /* Set the options in the web properties */
            settings.EnableRoles = chkEnableRoles.Checked;
            settings.ReviewMembershipRequests = chkReviewMembershipRequests.Checked;

            /* bms Set the URL strings in the web properties */
            settings.ChangePasswordPage = txtChangePasswordPage.Text;
            settings.PasswordQuestionPage = txtPasswordQuestionPage.Text;
            settings.ThankYouPage = txtThankYouPage.Text;

            /* bms Set the XSLT location web properties */
            settings.MembershipReplyToEmailAddress = txtReplyTo.Text;
            settings.MembershipApprovedEmail = txtMembershipApproved.Text;
            settings.MembershipPendingEmail = txtMembershipPending.Text;
            settings.MembershipRejectedEmail = txtMembershipRejected.Text;
            settings.PasswordRecoveryEmail = txtPasswordRecovery.Text;
            settings.ResetPasswordEmail = txtResetPassword.Text;

            //Wechat section
            //WeChatServiceUrl.Text = weixin.WeChatServiceUrl;
            weixinHttpHandler.WeChatServiceAppID = WeChatServiceAppID.Text;
            weixinHttpHandler.WeChatServiceToken = WeChatServiceToken.Text;
            weixinHttpHandler.WeChatServiceEncodingAESKey = WeChatServiceEncodingAESKey.Text;
            weixinHttpHandler.WeChatPublicAccountNameAndFBARole = WeChatPublicAccountNameAndFBARole.Text;

            if (!Utils.BaseRoleProvider().RoleExists(weixin.weixinHttpHandler.WeChatPublicAccountNameAndFBARole))
            {
                Utils.BaseRoleProvider().CreateRole(weixin.weixinHttpHandler.WeChatPublicAccountNameAndFBARole);
            }
                    
        }
                
    }
}