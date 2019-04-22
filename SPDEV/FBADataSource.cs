using System;
using System.Web.UI;
using System.Collections;

namespace Sharepoint.FormsBasedAuthentication
{
    /// <summary>
    /// Provides data sources for User and Role display pages
    /// </summary>
    public class MyFBADataSource : DataSourceControl
    {
        private string _viewName;
        private DataSourceView _view = null;

        public MyFBADataSource() : base() { }

        public string ViewName
        {
            get { return _viewName; }
            set { _viewName = value; }
        }

        public string SearchText
        {
            get
            {
                string s = (string)ViewState["SearchText"];
                return (s != null) ? s : String.Empty;
            }

            set { ViewState["SearchText"] = value; }
        }

        protected string _MandatoryRowFilter;
        public string MandatoryRowFilter
        {
            get { return _MandatoryRowFilter; }
            set { _MandatoryRowFilter = value; }

        }

        protected string _linkExpireTime;
        public string linkExpireTime
        {
            get { return _linkExpireTime; }
            set { _linkExpireTime = value; }
        }

        protected bool _IncludeSPSiteHiddenUserInfor = true;
        public bool IncludeSPSiteHiddenUserInfor
        {
            get { return _IncludeSPSiteHiddenUserInfor; }
            set { _IncludeSPSiteHiddenUserInfor = value; }
        }

        public bool ResetCache { get; set; }

        /// <summary>
        /// return a strongly typed view for the current data source control
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns> 
        protected override DataSourceView GetView(string viewName)
        {
            // only retrieve a view if a membership provider can be found
            if (_view == null)
            {
                
                try
                {
                    if (ViewName == "FBAUsersView")
                        _view = new MyFBAUsersView(this, viewName);
                    else if (ViewName == "FBARolesView")
                        _view = new MyFBARolesView(this, viewName);
                }
                catch (Exception ex)
                {
                    Utils.LogError(ex, true);
                }
            }
            return _view;
        }

        /// <summary>
        /// return a collection of available views
        /// </summary>
        /// <returns></returns> 
        protected override ICollection GetViewNames()
        {
            ArrayList views = new ArrayList(2);
            views.Add("FBAUsersView");
            views.Add("FBARolesView");
            return views as ICollection;
        }
    }

}
