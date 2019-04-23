using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Sharepoint.FormsBasedAuthentication;
using SharePoint.Helpers;
using System;
using System.Web;

namespace weixin
{
    public class weixinHttpHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        public static string WeChatPublicAccountNameAndFBARole
        {
            get { return SPContext.Current.Web.GetSPWebProperty("WeChatPublicAccountNameAndFBARole"); }
            set
            {
                SPWeb w = SPContext.Current.Web;
                SPHelper.SetSPWebProperty(w, "WeChatPublicAccountNameAndFBARole", value);
            }

        }


        internal static string WeChatServiceAppID
        {
            //TODO: 研究下AppID 要保密吗?
  
            get { return SPContext.Current.Web.GetSPWebProperty("WeChatServiceAppID"); }
            set {
                SPWeb w = SPContext.Current.Web;
                SPHelper.SetSPWebProperty(w, "WeChatServiceAppID", value);
            }
        }

        internal static string WeChatServiceUrl
        {
            get { return SPUtility.ConcatUrls(SPContext.Current.Web.Url,"_layouts/weixin/weixin.ashx"); }
        }

        internal static string WeChatServiceToken
        {
            get
            {
                return EncryptTool.Decrypt(SPContext.Current.Web.GetSPWebProperty("WeChatServiceToken"), MyCustomMessageHandler.SecretGuid, false);
            }
            set
            {
                string encryted = EncryptTool.Encrypt(value, MyCustomMessageHandler.SecretGuid, false);
                SPWeb w = SPContext.Current.Web;
                SPHelper.SetSPWebProperty(w, "WeChatServiceToken", encryted);
            }
        }

        internal static string WeChatServiceEncodingAESKey
        {
            get { return EncryptTool.Decrypt(SPContext.Current.Web.GetSPWebProperty("WeChatServiceEncodingAESKey"), MyCustomMessageHandler.SecretGuid, false); }
            set {
                string encryted = EncryptTool.Encrypt(value, MyCustomMessageHandler.SecretGuid, false);
                SPWeb w = SPContext.Current.Web;
                SPHelper.SetSPWebProperty(w, "WeChatServiceEncodingAESKey", encryted);
            }
        }


        public void ProcessRequest(HttpContext context)
        {
            using (SPMonitoredScope m=new SPMonitoredScope("Weixin.ProcessRequest", 5000))
            {
                MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Verbose, string.Concat(context.Request.HttpMethod, ":", context.Request.RawUrl));

                string signature = context.Request["signature"];
                string timestamp = context.Request["timestamp"];
                string nonce = context.Request["nonce"];
                string echostr = context.Request["echostr"];
                string servicetoken = WeChatServiceToken;

                if (context.Request.HttpMethod == "GET")
                {
                    //get method - 仅在微信后台填写URL验证时触发
                    if (CheckSignature.Check(signature, timestamp, nonce, servicetoken))
                    {
                        context.Response.Output.Write(echostr); //返回随机字符串则表示验证通过
                    }
                    else
                    {
                        string err = "failed:" + signature + "," + CheckSignature.GetSignature(timestamp, nonce, servicetoken) + "。" +
                                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。";

                        context.Response.Output.Write(err);
                        MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, err);
                    }
                    context.Response.End();
                }
                else
                {

                    //post method - 当有用户想公众账号发送消息时触发
                    if (!CheckSignature.Check(signature, timestamp, nonce, servicetoken))
                    {
                        string err = "参数错误！";

                        context.Response.Output.Write(err);
                        MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, err);
                        return;
                    }

                    //post method - 当有用户想公众账号发送消息时触发
                    var postModel = new PostModel()
                    {
                        Signature = context.Request.QueryString["signature"],
                        Msg_Signature = context.Request.QueryString["msg_signature"],
                        Timestamp = context.Request.QueryString["timestamp"],
                        Nonce = context.Request.QueryString["nonce"],
                        //以下保密信息不会（不应该）在网络上传播，请注意
                        //Token = Token,
                        Token = servicetoken,
                        EncodingAESKey = WeChatServiceEncodingAESKey,//根据自己后台的设置保持一致
                        //AppId = "wxee56a98aeb2690f4"//根据自己后台的设置保持一致
                        AppId = WeChatServiceAppID,
                    };

                    //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
                    var maxRecordCount = 10;

                    //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
                    var messageHandler = new MyCustomMessageHandler(context.Request.InputStream, postModel, maxRecordCount,context);
                    
                    try
                    {
                        messageHandler.OmitRepeatedMessage = true;

                        //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
                        //messageHandler.RequestDocument.Save(
                        //    context.Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Request_" +
                        //                   messageHandler.RequestMessage.FromUserName + ".txt"));
                        string reqmsg = string.Concat(messageHandler.RequestMessage.FromUserName, "@@@@", messageHandler.RequestDocument.ToString());
                        MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Medium, string.Concat("RequestDoc:", reqmsg));

                        //执行微信处理过程
                        messageHandler.Execute();
                        //测试时可开启，帮助跟踪数据
                        //messageHandler.ResponseDocument.Save(
                        //    context.Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Response_" +
                        //                   messageHandler.ResponseMessage.ToUserName + ".txt"));
                        if (messageHandler.SPFBAUser.Debug)
                        {
                            string debug = string.Concat("start-end(mm:ss:ff): ", string.Format("{0:mm:ss:ff}", m.GetMonitor<SPExecutionTimeCounter>().StartTime), "-", string.Format("{0:mm:ss:ff}", m.GetMonitor<SPExecutionTimeCounter>().EndTime), ", duration(ms):", m.GetMonitor<SPExecutionTimeCounter>().Value);
                            (messageHandler.ResponseMessage as ResponseMessageText).Content += string.Concat(System.Environment.NewLine, debug);
                        }

                        context.Response.Output.Write(messageHandler.ResponseDocument.ToString());
                        return;
                    }
                    catch (Exception ex)
                    {
                        MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, string.Concat(ex.Message, ex.StackTrace));
                        if (ex.InnerException != null) MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, string.Concat("InnerException:", ex.InnerException.Message, ex.InnerException.StackTrace));
                        if (messageHandler.ResponseDocument != null) MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, string.Concat("RespondDoc:", messageHandler.ResponseDocument.ToString()));
                    }
                    finally
                    {
                        
                        context.Response.End();
                    }
                }
            }
        }




    }
}
