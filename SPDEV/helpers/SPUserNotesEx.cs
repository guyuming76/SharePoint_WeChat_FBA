using Microsoft.SharePoint;
using Sharepoint.FormsBasedAuthentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SharePoint.Helpers
{
    [DataContract]
    public class SPUserNotesEx
    {
        protected SPUser user;
        protected Guid parentWebId;
        protected Guid SiteCollectionId;
        protected int userid;
        internal SPUserToken usertoken;

        public SPUserNotesEx() { }
        public SPUserNotesEx(SPUser u)
        {
            user = u;
            parentWebId = u.ParentWeb.ID;
            SiteCollectionId = u.ParentWeb.Site.ID;
            userid = u.ID;
            usertoken = u.UserToken;
        }
        

        [DataMember]
        public string OldNotesData;

        // Create a User object and serialize it to a JSON stream.  
        public bool Save<T>() where T:SPUserNotesEx
        {
            //Create User object.  
            //User user = new User("Bob", 42);

            //Create a stream to serialize the object to.  
            using (MemoryStream ms = new MemoryStream())
            {
                // Serializer the User object to the stream.  
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(ms, this);
                byte[] json = ms.ToArray();

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(SiteCollectionId))
                    {
                        using (SPWeb web = site.OpenWeb(parentWebId))
                        {
                            user = web.SiteUsers.GetByID(userid);
                            user.Notes = Encoding.UTF8.GetString(json, 0, json.Length);
                            //之前的SPUser 在SPFBAUser 里初始化后，SPWeb 已经Dispose 了
                            //不重新获取得话会报错： Detected use of SPRequest for previously closed SPWeb object.  
                            user.Update();
                        }
                    }
                });
                return true;
            }
        }

        // Deserialize a JSON stream to a User object.  
        public static T DeserializeFromNotes<T>(SPUser u) where T:SPUserNotesEx,new()
        {
            T deserializedUser = new T();
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(u.Notes)))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                    deserializedUser = ser.ReadObject(ms) as T;
                    deserializedUser.user = u;
                    deserializedUser.parentWebId = u.ParentWeb.ID;
                    deserializedUser.SiteCollectionId = u.ParentWeb.Site.ID;
                    deserializedUser.userid = u.ID;
                    deserializedUser.usertoken = u.UserToken;
                }
            }
            catch(Exception ex) //FormatException???
            {
                deserializedUser = deserializedUser.ForOldNotesData<T>(u, ex);
                deserializedUser.user = u;
                deserializedUser.parentWebId = u.ParentWeb.ID;
                deserializedUser.SiteCollectionId = u.ParentWeb.Site.ID;
                deserializedUser.userid = u.ID;
                deserializedUser.OldNotesData = u.Notes;

                MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.General, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.Message);
                MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.General, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.StackTrace);

            }

            return deserializedUser;
        }

        protected virtual T ForOldNotesData<T>(SPUser u, Exception ex) where T:SPUserNotesEx
        {
            return this as T;
        }
    }
}
