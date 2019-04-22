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

namespace weixin
{
    public partial class MyCustomMessageHandler:CustomMessageHandler
    {
        //protected bool NeedToWriteMessageToSP;
        //protected CultureInfo NeedToSetCultureInfo;

        protected CultureInfo _currentCulture;
        public CultureInfo CurrentCulture
        {
            get
            {
                if (_currentCulture == null)
                {
                    using (new SPMonitoredScope("Weixin.MyCustomMessageHandler.CurrentCulture.set", 5000))
                    {
                        try
                        {
                            _currentCulture = string.IsNullOrEmpty(SPFBAUser.Notes) ? new CultureInfo("zh-CN") : new CultureInfo(SPFBAUser.Notes);
                        }
                        catch (Exception ex)
                        {
                            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.Message);
                            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.StackTrace);
                            throw new ExceptionGetCultureForUser(SPFBAUserName);
                        }
                    }
                }
                return _currentCulture;
            }
            set
            {
                try
                {
                    using (new SPMonitoredScope("Weixin.MyCustomMessageHandler.CurrentCulture.set", 5000))
                    {
                        SPUser u = SPFBAUser;
                        u.Notes = value.Name;
                        u.Update();
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

        public SPUser SPFBAUser
        {
            get
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
                    SPUser ret = null;

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite site = new SPSite(siteid))
                        {
                            using (SPWeb web = site.OpenWeb(webid))
                            {
                                ret = web.EnsureUser(string.Concat("i:0#.f|fbamember|", SPFBAUserName));
                            }
                        }
                    });

                    //MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.High, string.Concat("SPFBAUser:", SPFBAUserName, ", Ensured."));
                    return ret;
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
                    default:
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

                        using (SPSite site = new SPSite(siteid, SPFBAUser.UserToken))
                        {
                            using (SPWeb web = site.OpenWeb(webid))
                            {
                                spfileurl = SPUtility.ConcatUrls(SPUtility.ConcatUrls(web.Lists["图片库"].RootFolder.Url, SPFBAUserName), string.Concat(SPFBAUserName, "_", string.Format("{0:yyyyMMdd_HHmmss}", DateTime.Now), ".", ImageHelper.DetectImageExtension(picStream)));
                                web.Files.Add(spfileurl, picStream);
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

            responseMessage.Content = CurrentCulture.Name.Equals("zh-CN") ? string.Concat("保存图片到服务器：",SPUtility.ConcatUrls(serverUrl,spfileurl))
                : string.Concat("Image file saved to server: ", SPUtility.ConcatUrls(serverUrl, spfileurl));
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

                    using (SPSite site = new SPSite(siteid, SPFBAUser.UserToken))
                    {
                        using (SPWeb web = site.OpenWeb(webid))
                        {
                            //http://blogs.microsoft.co.il/itaysk/2010/05/05/working-with-sharepoints-discussion-lists-programmatically-part-2/
                            SPList privateMessage = web.GetList("/sites/public/lists/Private%20Message");
                            string subject = content.Length > 255 ? content.Substring(0, 255) : content;
                            SPListItem t = Microsoft.SharePoint.Utilities.SPUtility.CreateNewDiscussion(privateMessage, subject);
                            t[SPBuiltInFieldId.Body] = content;
                            t.Update();

                            switch (CurrentCulture.Name)
                            {
                                case "zh-CN":
                                    return
                        "系统已把您发送的文本消息作为私信保存到电脑网站 " + serverUrl + "/sites/public/Lists/Private%20Message/AllItems.aspx" + System.Environment.NewLine
                      + "您可以直接用电脑登录打开此链接查看回复，或者用电脑浏览器打开 " + serverUrl + " 搜索（比如用自己的用户名作为关键词）" + System.Environment.NewLine
                      + "发送单个字符 G 重新获取网站用户名及动态密码。" + System.Environment.NewLine
                      + "Please send message 'en' to switch to English." + System.Environment.NewLine;
                                case "en-US":
                                default:
                                    return
                     "System saved the text message you sent to as private discussion into this SharePoint discussion board:" + serverUrl + "/sites/public/Lists/Private%20Message/AllItems.aspx" + System.Environment.NewLine
                    + "You can open the link with PC browser to check reply，or open with PC browser " + serverUrl + " to search (using your username get here as keyword for example)." + System.Environment.NewLine
                    + "Send letter G to get username and dynamic password (if, for example, its 3 o'clock in the afternoon，the dynamic password returned will expire at 4 o'clock)." + System.Environment.NewLine
                    + "如果您想切换回中文,发送消息 'cn' ";

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
