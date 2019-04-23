using System.Globalization;

namespace weixin
{
    public partial class MyCustomMessageHandler
    {

        private string GetWelcomeInfo(CultureInfo c)
        {
            switch (c.Name)
            {
                case "zh-CN":
                    return
     "欢迎关注 guyuming！" + System.Environment.NewLine + System.Environment.NewLine
    //+ " 系统会把您发送到此公众号的文本消息作为私信保存到电脑网站 " + serverUrl + "/sites/public/Lists/Private%20Message/AllItems.aspx" + System.Environment.NewLine + System.Environment.NewLine
    //+ " 您可以直接用电脑登录打开此链接查看回复，或者用电脑浏览器打开 " + serverUrl + " 搜索（比如用自己的用户名作为关键词）" + System.Environment.NewLine
    + "系统根据您的WeiXinOpenId在电脑网站(" + SiteWelcomeUrl + ") 生成用户名:" + SPFBAUserName + System.Environment.NewLine
    + "动态密码:" + DynamicPassword(SPFBAUserName) + System.Environment.NewLine + System.Environment.NewLine
    //+ " 当前整点时间是 " + string.Format("{0:yyyy/MM/dd dddd tt hh}", DateTime.Now) + "点。" + System.Environment.NewLine + System.Environment.NewLine
    + "发送单个字符 G 重新获取网站用户名及实时整点动态密码(假如现在是下午3点多，返回的动态密码当天下午4点失效)。" + System.Environment.NewLine + System.Environment.NewLine
    + "Send message 'en' to switch to English."; 

    //+ " 发送其他任意消息，系统返回本提示消息。" + System.Environment.NewLine
    //+ " 目前系统只处理文本消息。" + System.Environment.NewLine
    //+ " 若关注公众号后始终无法用此网站账户登录网站，有可能是生成账户失败，可以尝试取消关注，再重新关注，让系统重新生成网站账户。也可在网站匿名公开留言，提醒管理员解决。";
                    break;
                case "en-US":

                default:
                    return
    "Thanks for following guyuming！" + System.Environment.NewLine + System.Environment.NewLine
    //+ " System will save the text message you send to this WeChat public account as private discussion into this SharePoint discussion board:" + serverUrl + "/sites/public/Lists/Private%20Message/AllItems.aspx" + System.Environment.NewLine + System.Environment.NewLine
    //+ " You can open the link with PC browser to check reply，or open with PC browser " + serverUrl + " to search (using your username get here as keyword for example)." + System.Environment.NewLine
    + "System created username " + SPFBAUserName + " for you (at SharePoint Site " +SiteWelcomeUrl+"):" + System.Environment.NewLine
    + "And dynamic password:" + DynamicPassword(SPFBAUserName) + System.Environment.NewLine + System.Environment.NewLine
    + "Send letter G to get new dynamic password (if, for example, its 3 o'clock in the afternoon，the dynamic password returned will expire at 4 o'clock)." + System.Environment.NewLine + System.Environment.NewLine
    //+ " System only handles text message now." + System.Environment.NewLine
    //+ " If you can never sign in the SharePoint web site with the username you get here after following, please try unfollow and follow again first, or leave a message on the web site to get help from administrator." + System.Environment.NewLine
    + "如果您想切换回中文，请发送消息 'cn'";


            }
        }

        //private string GetWelcomeInfo()
        //{
        //    return GetWelcomeInfo(CurrentCulture);
        //}

        private string GetSPFBAUserNamePassword()
        {
            switch (CurrentCulture.Name)
            {
                case "zh-CN":
                    return
    "电脑网站("+SiteWelcomeUrl+")用户名:" + SPFBAUserName + System.Environment.NewLine
    + "动态密码:" + DynamicPassword(SPFBAUserName) + System.Environment.NewLine + System.Environment.NewLine
    //            + " 当前整点时间是 " + string.Format("{0:yyyy/MM/dd dddd tt hh}", DateTime.Now) + "点。" + System.Environment.NewLine
    + "若关注公众号后始终无法用此网站账户登录网站，有可能是生成账户失败，可以尝试取消关注，再重新关注，让系统重新生成网站账户。也可在网站匿名公开留言，提醒管理员解决";
                    break;

                case "en-US":
                default:
                    return
       "SharePoint ("+ SiteWelcomeUrl+ ") username:" + SPFBAUserName + System.Environment.NewLine
       + "Dynamic password:" + DynamicPassword(SPFBAUserName) + System.Environment.NewLine + System.Environment.NewLine
       + "If you can never sign in the SharePoint web site with the username you get here after following, please try unfollow and follow again first, or leave a message on the web site to get help from administrator." + System.Environment.NewLine;
            }
        }
        private string GetHelpInfo()
        {
            switch (CurrentCulture.Name)
            {
                case "zh-CN":
                    return

    //   "欢迎关注 guyuming！" + System.Environment.NewLine + System.Environment.NewLine
       "系统会把您发送到此公众号的文本消息作为私信保存到电脑网站 " + serverUrl + "/sites/public/Lists/Private%20Message/AllItems.aspx" + System.Environment.NewLine + System.Environment.NewLine
      + "您可以直接用电脑登录打开此链接查看回复，或者用电脑浏览器打开 " + serverUrl + " 搜索（比如用自己的用户名作为关键词）" + System.Environment.NewLine
      //+ " 系统根据您的WeiXinOpenId为您生成 网站用户名:" + SPFBAUserName + System.Environment.NewLine
      + "发送单个字符 G 重新获取网站用户名及动态密码。" + System.Environment.NewLine + System.Environment.NewLine
                    //  + " 发送其他任意消息，系统返回本提示消息。" + System.Environment.NewLine
                    //  + " 目前系统只处理文本消息。" + System.Environment.NewLine
                    //  + " 若关注公众号后始终无法用此网站账户登录网站，有可能是生成账户失败，可以尝试取消关注，再重新关注，让系统重新生成网站账户。也可在网站匿名公开留言，提醒管理员解决";
        + "Send message 'en' to switch to English." ;
                case "en-US":
                default:
                    return
    //"Thanks for following guyuming！" + System.Environment.NewLine
     "System will save the text message you send to this WeChat public account as private discussion into this SharePoint discussion board:" + serverUrl + "/sites/public/Lists/Private%20Message/AllItems.aspx" + System.Environment.NewLine + System.Environment.NewLine
    + "You can open the link with PC browser to check reply，or open with PC browser " + serverUrl + " to search (using your username get here as keyword for example)." + System.Environment.NewLine
    //+ "System create username automatically for you (base on your WeChatOpenId):" + SPFBAUserName + System.Environment.NewLine
    + "Send letter G to get username and dynamic password." + System.Environment.NewLine + System.Environment.NewLine
    //+ " System only handles text message now." + System.Environment.NewLine
    //+ " If you can never sign in the SharePoint web site with the username you get here after following, please try unfollow and follow again first, or leave a message on the web site to get help from administrator." + System.Environment.NewLine
    + "如果您想切换回中文,发送消息 'cn' ";

            }
        }

    }
}