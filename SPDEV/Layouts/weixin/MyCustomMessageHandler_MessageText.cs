using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Sharepoint.FormsBasedAuthentication;
using System;
using System.Globalization;

namespace weixin
{
    public partial class MyCustomMessageHandler
    {
        public static string MessageLink(string messaage,string id,string text)
        {
            return string.Format("<a href='weixin://bizmsgmenu?msgmenucontent={0}&msgmenuid={1}'>{2}</a>", messaage, id, text);
        }

        public static string WebLink(string url, string text)
        {
            return string.Format("<a href='{0}'>{1}</a>", url, text);
        }

        private string GetWelcomeInfo(CultureInfo c)
        {
            //string oneTimePassword = MyCustomMessageHandler.OneTimeDynamicPassword(SPFBAUserName);
            //SPFBAUser.OneTimeDynamicPassword = oneTimePassword;
            //SPFBAUser.Save<WeChatUser>();
            string oneTimePassword = Utils.BaseMembershipProvider().GetUser(SPFBAUserName, false).Comment;

            //string redirectUrl = string.Concat(MyCustomMessageHandler.SiteWelcomeUrl, "?mobile=0");
            string redirectUrl = SPUtility.ConcatUrls(SPContext.Current.Web.Url, string.Concat("_layouts/osssearchresults.aspx?k=http&mobile=0&u=", System.Web.HttpUtility.UrlEncode(SPContext.Current.Site.Url)));
            //string searchresultpage = SPUtility.ConcatUrls(serverUrl, string.Concat("results.aspx?mobile=0&k=http&u=", System.Web.HttpUtility.UrlEncode(SPContext.Current.Web.Url)));
            string imageView = SPUtility.ConcatUrls(serverUrl, SPUtility.ConcatUrls(ImageLibUrl, "/Forms/AllPictureNoFolder.aspx?mobile=1&viewmode=thumbnail"));

            switch (c.Name)
            {
                case "zh-CN":
                    return
     // "欢迎关注 guyuming！" + System.Environment.NewLine + System.Environment.NewLine
     //+ " 系统会把您发送到此公众号的文本消息作为私信保存到电脑网站 " + serverUrl + "/sites/public/Lists/Private%20Message/AllItems.aspx" + System.Environment.NewLine + System.Environment.NewLine
     //+ " 您可以直接用电脑登录打开此链接查看回复，或者用电脑浏览器打开 " + serverUrl + " 搜索（比如用自己的用户名作为关键词）" + System.Environment.NewLine
     // "系统根据您的WeiXinOpenId在电脑网站(" + siteWelcomeUrl + ") 生成用户名:" + SPFBAUserName + System.Environment.NewLine
     //"系统根据您的WeiXinOpenId在电脑网站(" + WebLink(string.Concat(SPUtility.ConcatUrls(SPContext.Current.Web.Url,"_layouts/FBA/WeChatSignIn.aspx?mobile=0&WeChatSignInTK="),WeChatSignInPageBase.CreateTKForUserName(SPFBAUserName)),siteWelcomeUrl) + ") 生成用户名:" + SPFBAUserName + System.Environment.NewLine
     //"系统根据您的WeiXinOpenId在网站(" + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(redirectUrl, SPFBAUserName), siteWelcomeUrl) + ") 生成用户名:" + SPFBAUserName + System.Environment.NewLine
     //"系统根据您的WeiXinOpenId在网站(" + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(redirectUrl, SPFBAUserName), SPContext.Current.Web.Url) + ") 生成用户名:" + SPFBAUserName + System.Environment.NewLine
     "网站:" + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(redirectUrl, SPFBAUserName, oneTimePassword), SPContext.Current.Web.Url) + System.Environment.NewLine
    + "用户名:" + SPFBAUserName + System.Environment.NewLine
    //+ "动态密码:" + DynamicPassword(SPFBAUserName) + System.Environment.NewLine + System.Environment.NewLine
    + "动态密码:" + oneTimePassword + System.Environment.NewLine + System.Environment.NewLine
    //+ " 当前整点时间是 " + string.Format("{0:yyyy/MM/dd dddd tt hh}", DateTime.Now) + "点。" + System.Environment.NewLine + System.Environment.NewLine
    //+ MessageLink("G", "1", "发送单个字符 G 重新获取网站用户名及动态密码。") + "(假如现在是下午3点多，返回的动态密码当天下午4点失效)。" + System.Environment.NewLine
    //+ "搜索中心:" + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(string.Concat(serverUrl, "?mobile=0"), SPFBAUserName), serverUrl) + System.Environment.NewLine
    //+ "搜索中心:" + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(searchresultpage, SPFBAUserName), serverUrl) + System.Environment.NewLine
    + MessageLink("lg", "5", "查看最新公开留言") + System.Environment.NewLine
    + MessageLink("ls", "6", "查看最新私信") + System.Environment.NewLine
    + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(imageView, SPFBAUserName, oneTimePassword), "查看最新图片") + System.Environment.NewLine + System.Environment.NewLine

    + string.Concat("当前消息保存状态:", SPFBAUser.SaveMessageToPublic ? "公开" : "私有") + System.Environment.NewLine
    + MessageLink("x", "7", "切换后续消息保存状态") + System.Environment.NewLine + System.Environment.NewLine

    + MessageLink("h", "8", "刷新此命令列表") + System.Environment.NewLine + System.Environment.NewLine

    + MessageLink("en", "2", "Switch to English.");
    



    //+ " 发送其他任意消息，系统返回本提示消息。" + System.Environment.NewLine
    //+ " 目前系统只处理文本消息。" + System.Environment.NewLine
    //+ " 若关注公众号后始终无法用此网站账户登录网站，有可能是生成账户失败，可以尝试取消关注，再重新关注，让系统重新生成网站账户。也可在网站匿名公开留言，提醒管理员解决。";
                    break;
                case "en-US":

                default:
                    return
     //"Thanks for following guyuming！" + System.Environment.NewLine + System.Environment.NewLine
     //+ " System will save the text message you send to this WeChat public account as private discussion into this SharePoint discussion board:" + serverUrl + "/sites/public/Lists/Private%20Message/AllItems.aspx" + System.Environment.NewLine + System.Environment.NewLine
     //+ " You can open the link with PC browser to check reply，or open with PC browser " + serverUrl + " to search (using your username get here as keyword for example)." + System.Environment.NewLine
     //"System created username " + SPFBAUserName + " for you (at SharePoint Site " + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(redirectUrl, SPFBAUserName), siteWelcomeUrl) + "):" + System.Environment.NewLine
     //"System created username " + SPFBAUserName + " for you (at SharePoint Site " + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(redirectUrl, SPFBAUserName), SPContext.Current.Web.Url) + "):" + System.Environment.NewLine
    "SharePoint site:" + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(redirectUrl, SPFBAUserName,oneTimePassword), SPContext.Current.Web.Url) + System.Environment.NewLine
    + "Username:" + SPFBAUserName + System.Environment.NewLine
    //+ "Dynamic password:" + DynamicPassword(SPFBAUserName) + System.Environment.NewLine + System.Environment.NewLine
    + "Dynamic password:" + oneTimePassword + System.Environment.NewLine + System.Environment.NewLine
    //+ MessageLink("G","1","Send letter G to get new dynamic password")+" (if, for example, its 3 o'clock in the afternoon，the dynamic password returned will expire at 4 o'clock)." + System.Environment.NewLine + System.Environment.NewLine
    //+ " System only handles text message now." + System.Environment.NewLine
    //+ " If you can never sign in the SharePoint web site with the username you get here after following, please try unfollow and follow again first, or leave a message on the web site to get help from administrator." + System.Environment.NewLine
    //+ "Search Center:" + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(string.Concat(serverUrl, "?mobile=0"), SPFBAUserName), serverUrl) + System.Environment.NewLine
    //+ "Search Center:" + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(searchresultpage, SPFBAUserName), serverUrl) + System.Environment.NewLine
    + MessageLink("lg", "5", "view latest public message") + System.Environment.NewLine
    + MessageLink("ls", "6", "view latest private message") + System.Environment.NewLine
    + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(imageView, SPFBAUserName,oneTimePassword), "view latest picture") + System.Environment.NewLine + System.Environment.NewLine

    + string.Concat("current message saving privacy: ", SPFBAUser.SaveMessageToPublic ? "public" : "private") + System.Environment.NewLine
    + MessageLink("x", "7", "toggle future message privacy") + System.Environment.NewLine + System.Environment.NewLine
    
    + MessageLink("h", "8", "refresh this command list") + System.Environment.NewLine + System.Environment.NewLine

    + MessageLink("cn","3","切换回中文");


            }
        }

        //private string GetWelcomeInfo()
        //{
        //    return GetWelcomeInfo(CurrentCulture);
        //}

        [Obsolete]
        private string GetSPFBAUserNamePassword()
            //不再被调用了
        {
            //string redirectUrl = string.Concat(MyCustomMessageHandler.SiteWelcomeUrl, "?mobile=0");
            string redirectUrl = SPUtility.ConcatUrls(SPContext.Current.Web.Url, string.Concat("_layouts/osssearchresults.aspx?k=http&mobile=0"));
            switch (CurrentCulture.Name)
            {
                case "zh-CN":
                    return
    //"网站("+ WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(redirectUrl, SPFBAUserName), siteWelcomeUrl) + ")用户名:" + SPFBAUserName + System.Environment.NewLine
    "网站(" + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(redirectUrl, SPFBAUserName), SPContext.Current.Web.Url) + ")用户名:" + SPFBAUserName + System.Environment.NewLine
    + "动态密码:" + DynamicPassword(SPFBAUserName) + System.Environment.NewLine + System.Environment.NewLine
    //            + " 当前整点时间是 " + string.Format("{0:yyyy/MM/dd dddd tt hh}", DateTime.Now) + "点。" + System.Environment.NewLine
    + "若关注公众号后始终无法用此网站账户登录网站，有可能是生成账户失败，可以尝试取消关注，再重新关注，让系统重新生成网站账户。也可在网站匿名公开留言，提醒管理员解决";
                    break;

                case "en-US":
                default:
                    return
       //"SharePoint ("+ WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(redirectUrl, SPFBAUserName), siteWelcomeUrl) + ") username:" + SPFBAUserName + System.Environment.NewLine
       "SharePoint (" + WebLink(WeChatSignIn.WeChatSignInAndRedirectToUrl(redirectUrl, SPFBAUserName), SPContext.Current.Web.Url) + ") username:" + SPFBAUserName + System.Environment.NewLine
       + "Dynamic password:" + DynamicPassword(SPFBAUserName) + System.Environment.NewLine + System.Environment.NewLine
       + "If you can never sign in the SharePoint web site with the username you get here after following, please try unfollow and follow again first, or leave a message on the web site to get help from administrator." + System.Environment.NewLine;
            }
        }

    }
}