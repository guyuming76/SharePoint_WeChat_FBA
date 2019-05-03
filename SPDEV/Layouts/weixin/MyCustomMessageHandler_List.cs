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

        public string GetLatestMessageSubject(DateTime beforetime, out string nextBeforeTime,bool publicmessage)
        {
            using (new SPMonitoredScope("Weixin.GetLatestMessageSubject", 5000))
            {

                Guid siteid = SPContext.Current.Site.ID;
                Guid webid = SPContext.Current.Web.ID;
                //Guid siteid = new Guid("f424bb38-822a-4d31-b61d-49207f22b510");
                //Guid webid = new Guid("341722fa-3840-417d-8ed9-3f8aa7a735dc");
                StringBuilder ret = new StringBuilder();

                using (SPSite site = new SPSite(siteid, SPFBAUser.usertoken))

                //using (SPSite site = new SPSite(siteid))
                {
                    using (SPWeb web = site.OpenWeb(webid))
                    {
                        //SPList currentMessageList = web.GetList("/sites/test/lists/Team%20Discussion/");
                        SPList currentMessageList = web.GetList(publicmessage ? PublicMessageListUrl : PrivateMessageListUrl);
                        SPQuery qry = new SPQuery();
                        qry.RowLimit = 5;
                        qry.Query = "<OrderBy Override=\"TRUE\"><FieldRef Name=\"Modified\" Ascending=\"FALSE\" /></OrderBy><Where>"
                              + "<Lt><FieldRef Name='Created'/><Value IncludeTimeValue='TRUE' Type='DateTime'>"
                              + SPUtility.CreateISO8601DateTimeFromSystemDateTime(beforetime)
                              + "</Value></Lt></Where>";
                        qry.ViewFields = @"<FieldRef Name=""Author"" /><FieldRef Name=""ItemChildCount"" /><FieldRef Name=""Modified"" /><FieldRef Name=""Title"" />";

                        try
                        {
                            SPListItemCollection result = currentMessageList.GetItems(qry);

                            if (result != null && result.Count > 0)
                            {
                                string lastTime = string.Empty;
                                foreach (SPListItem item in result)
                                {
                                    string author = item["Author"] == null ? string.Empty : item["Author"].ToString().Trim();
                                    SPFieldUserValue au = author.Equals(string.Empty) ? null :
                                        (item.Fields.GetFieldByInternalName("Author").GetFieldValue(item["Author"].ToString()) as SPFieldUserValue);
                                    string u = au == null ? string.Empty
                                        : (au.User == null ? string.Empty : au.User.Name);

                                    ret.AppendLine(string.Concat("主题:", item["Title"]));
                                    ret.AppendLine(string.Concat("作者:", u));
                                    ret.AppendLine(string.Concat("修改时间", item["Modified"].ToString()));
                                    ret.AppendLine(string.Concat("回复数:", item["ItemChildCount"]));
                                    ret.AppendLine();
                                    lastTime = item["Modified"].ToString();
                                }

                                nextBeforeTime = lastTime;
                                return ret.ToString();
                            }
                            else
                            {
                                nextBeforeTime = string.Empty;
                                return "已无更早留言";
                            }
                        }
                        catch(Exception ex)
                        {
                            nextBeforeTime = string.Empty;
                            return ex.Message;
                        }
                    }
                }
            }

        }
    }
}
