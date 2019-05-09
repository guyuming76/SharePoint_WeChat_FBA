using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Senparc.Weixin.MP.Entities;
using System;
using System.Collections.Generic;
using System.Data;
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


        //public string GetLatestMessageNews(DateTime beforetime, out string nextBeforeTime, bool publicmessage)
        public static string GetLatestMessageNews(DateTime beforetime, bool publicmessage)
        {
            using (new SPMonitoredScope("Weixin.GetLatestMessageNews", 5000))
            {
                //Guid siteid = SPContext.Current.Site.ID;
                //Guid webid = SPContext.Current.Web.ID;
                Guid siteid = new Guid("fbf5935b-6717-4036-8bfc-856c1ef075a7");
                Guid webid = new Guid("3acebb12-b9c8-4a00-94f7-4d779743e805");

                SPUserToken t=null;
                SPSecurity.RunWithElevatedPrivileges(delegate () { using (SPSite s=new SPSite(siteid)) { using (SPWeb w = s.OpenWeb(webid)) { t = w.GetUserToken("i:0#.f|fbamember|233173287"); } } });


                StringBuilder ret = new StringBuilder();

                //using (SPSite site = new SPSite(siteid, SPFBAUser.usertoken))
                using (SPSite site = new SPSite(siteid,t))
                {
                    using (SPWeb web = site.OpenWeb(webid))
                    {
                        SPSiteDataQuery query = new SPSiteDataQuery();

                        //query.Lists = string.Concat("<Lists><List ID = '", web.GetList(publicmessage ? PublicMessageListUrl : PrivateMessageListUrl).ID.ToString(), "' /><List ID = '", web.GetList(ImageLibUrl).ID.ToString(), "' /></Lists>");
                        //query.Webs = "<Webs Scope=\"SiteCollection\"/>";
                        query.Lists = string.Concat("<Lists><List ID = '", web.GetList(publicmessage ? "/sites/public/lists/list/" : "/sites/public/lists/Private%20Message/").ID.ToString(), "' /><List ID = '", web.GetList("/sites/public/Images1/").ID.ToString(), "' /></Lists>");
                        query.ViewFields = @"<FieldRef Name=""Author"" Nullable=""TRUE"" /><FieldRef Name=""ItemChildCount"" Nullable=""TRUE"" /><FieldRef Name=""Modified""/><FieldRef Name=""Title"" Nullable=""TRUE"" />";
                        query.ViewFields += @"<FieldRef Name=""WeChatPicUrl"" Nullable=""TRUE""/>";
                        query.RowLimit = 5;
                        query.Query = "<OrderBy><FieldRef Name=\"Modified\" Ascending=\"FALSE\" /></OrderBy><Where>"
                              + "<And>"
                              + "<Or>"
                      //           + "<And><IsNotNull><FieldRef Name=\"WeChatPicUrl\" Nullable=\"TRUE\"/></IsNotNull>"
                                     + "<IsNotNull><FieldRef Name=\"WeChatPicUrl\" Nullable=\"TRUE\"/></IsNotNull>"
                                 //           + (publicmessage? string.Concat("<Eq>< FieldRef Name = \"FileDirRef\" ></FieldRef ><Value Type = \"Text\" >",ImageLibUrl,"</Value></Eq>"): string.Concat("<Contains>< FieldRef Name = \"FileDirRef\" ></FieldRef ><Value Type = \"Text\" >", SPFBAUserName, "</Value><Contains>"))
                                 //              + (publicmessage ? string.Concat("<Eq><FieldRef Name = \"FileDirRef\" /><Value Type = \"Text\" >", "/sites/public/Images1/", "</Value></Eq>") : string.Concat("<Contains>< FieldRef Name = \"FileDirRef\" /><Value Type = \"Text\" >", "233173287", "</Value></Contains>"))
                                 //            + "</And>"
                                    + "<BeginsWith><FieldRef Name = \"ContentTypeId\" /><Value Type = \"ContentTypeId\" >0x012002</Value></BeginsWith>"
                              + "</Or>"
                                + "<Lt><FieldRef Name='Created'/><Value IncludeTimeValue ='TRUE' Type ='DateTime'>"
                                      + SPUtility.CreateISO8601DateTimeFromSystemDateTime(beforetime)
                                      + "</Value></Lt>"
                              + "</And></Where>";

                        DataTable results = web.GetSiteData(query);
                        if (results != null && results.Rows.Count > 0)
                        {
                            //var responseMessageNews = CreateResponseMessage<ResponseMessageNews>();
                            //Article a = new Article();
                            ////string prevAuthor = string.Empty;
                            //int i = 0;
                            //StringBuilder ret = new StringBuilder();
                            //foreach (DataRow r in results.Rows)
                            //{
                            //    string author = r["Author"] == null ? string.Empty : r["Author"].ToString().Trim();
                            //    //SPFieldUserValue au = author.Equals(string.Empty) ? null :
                            //    //    (item.Fields.GetFieldByInternalName("Author").GetFieldValue(item["Author"].ToString()) as SPFieldUserValue);
                            //    //string u = au == null ? string.Empty
                            //    //    : (au.User == null ? string.Empty : au.User.Name);
                            //    if (string.IsNullOrEmpty((r["WeChatPicUrl"] ?? string.Empty).ToString()))
                            //    {
                            //        ret.AppendLine(string.Concat(author, ":", r["Title"]));
                            //        ret.AppendLine(string.Concat("  修改时间", r["Modified"].ToString()));
                            //    }
                            //}

                            //responseMessageNews.Articles.Add(a);

                            string lastTime = string.Empty;
                            foreach (DataRow item in results.Rows)
                            {
                                string author = item["Author"] == null ? string.Empty : item["Author"].ToString().Trim();
                                //SPFieldUserValue au = author.Equals(string.Empty) ? null :
                                //    (item.Fields.GetFieldByInternalName("Author").GetFieldValue(item["Author"].ToString()) as SPFieldUserValue);
                                //string u = au == null ? string.Empty
                                //    : (au.User == null ? string.Empty : au.User.Name);

                                if (item["WeChatPicUrl"] == null)
                                {
                                    ret.AppendLine(string.Concat("主题:", item["Title"]));
                                    ret.AppendLine(string.Concat("作者:", author));
                                    ret.AppendLine(string.Concat("修改时间", item["Modified"].ToString()));
                                    ret.AppendLine(string.Concat("回复数:", item["ItemChildCount"]));
                                }
                                else
                                {
                                    ret.AppendLine(string.Concat("作者:", author));
                                    ret.AppendLine(string.Concat("修改时间", item["Modified"].ToString()));
                                    ret.AppendLine(string.Concat("PicUrl", item["WeChatPicUrl"].ToString()));
                                }
                                ret.AppendLine();
                                lastTime = item["Modified"].ToString();
                            }

                            //nextBeforeTime = lastTime;
                            return ret.ToString();

                        }
                        else
                        {
                            //nextBeforeTime = string.Empty;
                            return "已无更早留言";
                        }
                        //SPList ImageLib = web.GetList(ImageLibUrl);
                        //SPQuery qry = new SPQuery();
                        //qry.RowLimit = 1;
                        //qry.Query = "<OrderBy Override=\"TRUE\"><FieldRef Name=\"Modified\" Ascending=\"FALSE\" /></OrderBy><Where>"
                        //      + "<Lt><FieldRef Name='Created'/><Value IncludeTimeValue='TRUE' Type='DateTime'>"
                        //      + SPUtility.CreateISO8601DateTimeFromSystemDateTime(beforetime)
                        //      + "</Value></Lt></Where>";
                        //qry.ViewFields = @"<FieldRef Name=""Author"" /><FieldRef Name=""Modified"" /><FieldRef Name=""WeChatPicUrl"" />";


                    }
                }
            }
        }
    }
}
