using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace weixin
{
    public partial class MyCustomMessageHandler
    {
        public static  string PublicMessageListUrl
        {
            get { return SPUtility.ConcatUrls(SPContext.Current.Web.ServerRelativeUrl, "Lists/list"); }
        }

        public static string PrivateMessageListUrl
        {
            get { return SPUtility.ConcatUrls(SPContext.Current.Web.ServerRelativeUrl, "lists/Private%20Message"); }
        }

        public string CurrentMessageListUrl
        {
            get { return SPFBAUser.SaveMessageToPublic ? PublicMessageListUrl : PrivateMessageListUrl; }
        }

        public static string ImageLibUrl
        {
            get { return SPUtility.ConcatUrls(SPContext.Current.Web.ServerRelativeUrl, "Images1"); }
        }
    }
}
