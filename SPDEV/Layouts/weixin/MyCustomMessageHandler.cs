using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler;
using System;
using System.IO;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.Context;
using System.Globalization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Web;
using System.Web.Security;
using SharePoint.Helpers;
using System.Collections.Generic;
using Sharepoint.FormsBasedAuthentication;
using System.Collections;

namespace weixin
{
    public partial class MyCustomMessageHandler:CustomMessageHandler,IDisposable
    {
        //protected bool NeedToWriteMessageToSP;
        //protected CultureInfo NeedToSetCultureInfo;

        //protected CultureInfo _currentCulture;
        public CultureInfo CurrentCulture
        {
            get
            {
                return SPFBAUser.Culture;
            }
            set
            {
                try
                {
                    using (new SPMonitoredScope("Weixin.MyCustomMessageHandler.CurrentCulture.set", 5000))
                    {
                        WeChatUser u = SPFBAUser;
                        u.Culture = value;
                        u.Save<WeChatUser>();
                    }
                }
                catch(Exception ex)
                {
                    MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.Message);
                    MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.StackTrace);
                    throw new ExceptionSetCultureForUser(SPFBAUserName);
                }
            }
        }

        public string SPFBAUserName
        {
            get { return Math.Abs(WeixinOpenId.GetHashCode()).ToString(); }
        }

        protected WeChatUser _weChatUser;
        public WeChatUser SPFBAUser
        {
            get
            {
                if (_weChatUser == null)
                {
                    //try
                    //{
                    //    SPUser ret = SPContext.Current.Web.SiteUsers[string.Concat("i:0#.f|fbamember|", SPFBAUserName)];
                    //    if (ret == null)
                    //    {
                    //        throw new Exception("ToEnsureUser");
                    //    }
                    //    return ret;
                    //}
                    //catch(Exception ex)
                    //{
                    Guid siteid = SPContext.Current.Site.ID;
                    Guid webid = SPContext.Current.Web.ID;
                    //WeChatUser ret = null;

                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteid))
                        {
                            using (SPWeb web = site.OpenWeb(webid))
                            {
                                _weChatUser = SPUserNotesEx.DeserializeFromNotes<WeChatUser>(web.EnsureUser(string.Concat("i:0#.f|fbamember|", SPFBAUserName)));

                            }
                        }
                    });
                }

                //MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.High, string.Concat("SPFBAUser:", SPFBAUserName, ", Ensured."));
                return _weChatUser;
                //}
            }
        }

        public const string SecretGuid= "2c07d9ca-4a77-4ee2-a170-e0049d0a92e3";

        public static string DynamicPassword(string uName)
        {
            return Math.Abs(string.Concat(uName, SecretGuid, string.Format("{0:yyyy/MM/dd HH}", DateTime.UtcNow)).GetHashCode()).ToString();
        }
        
        protected string serverUrl
        {
            get { return System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority); }
        }

        protected string SiteWelcomeUrl
        {
            //TODO:weixin 下面取WelcomeUrl 报ThreadAbort Exception 估计是没提权的原因，暂且Hardcode
            //get { return SPUtility.GetFullUrl(SPContext.Current.Site, SPUtility.ConcatUrls(SPContext.Current.Site.RootWeb.ServerRelativeUrl,SPContext.Current.Site.RootWeb.RootFolder.WelcomePage)); }
            get { return SPUtility.GetFullUrl(SPContext.Current.Site, SPUtility.ConcatUrls(SPContext.Current.Site.RootWeb.ServerRelativeUrl, "Lists/List/AllItems.aspx")); }
        }

        protected HttpContext Ctx;
        public MyCustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount,HttpContext context)
            : base(inputStream, postModel, maxRecordCount)
        {
            Ctx = context;
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            try
            {
                switch (requestMessage.Content.ToLower().Trim())
                {
                    case "g":
                        responseMessage.Content = GetSPFBAUserNamePassword();
                        break;
                    case "debug":
                        SPFBAUser.Debug = !SPFBAUser.Debug;
                        SPFBAUser.Save<WeChatUser>();
                        responseMessage.Content = GetWelcomeInfo(CurrentCulture);
                        break;
                    case "h":
                        responseMessage.Content = GetWelcomeInfo(CurrentCulture);
                        break;

                    case "x":
                        SPFBAUser.SaveMessageToPublic = !SPFBAUser.SaveMessageToPublic;
                        SPFBAUser.Save<WeChatUser>();
                        responseMessage.Content = CurrentCulture.Equals(new CultureInfo("zh-CN"))?
                            string.Concat("后续消息将", SPFBAUser.SaveMessageToPublic ? "公开" : "私有", "保存") :
                            string.Concat("Future message will be saved ", SPFBAUser.SaveMessageToPublic ? "publicly" : "privately");
                        break;
                    //TODO: 这个地方可以是 x 返回当前状态，让后 xx 更改，或者搞一个命令 q, 返回一系列的状态，然后有提示切换
                    case "s":
                        //按s 返回所有AppearInWeChat 的 WeChatResult Managed Property
                        //然后， user property 里面可以保留一个 最后 搜索关键字
                        //可以设置一个WeChat 常用搜索列表
                        //点S进入搜索状态，在UserProperty 里面设置标记，返回常用搜索列表
                        //如果用户没点常用搜索项，而是输入任意非命令？ 字符串，则返回搜索此字符串结果，并把此字符串存入 user property 最后搜索关键字
                    case "cn":
                        CultureInfo c = new CultureInfo("zh-CN");
                        CurrentCulture = c;
                        responseMessage.Content = GetWelcomeInfo(c);
                        //NeedToSetCultureInfo = c;
                        break;
                    case "en":
                        CultureInfo e = new CultureInfo("en-US");
                        CurrentCulture = e;
                        responseMessage.Content = GetWelcomeInfo(e);
                        //NeedToSetCultureInfo = e;
                        break;
                    case "ls": //list priviate message
                    case "lg": //list public message
                        string nextDateTime;

                        responseMessage.Content = GetLatestMessageSubject(DateTime.Now, out nextDateTime, requestMessage.Content.ToLower().Trim().Equals("lg"));
                        if(!string.IsNullOrEmpty(nextDateTime))
                        {
                            responseMessage.Content += (System.Environment.NewLine
                                + MessageLink(requestMessage.Content.ToLower().Trim() + HttpUtility.UrlEncode(nextDateTime), "9", "更早的留言"));
                        }
                        break;

                    //case "ps":
                    //case "pg":
                    //    var responseMessageNews = CreateResponseMessage<ResponseMessageNews>();

                    //    return responseMessageNews;
                    //    break;
                    default:
                        DateTime next;
                        string nextDateTime1;
                        if ((requestMessage.Content.ToLower().StartsWith("lg")|| requestMessage.Content.ToLower().StartsWith("ls")) && DateTime.TryParse(HttpUtility.UrlDecode(requestMessage.Content.Substring(2, requestMessage.Content.Length-2)),out next))
                        {
                            responseMessage.Content = GetLatestMessageSubject(next, out nextDateTime1, requestMessage.Content.ToLower().Trim().StartsWith("lg"));
                            if (!string.IsNullOrEmpty(nextDateTime1))
                            {
                                responseMessage.Content += (System.Environment.NewLine
                                    + MessageLink(requestMessage.Content.ToLower().Substring(0,2) + HttpUtility.UrlEncode(nextDateTime1), "9", "更早的留言"));
                            }
                            break;
                        }

                        responseMessage.Content = WriteUserWeixinMessageToSP(SPFBAUserName, requestMessage.Content);
                        //NeedToWriteMessageToSP = true;
                        break;
                }
            }
            catch(WeChatException ex)
            {
                responseMessage.Content = ex.Message;
            }
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            //return base.OnEvent_SubscribeRequest(requestMessage);
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            //responseMessage.Content = GetWelcomeInfo(new CultureInfo("zh-CN"));
            responseMessage.Content = CreateSPFBAUser(SPFBAUserName);

            return responseMessage;
        }
        public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        {

            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            if (CurrentCulture.Name.Equals("zh-CN"))
            {
                responseMessage.Content = "经度：" + requestMessage.Location_Y.ToString() + System.Environment.NewLine
                                    + "维度：" + requestMessage.Location_X.ToString() + System.Environment.NewLine;
            }
            else
            {
                responseMessage.Content = "Longitude：" + requestMessage.Location_Y.ToString() + System.Environment.NewLine
                                    +"Latitude：" + requestMessage.Location_X.ToString() + System.Environment.NewLine;

            }
            return responseMessage;
        }

        protected static object saveFileLock = new object();
        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            string spfileurl = string.Empty;
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

            using (new SPMonitoredScope("Weixin.OnImageRequest", 5000))
            {
                try
                {
                    using (MemoryStream picStream = new MemoryStream())
                    {
                        using (new SPMonitoredScope("Weixin.OnImageRequest_DownloadImage", 5000))
                        {
                            Senparc.Weixin.HttpUtility.Get.Download(requestMessage.PicUrl, picStream);
                        }


                        Guid siteid = SPContext.Current.Site.ID;
                        Guid webid = SPContext.Current.Web.ID;

                        lock (saveFileLock)
                        {
                            using (SPSite site = new SPSite(siteid, SPFBAUser.usertoken))
                            {
                                using (SPWeb web = site.OpenWeb(webid))
                                {
                                    if (SPFBAUser.SaveMessageToPublic)
                                    {
                                        spfileurl = SPUtility.ConcatUrls(web.Lists["图片库"].RootFolder.Url, string.Concat(SPFBAUserName, "_", string.Format("{0:yyyyMMdd_HHmmss_fff}", DateTime.Now), ".", ImageHelper.DetectImageExtension(picStream)));
                                    }
                                    else
                                    {
                                        spfileurl = SPUtility.ConcatUrls(SPUtility.ConcatUrls(web.Lists["图片库"].RootFolder.Url, SPFBAUserName), string.Concat(SPFBAUserName, "_", string.Format("{0:yyyyMMdd_HHmmss_fff}", DateTime.Now), ".", ImageHelper.DetectImageExtension(picStream)));
                                    }
                                    Hashtable prop = new Hashtable();
                                    prop.Add("WeChatMediaId", requestMessage.MediaId);
                                    prop.Add("WeChatPicUrl", requestMessage.PicUrl);
                                    web.Files.Add(spfileurl, picStream, prop);
                                }
                            }
                        }
                    }
                }
                
                catch (Exception ex)
                {
                    MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.Message);
                    MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.StackTrace);
                    //TODO: 在用户消息里加入CorrelationID
                    responseMessage.Content = CurrentCulture.Name.Equals("zh-CN") ? string.Concat("保存图片到服务器失败！")
                            : string.Concat("Failed to save image file to server!");
                    return responseMessage;
                }
            }

            responseMessage.Content = CurrentCulture.Name.Equals("zh-CN") ? string.Concat((SPFBAUser.SaveMessageToPublic?"公开":"私有"),"保存图片到服务器：",SPUtility.ConcatUrls(serverUrl,spfileurl))
                : string.Concat("Image file saved to server ", (SPFBAUser.SaveMessageToPublic ? "publicly" : "privately"), ": ", SPUtility.ConcatUrls(serverUrl, spfileurl));
            return responseMessage;
        }

        public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage)
        {
            return base.OnLinkRequest(requestMessage);

        }


        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            //所有没有被处理的消息会默认返回这里的结果
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            string msg = CurrentCulture.Name.Equals("zh-CN") ? "系统暂不支持的,或未知的MsgType请求类型" : "Unsupported or Unknown MsgType.";
            responseMessage.Content = msg;
            return responseMessage;
        }

        public override void Execute()
        {
            if (CancelExcute)
            {
                return;
            }

            OnExecuting();

            if (CancelExcute)
            {
                return;
            }

            try
            {
                if (RequestMessage == null)
                {
                    return;
                }

                switch (RequestMessage.MsgType)
                {
                    case RequestMsgType.Text:
                        {
                            var requestMessage = RequestMessage as RequestMessageText;
                            ResponseMessage = OnTextOrEventRequest(requestMessage) ?? OnTextRequest(requestMessage);
                            break;
                        }
                    case RequestMsgType.Event:
                        {
                            var requestMessageText = (RequestMessage as IRequestMessageEventBase).ConvertToRequestMessageText();
                            ResponseMessage = OnTextOrEventRequest(requestMessageText) ?? OnEventRequest(RequestMessage as IRequestMessageEventBase);
                            break;
                        }
                    case RequestMsgType.Location:
                        ResponseMessage = OnLocationRequest(RequestMessage as RequestMessageLocation);
                        break;
                    case RequestMsgType.Image:
                        ResponseMessage = OnImageRequest(RequestMessage as RequestMessageImage);
                        break;
                    //case RequestMsgType.Link:
                    //    ResponseMessage = OnLinkRequest(RequestMessage as RequestMessageLink);
                    //    break;
                    default:
                        {
                            //var responseMessage = CreateResponseMessage<ResponseMessageText>();
                            //var requestMessage = RequestMessage as RequestMessageText;
                            ResponseMessage = DefaultResponseMessage(RequestMessage);
                            break;
                        }

                }

                //记录上下文
                if (WeixinContextGlobal.UseWeixinContext && ResponseMessage != null)
                {
                    WeixinContext.InsertMessage(ResponseMessage);
                }
            }
            //catch(WeChatException ex)
            //{
            //    (ResponseMessage as ResponseMessageText).Content = ex.Message;
            //    Ctx.Response.Output.Write(ResponseDocument.ToString());
            //    //throw;
            //}
            //catch (Exception ex)
            //{
            //    throw new MessageHandlerException("MessageHandler中Execute()过程发生错误：" + ex.Message, ex);
            //}
            finally
            {
                OnExecuted();
                //if (ResponseMessage != null)
                //{
                //    string respmsg = string.Concat(ResponseMessage.ToUserName, "@@@@", ResponseDocument.ToString());
                //    MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Medium, string.Concat("RespondDoc:", respmsg));
                //}

            }
        }

        private string CreateSPFBAUser(string username)
        {
            using (new SPMonitoredScope(string.Concat("Weixin.CreateSPFBAUser(", username, ")"), 5000))
            {
                MembershipUser user = Utils.BaseMembershipProvider().GetUser(username, false);

                try
                {
                    if (user == null)
                    {

                        // get site reference             
                        string provider = Utils.GetMembershipProvider(SPContext.Current.Site);

                        // create FBA database user
                        MembershipCreateStatus createStatus;

                        user = Utils.BaseMembershipProvider().CreateUser(username, MyCustomMessageHandler.SecretGuid, null, null, null, true, null, out createStatus);

                        if (createStatus != MembershipCreateStatus.Success)
                        {
                            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, string.Concat("Failed To create SPFBAUser:", username, ", CreateStatus:", createStatus.ToString()));
                            return (new ExceptionCreateSPFBAUser(username)).Message;
                        }

                        if (user == null)
                        {
                            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, string.Concat("Failed To create SPFBAUser:", username, ", Error Unknown."));
                            return (new ExceptionCreateSPFBAUser(username)).Message;
                        }

                        Utils.BaseRoleProvider().AddUsersToRoles(new string[] { user.UserName }, new string[] { "WeChat", weixinHttpHandler.WeChatPublicAccountNameAndFBARole });
                        //测试结果重复加会报错
                    }
                    else
                    {

                        Utils.BaseRoleProvider().AddUserToRolesIfNotInYet(user.UserName, (new List<string>() { "WeChat", weixinHttpHandler.WeChatPublicAccountNameAndFBARole }));
                        MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.High, string.Concat("SPFBAUser:", username, ", alreay exists in membership database"));
                    }

                    Guid siteid = SPContext.Current.Site.ID;
                    Guid webid = SPContext.Current.Web.ID;
                    SPUser currentWeixinFBA = null;

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite site = new SPSite(siteid))
                        {
                            using (SPWeb web = site.OpenWeb(webid))
                            {
                                currentWeixinFBA = web.EnsureUser(string.Concat("i:0#.f|fbamember|", username));

                                //WeChatUser u = new WeChatUser(currentWeixinFBA);

                                //TODO: 把库Title的获取变成SiteProperty，可以为不同的公众号Override，另外加上中英文Title测试，貌似中文状态用英文获取不了。
                                SPHelper.EnsurePersonalFolder(web, web.Lists["图片库"].DefaultViewUrl, SPFBAUserName);
                                SPHelper.EnsurePersonalFolder(web, web.Lists["文档库"].DefaultViewUrl, SPFBAUserName);
                            }
                        }
                    });

                    
                    MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.High, string.Concat("SPFBAUser:", username, ", created successfully."));
                    return GetWelcomeInfo(new CultureInfo("zh-CN"));
                }
                catch (Exception ex)
                {
                    MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.Message);
                    MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.StackTrace);
                    return (new ExceptionCreateSPFBAUser(username)).Message;
                }
            }
        }

        private string WriteUserWeixinMessageToSP(string username, string content)
        {
            using (new SPMonitoredScope("Weixin.WriteUserWeixinMessageToSP", 5000))
            {
                try
                {
                    Guid siteid = SPContext.Current.Site.ID;
                    Guid webid = SPContext.Current.Web.ID;

                    using (SPSite site = new SPSite(siteid, SPFBAUser.usertoken))
                    {
                        using (SPWeb web = site.OpenWeb(webid))
                        {
                            //http://blogs.microsoft.co.il/itaysk/2010/05/05/working-with-sharepoints-discussion-lists-programmatically-part-2/
                            //SPList currentMessageList = web.GetList("/sites/public/lists/Private%20Message");
                            SPList currentMessageList = web.GetList(CurrentMessageListUrl);
                            string subject = content.Length > 255 ? content.Substring(0, 255) : content;
                            SPListItem t = Microsoft.SharePoint.Utilities.SPUtility.CreateNewDiscussion(currentMessageList, subject);
                            t[SPBuiltInFieldId.Body] = content;
                            t.Update();

                            
                            



                            switch (CurrentCulture.Name)
                            {
                                case "zh-CN":
                                    string PrivateOrPublicMessage = SPFBAUser.SaveMessageToPublic ? "公开" : "作为私信";
                                    return
                        //"系统已把您发送的文本消息作为私信保存到电脑网站 " + serverUrl + "/sites/public/Lists/Private%20Message/AllItems.aspx" + System.Environment.NewLine
                        "系统已把您发送的文本消息 "+ PrivateOrPublicMessage + " 保存到电脑网站 " + SPUtility.ConcatUrls(serverUrl, currentMessageList.DefaultViewUrl) + System.Environment.NewLine
                      //+ "发送字符 X 切换您发送的后续消息保存公开状态." + System.Environment.NewLine
                      + MessageLink("x", "7", "发送命令x，切换您发送的后续消息保存公开状态") + System.Environment.NewLine
                      + "您可以直接用电脑登录打开此链接查看回复，或者用电脑浏览器打开 " + serverUrl + " 搜索（比如用自己的用户名作为关键词）" + System.Environment.NewLine
                      //+ "发送单个字符 G 重新获取网站用户名及动态密码。" + System.Environment.NewLine
                      + MessageLink("h", "8", "发送命令h，获取命令列表") + System.Environment.NewLine + System.Environment.NewLine
                      //+ "Please send message 'en' to switch to English." + System.Environment.NewLine;
                      + MessageLink("en", "2", "Send message 'en' to switch to English.");

                                case "en-US":
                                default:
                                    string PrivateOrPublicMessageEn = SPFBAUser.SaveMessageToPublic ? "publicly" : "privately";
                                    return
                     //"System saved the text message you sent to as private discussion into this SharePoint discussion board:" + serverUrl + "/sites/public/Lists/Private%20Message/AllItems.aspx" + System.Environment.NewLine
                     "System saved the text message you sent "+ PrivateOrPublicMessageEn +" into this SharePoint discussion board:" + SPUtility.ConcatUrls(serverUrl, currentMessageList.DefaultViewUrl) + System.Environment.NewLine
                    //+ "Send letter X to toggle your future message privacy." + System.Environment.NewLine
                    + MessageLink("x", "7", "send x to toggle your future message privacy") + System.Environment.NewLine
                    + "You can open the link with PC browser to check reply，or open with PC browser " + serverUrl + " to search (using your username get here as keyword for example)." + System.Environment.NewLine
                    + MessageLink("h", "8", "send h to view this command list") + System.Environment.NewLine + System.Environment.NewLine
                    + MessageLink("cn", "3", "如果您想切换回中文，请发送消息 'cn'");

                                    //+ "Send letter G to get username and dynamic password (if, for example, its 3 o'clock in the afternoon，the dynamic password returned will expire at 4 o'clock)." + System.Environment.NewLine
                                    //+ "如果您想切换回中文,发送消息 'cn' ";

                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.Message);
                    MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.StackTrace);
                    throw new ExceptionWriteMessageIntoSharePoint(SPFBAUserName);

            //        switch (CurrentCulture.Name)
            //        {
            //            case "zh-CN":
            //                return
            //    "保存消息失败，请重试。"+ System.Environment.NewLine
            //  + "Please send message 'en' to switch to English." + System.Environment.NewLine;
            //            case "en-US":
            //            default:
            //                return
            // "Failed to save message, please retry." + System.Environment.NewLine
            //+ "如果您想切换回中文,发送消息 'cn' ";

            //        }

                }
            }
        }


    }
}
