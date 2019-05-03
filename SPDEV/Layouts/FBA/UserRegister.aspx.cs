using System;
using System.Web.Security;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Web;
using System.Text;
using weixin;
using Visigo.Sharepoint.FormsBasedAuthentication;

namespace Sharepoint.FormsBasedAuthentication
{
    /// <summary>
    /// Code behind for UserNew.aspx
    /// </summary>
    public partial class UserRegister : UnsecuredLayoutsPageBase
    {
        protected override bool AllowAnonymousAccess
        {
            get
            {
                return true;
            }
        }

        //protected override bool RequireSiteAdministrator
        //{
        //    get { return true; }
        //}

        protected void OnSubmit(object sender, EventArgs e)
        {
            // check to see if username already in use
            MembershipUser user = Utils.BaseMembershipProvider().GetUser(txtUsername.Text,false);
            
            if (user == null)
            {
                try
                {
                    // get site reference             
                    string provider = Utils.GetMembershipProvider(this.Site);

                    // create FBA database user
                    MembershipCreateStatus createStatus;

                    //user = Utils.BaseMembershipProvider().CreateUser(txtUsername.Text, txtPassword.Text, txtEmail.Text, null, null, false, null, out createStatus);
                    //这里不应该写入邮箱，邮箱应该在激活成功后更新进去
                    //如果激活邮件用户没收到，之前的设计是希望用户使用通过邮件找回账号的方式重置密码，同时激活账号
                    //但没考到到如果之前输入了错误的邮箱号，或者这个邮箱本身打不开了怎么办，这样这个用户名就“死”了
                    //现在改成在激活后写入邮箱属性，就没有这个问题

                    user = Utils.BaseMembershipProvider().CreateUser(txtUsername.Text, txtPassword.Text, null, null, null, false, null, out createStatus);

                    if (createStatus != MembershipCreateStatus.Success)
                    {
                        SetErrorMessage(createStatus);
                        return;
                    }

                    if (user == null)
                    {
                        lblMessage.Text = LocalizedString.GetGlobalString("FBAPackWebPages", "UnknownError");
                        return;
                    }


                    //Utils.BaseRoleProvider().AddUsersToRoles(new string[] { user.UserName }, new string[] { "Registered" });
                    //不能这个时候加Role，应该是在邮件激活后加入一个EmailValidated Role
                    SendActivationEmailAndRedirect(user, txtEmail.Text.Trim().ToLower());
                    

                }
                catch (Exception ex)
                {
                    Utils.LogError(ex, true);
                }
            }
            //else if (!string.IsNullOrEmpty(user.Email) && user.IsApproved)
            //{
            //    lblMessage.Text = LocalizedString.GetGlobalString("MyResource", "DuplicateUserNameWithEmail"); ;
            //}
            else
            {
                //这里 还得先验证下用户名密码
                if (Utils.BaseMembershipProvider().ValidateUser(txtUsername.Text, txtPassword.Text))
                {
                    //绑定邮箱到Weixin自动生成的账户
                    SendActivationEmailAndRedirect(user, txtEmail.Text.Trim().ToLower());
                }
                else
                {
                    lblMessage.Text = LocalizedString.GetGlobalString("MyResource", "PasswordIncorrect");
                    
                }

            }
        }

        private void SendActivationEmailAndRedirect(MembershipUser user, string emailInLower)
        {
            string linkExpireTime = DateTime.UtcNow.AddMinutes(30).Ticks.ToString();
            //string token = string.Concat(user.UserName.ToLower(), user.Email.ToLower(), MyCustomMessageHandler.SecretGuid, linkExpireTime).GetHashCode().ToString();
            string token = string.Concat(user.UserName.ToLower(), emailInLower, MyCustomMessageHandler.SecretGuid, linkExpireTime).GetHashCode().ToString();

            string SignInUrl = Encoding.Default.GetString(Convert.FromBase64String(Request.QueryString["SignInUrl"]));

            string activationLink = SPUtility.ConcatUrls(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority), string.Concat("/_layouts/FBA/UserActivate.aspx?USERNAME=", user.UserName, "&email=", emailInLower, "&token=", token, "&linkExpireTime=", linkExpireTime, "&Source=", System.Uri.EscapeDataString(SignInUrl)));
            Email.SendEmail(this.Web, emailInLower, LocalizedString.GetGlobalString("MyResource", "activateYourAccount"), activationLink);

            string source = Request.QueryString["source"];
            if (string.IsNullOrEmpty(source))
            {
                //FBADiagnosticsService.Local.WriteTrace(0, FBADiagnosticsService.FBADiagnosticsCategory.General, Microsoft.SharePoint.Administration.TraceSeverity.High, string.Concat("Activate Email sent to ", user.Email, ". Url:", activationLink));

                //SPUtility.Redirect("FBA/Management/UsersDisp.aspx", SPRedirectFlags.RelativeToLayoutsPage | SPRedirectFlags.UseSource | SPRedirectFlags.DoNotEndResponse, this.Context);
                MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.General, Microsoft.SharePoint.Administration.TraceSeverity.Verbose, string.Concat("RedirectTo:", SignInUrl));
                SPUtility.Redirect(SignInUrl, SPRedirectFlags.DoNotEndResponse, this.Context);
            }
            else
            {
                MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.General, Microsoft.SharePoint.Administration.TraceSeverity.Verbose, string.Concat("RedirectTo:", source));
                SPUtility.Redirect(Request.RawUrl, SPRedirectFlags.UseSource | SPRedirectFlags.DoNotEndResponse, this.Context);
            }
        }


        protected void SetErrorMessage(MembershipCreateStatus status)
        {
             switch (status)
             {
                 case MembershipCreateStatus.DuplicateUserName:
                    lblMessage.Text = LocalizedString.GetGlobalString("FBAPackWebPages", "DuplicateUserName");
                    break;

                case MembershipCreateStatus.DuplicateEmail:
                    lblEmailMessage.Text = LocalizedString.GetGlobalString("FBAPackWebPages", "DuplicateEmail");
                    break;

                case MembershipCreateStatus.InvalidPassword:
                    string message = "";
                    if (string.IsNullOrEmpty(Utils.BaseMembershipProvider().PasswordStrengthRegularExpression))
                    {
                        message = string.Format(LocalizedString.GetGlobalString("FBAPackWebPages", "InvalidPasswordChars"), Utils.BaseMembershipProvider().MinRequiredPasswordLength,  Utils.BaseMembershipProvider().MinRequiredNonAlphanumericCharacters);
                    }
                    else
                    {
                        message = string.Format(LocalizedString.GetGlobalString("FBAPackWebPages", "InvalidPasswordCharsRegex"), Utils.BaseMembershipProvider().MinRequiredPasswordLength,  Utils.BaseMembershipProvider().MinRequiredNonAlphanumericCharacters, Utils.BaseMembershipProvider().PasswordStrengthRegularExpression);
                    }
                    //LocalizedString.GetGlobalString("FBAPackWebPages", "InvalidPassword")
                    // TODO: use resource files
                    lblPasswordMessage.Text = message;
                    break;

                case MembershipCreateStatus.InvalidEmail:
                    lblEmailMessage.Text = LocalizedString.GetGlobalString("FBAPackWebPages", "InvalidEmail");
                    break;

                //case MembershipCreateStatus.InvalidAnswer:
                //    lblAnswerMessage.Text = LocalizedString.GetGlobalString("FBAPackWebPages", "InvalidAnswer");
                //    break;

                //case MembershipCreateStatus.InvalidQuestion:
                //    lblQuestionMessage.Text = LocalizedString.GetGlobalString("FBAPackWebPages", "InvalidQuestion");
                //    break;

                case MembershipCreateStatus.InvalidUserName:
                    lblMessage.Text = LocalizedString.GetGlobalString("FBAPackWebPages", "InvalidUserName");
                    break;

                case MembershipCreateStatus.ProviderError:
                    lblMessage.Text = LocalizedString.GetGlobalString("FBAPackWebPages", "ProviderError");
                    break;

                case MembershipCreateStatus.UserRejected:
                    lblMessage.Text = LocalizedString.GetGlobalString("FBAPackWebPages", "UserRejected");
                    break;

                default:
                    lblMessage.Text = LocalizedString.GetGlobalString("FBAPackWebPages", "UnknownError");
                    break;
            }
        }

        /// <summary>
        /// Adds a user to the SharePoint (in no particular group)
        /// </summary>
        /// <param name="login"></param>
        /// <param name="email"></param>
        /// <param name="fullname"></param>
        private void AddUserToSite(string login, string email, string fullname)
        {
            this.Web.AllUsers.Add(
                login,
                email,
                fullname,
                "");
        }
    }
}
