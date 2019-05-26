using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls.WebParts;

namespace SPDEV
{
    [ToolboxItemAttribute(false)]
    public partial class HideQuickLauchForListForm : WebPart
    {
        // 仅当使用检测方法对场解决方案进行性能分析时才取消注释以下 SecurityPermission
        // 特性，然后在代码准备进行生产时移除 SecurityPermission 特性
        // 特性。因为 SecurityPermission 特性会绕过针对您的构造函数的调用方的
        // 安全检查，不建议将它用于生产。
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public HideQuickLauchForListForm()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            string path = HttpContext.Current.Request.Path;
            if(SPContext.Current.List!=null)
            {
                //for discusionboard, the flat.aspx is based on listview instead of listform
                if (path.ToLower().EndsWith("/flat.aspx") && SPListTemplateType.DiscussionBoard.Equals(SPContext.Current.List.BaseTemplate))
                {
                    HttpContext.Current.Response.Write("<style>#s4-leftpanel-content{display:none!important;</style>");
                }
                else if (SPContext.Current.FormContext == null || SPContext.Current.FormContext.FormMode.Equals(SPControlMode.Invalid))
                //how to find out current page is a listform page?
                {
                    
                }
                else
                {
                    HttpContext.Current.Response.Write("<style>#s4-leftpanel-content{display:none!important;</style>");
                }
            }
        }
    }
}
