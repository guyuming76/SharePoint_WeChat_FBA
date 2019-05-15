using Microsoft.Office.Server.Search.Administration;
using Microsoft.Office.Server.Search.Query;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
//using Microsoft.SharePoint.Search.Query;

namespace weixin
{
    public partial class MyCustomMessageHandler
    {
        public const string AppearInWeChat = "AppearInWeChat";
        public const string WeChatResult = "WeChatResult";

        public const string searchKeyWordCmdPrefix = "ss";
        //提示用户输入搜索关键词的用起来貌似有些复杂
        //
        public const string contentSourceName = "Local SharePoint sites";
        public string GetSearchKeyWord(string messageInput)
        {
            throw new NotImplementedException();
            // 去掉ss 前缀
            // 如果剩下为空，则从 SPFBAUser.RecentSearchKeyword 中取

            // 如果剩下不为空， 则返回剩下部分，并把剩下部分存入SPFBAUser.RecentSearchKeyword
        }

        public string GetSearchResult(string messageInput)
        {
            string k = GetSearchKeyWord(messageInput);
            string ret = string.Empty;

            using (SPMonitoredScope scope = new SPMonitoredScope("MyCustomMessageHandler.GetSearchResult"))
            {
                //KeywordQuery keywordquery = new KeywordQuery(SPContext.Current.Site);
                //keywordquery.ResultTypes = ResultType.RelevantResults;
                //keywordquery.QueryText = string.Concat("ContentSource=", contentSourceName, " ", AppearInWeChat, "=True");
                //keywordquery.SelectProperties.Add(WeChatResult);
                //keywordquery.TrimDuplicates = false;
                //keywordquery.RowsPerPage = 0;
                //keywordquery.RowLimit = 10;
                //keywordquery.Timeout = 5000;

                //SearchExecutor searchexecutor = new SearchExecutor();
                //2013 foundation 里面的SearchExecutor这里没有
                //ResultTableCollection resulttablecollection = searchexecutor.ExecuteQuery(keywordquery);
                //ResultTable resulttable = resulttablecollection.Filter("TableType", KnownTableTypes.RelevantResults).FirstOrDefault();

                //https://docs.microsoft.com/en-us/previous-versions/office/developer/sharepoint-2010/ms493601(v=office.14)
                SearchServiceApplicationProxy proxy = (SearchServiceApplicationProxy)SearchServiceApplicationProxy.GetProxy(SPServiceContext.GetContext(SPContext.Current.Site));
                KeywordQuery query = new KeywordQuery(proxy);
                query.ResultsProvider = Microsoft.Office.Server.Search.Query.SearchProvider.Default;
                query.QueryText = string.Concat("ContentSource=", contentSourceName, " ", AppearInWeChat, "=True");
                query.ResultTypes |= ResultType.RelevantResults;
                ResultTableCollection searchResults = query.Execute();
                if (searchResults.Exists(ResultType.RelevantResults))
                {
                    ResultTable searchResult = searchResults[ResultType.RelevantResults];
                    DataTable result = new DataTable();
                    result.TableName = "Result";
                    result.Load(searchResult, LoadOption.OverwriteChanges);

                    StringBuilder sb = new StringBuilder();
                    foreach(DataRow r in result.Rows)
                    {
                        sb.Append(r[WeChatResult]);
                        sb.Append(System.Environment.NewLine);
                    }
                    ret = sb.ToString();
                }

            }

            return ret;
        }
    }
}
