using Microsoft.SharePoint;
using Sharepoint.FormsBasedAuthentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SharePoint.Helper
{
    [DataContract]
    public class SPUserNotesEx
    {
        public SPUser user;
        public SPUserNotesEx() { }
        public SPUserNotesEx(SPUser u) { user = u; }

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

                user.Notes = Encoding.UTF8.GetString(json, 0, json.Length);
                user.Update();
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
                }
            }
            catch(Exception ex) //FormatException???
            {
                deserializedUser = deserializedUser.ForOldNotesData<T>(u, ex);
                deserializedUser.user = u;
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
