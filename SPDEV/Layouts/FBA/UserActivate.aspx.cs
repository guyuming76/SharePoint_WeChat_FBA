using System;
using System.Web.Security;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using weixin;

namespace Sharepoint.FormsBasedAuthentication
{
    /// <summary>
    /// Code behind for UserEdit.aspx
    /// </summary>
    public partial class UserActivate : UnsecuredLayoutsPageBase
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

        protected override void OnLoad(EventArgs e)
        {
            // get user info
            string userName = this.Request.QueryString["USERNAME"];
            string token = this.Request.QueryString["token"];
            string linkExpireTime = Request.QueryString["linkExpireTime"];
            string emailInLower = Request.QueryString["email"].Trim().ToLower();
            //SPUser spuser = null;
            //try
            //{
            //    spuser = this.Web.AllUsers[Utils.EncodeUsername(userName)];
            //}
            //catch
            //{
                
            //}
            MembershipUser user = Utils.BaseMembershipProvider().GetUser(userName,false);

            if (user != null)
            {
                if (!Page.IsPostBack)
                {
                    // load user props
                    //if (spuser != null)
                    //{
                    //    txtEmail.Text = spuser.Email;
                    //    txtFullName.Text = spuser.Name;
                    //}
                    //else
                    //{
                        txtEmail.Text = user.Email;
                        txtFullName.Text = user.UserName;
                    //}
                    txtUsername.Text = user.UserName;
                    isActive.Checked = user.IsApproved;
                    isLocked.Checked = user.IsLockedOut;
                    isLocked.Enabled = user.IsLockedOut;

                    //if (string.Concat(user.UserName.ToLower(), user.Email.ToLower(), MyCustomMessageHandler.SecretGuid,linkExpireTime).GetHashCode().ToString().Equals(token))
                    if (string.Concat(user.UserName.ToLower(), emailInLower, MyCustomMessageHandler.SecretGuid, linkExpireTime).GetHashCode().ToString().Equals(token))
                    {
                        if (DateTime.UtcNow.Ticks>long.Parse(linkExpireTime))
                        {
                            SPUtility.TransferToErrorPage(LocalizedString.GetGlobalString("MyResource", "LinkExpired"));
                        }

                        user.Email = emailInLower;
                        user.Comment = MyCustomMessageHandler.OneTimeDynamicPassword(user.UserName);
                        user.IsApproved = true;
                        Utils.BaseMembershipProvider().UpdateUser(user);
                        //TODO:这里还要加个WorkItemTimerJob, 用来同步membership Email 变化到SharePoint UserInfoList

                        //SPUtility.Redirect("FBA/Management/UsersDisp.aspx", SPRedirectFlags.RelativeToLayoutsPage | SPRedirectFlags.UseSource | SPRedirectFlags.DoNotEndResponse, this.Context);
                        //string SignInUrl = Encoding.Default.GetString(Convert.FromBase64String(Request.QueryString["SignInUrl"]));

                        //SPUtility.Redirect(SignInUrl, SPRedirectFlags.DoNotEndResponse, this.Context); ;
                        //Uri SignIn = new Uri(SignInUrl, UriKind.RelativeOrAbsolute);
                        //string path;
                        //string query;
                        //if (SignIn.IsAbsoluteUri)
                        //{
                        //    path = SignIn.GetLeftPart(UriPartial.Path);
                        //    query = SignIn.Query;
                        //}
                        //else
                        //{
                        //    int num = SignInUrl.IndexOf('?');
                        //    path = num < 0 ? SignInUrl : SignInUrl.Substring(0, num);
                        //    query = num < 0 ? string.Empty : SignInUrl.Substring(num + 1, SignInUrl.Length - num - 1);
                        //}

                        //FBADiagnosticsService.Local.WriteTrace(0, FBADiagnosticsService.FBADiagnosticsCategory.General, Microsoft.SharePoint.Administration.TraceSeverity.Verbose, string.Concat("RedirectTo:", SignInUrl, ";   Path:", path, ";   QueryString:", query));
                        SPUtility.Redirect(this.Context.Request.RawUrl, SPRedirectFlags.UseSource | SPRedirectFlags.DoNotEndResponse, this.Context);

                        //Response.Redirect(SignInUrl);
                    }
                    else
                    {
                        SPUtility.TransferToErrorPage(LocalizedString.GetGlobalString("MyResource", "InvalidToken"));
                    }

                }
            }
            else
            {
                SPUtility.TransferToErrorPage(LocalizedString.GetGlobalString("FBAPackWebPages", "UserNotFound"));
            }
        }

        #region 没用
        //这个没用，可以注释掉，Onload 成功后直接就 Redirect 掉了 ，除非以后成功后还容许在这个界面上设置一些用户属性
        protected void OnSubmit(object sender, EventArgs e)
        {
            // get user info
            string userName = this.Request.QueryString["USERNAME"];
            SPUser spuser = null;
            // This could be done with EnsureUsers, which won't throw an exception if the user hasn't logged on to the site.
            try
            {
                spuser = this.Web.AllUsers[Utils.EncodeUsername(userName)];
            }
            catch
            {

            }
            MembershipUser user = Utils.BaseMembershipProvider().GetUser(userName,false);
            
            // check user exists
            if (user != null)
            {
                try
                {
                    // TODO: If we want the Email to be used for the user account, we need to delete the user and create a new one with the new email address.
                    // This will mean we need to iterate over the groups that the user is a member of, in all site collections in all web apps, and add the new user
                    // to those groups.  In the meantime, we allow the email to be changed, but this won't update the account username.

                    // update membership provider info
                    user.Email = txtEmail.Text;
                    user.IsApproved = isActive.Checked;

                    //Unlock Account
                    if (user.IsLockedOut && !isLocked.Checked)
                    {
                        user.UnlockUser();
                    }
                    try
                    {
                        Utils.BaseMembershipProvider().UpdateUser(user);
                    }
                    catch (System.Configuration.Provider.ProviderException ex)
                    {
                        lblMessage.Text = ex.Message;
                        return;
                    }

     
                    // update sharepoint user info
                    if (spuser != null)
                    {
                        spuser.Email = txtEmail.Text;
                        spuser.Name = txtFullName.Text;
                        spuser.Update();
                    }

                    SPUtility.Redirect("FBA/Management/UsersDisp.aspx", SPRedirectFlags.RelativeToLayoutsPage | SPRedirectFlags.UseSource, this.Context);
                    
                }
                catch (Exception ex)
                {
                    Utils.LogError(ex, true);
                }
            }
            else
            {
                SPUtility.TransferToErrorPage(LocalizedString.GetGlobalString("FBAPackWebPages","UserNotFound"));
            }
        }

        protected void OnResetPassword(object sender, EventArgs e)
        {
            SPUtility.Redirect(string.Format("FBA/Management/UserResetPassword.aspx?UserName={0}&Source={1}", this.Request.QueryString["USERNAME"], SPHttpUtility.UrlKeyValueEncode(SPUtility.OriginalServerRelativeRequestUrl)), SPRedirectFlags.RelativeToLayoutsPage, this.Context);
        }

        protected void OnDeleteUser(object sender, EventArgs e)
        {
            SPUtility.Redirect(string.Format("FBA/Management/UserDelete.aspx?UserName={0}&Source={1}", this.Request.QueryString["USERNAME"], SPHttpUtility.UrlKeyValueEncode(SPUtility.OriginalServerRelativeRequestUrl)), SPRedirectFlags.RelativeToLayoutsPage, this.Context);
        }
        #endregion
    }
}
