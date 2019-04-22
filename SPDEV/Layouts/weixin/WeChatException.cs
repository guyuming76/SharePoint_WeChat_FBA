using Microsoft.SharePoint;

namespace weixin
{
    public class WeChatException:SPException
    {
        public WeChatException(string msg) : base(msg) { }
    }

    public class ExceptionCreateSPFBAUser:WeChatException
    {
        public ExceptionCreateSPFBAUser(string msg) : base(string.Concat("生成用户 ",msg, " 出错,可以尝试取消关注，再重新关注，让系统重新生成网站账户。也可在网站匿名公开留言，提醒管理员解决。"
            , System.Environment.NewLine
            ,"Errror creating user ",msg," ,please try unfollow and follow again, or leave a message on the web site to get help from administrator.")) { }
    }

    //public class ExceptionSPFBAUser:WeChatException
    //{
    //    public ExceptionSPFBAUser(string msg) : base(string.Concat("获取用户 ", msg, " 出错,可以尝试取消关注，再重新关注，让系统重新生成网站账户。也可在网站匿名公开留言，提醒管理员解决。"
    //        , System.Environment.NewLine
    //        , "Errror creating user ", msg, " ,please try unfollow and follow again, or leave a message on the web site to get help from administrator."))
    //    { }
    //}

    public class ExceptionWriteMessageIntoSharePoint:WeChatException
    {
        public ExceptionWriteMessageIntoSharePoint(string msg) : base(string.Concat("Failed to save message;保存消息出错，用户名：", msg)) { }
    }

    public class ExceptionSetCultureForUser:WeChatException
    {
        public ExceptionSetCultureForUser(string msg) : base(string.Concat("Exception:设置语言出错，用户名：",msg)) { }
    }

    public class ExceptionGetCultureForUser:WeChatException
    {
        public ExceptionGetCultureForUser(string msg) : base(string.Concat("Exception:获取语言设置出错，用户名：", msg)) { }
    }

}
