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
        public string PublicMessageListUrl
        {
            get { return SPUtility.ConcatUrls(SPContext.Current.Web.ServerRelativeUrl, "Lists/list"); }
        }

        public string PrivateMessageListUrl
        {
            get { return SPUtility.ConcatUrls(SPContext.Current.Web.ServerRelativeUrl, "lists/Private%20Message"); }
        }

        public string CurrentMessageListUrl
        {
            get { return SPFBAUser.SaveMessageToPublic ? PublicMessageListUrl : PrivateMessageListUrl; }
        }

    }
}
