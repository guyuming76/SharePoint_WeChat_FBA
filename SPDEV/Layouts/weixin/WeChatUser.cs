using Microsoft.SharePoint;
using SharePoint.Helpers;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace weixin
{
    [DataContract]
    public class WeChatUser:SPUserNotesEx
    {
        public WeChatUser() : base() { }
        public WeChatUser(SPUser u) : base(u) { }

        [DataMember]
        public string CultureName;

        [DataMember]
        public bool Debug;

        [DataMember]
        public bool SaveMessageToPublic;

        [DataMember]
        public bool SaveImageToPublic;

        [DataMember]
        public string RecentSearchKeywords;

        public string RecentSearchKeyword
        {
            get { return RecentSearchKeywords; }
        }

        public CultureInfo Culture
        {
            get { return String.IsNullOrEmpty(CultureName) ? new CultureInfo("zh-CN") : new CultureInfo(CultureName); }
            set { CultureName = value.Name; }
        }

        protected override T ForOldNotesData<T>(SPUser u, Exception ex)
        {
            CultureName = u.Notes;
            return base.ForOldNotesData<T>(u, ex);
        }
    }
}
