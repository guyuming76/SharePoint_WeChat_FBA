using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI;

namespace SPDEV
{
    [ToolboxItemAttribute(false)]
    public partial class QuickLaunchToRightFixForPictureLibView : WebPart
    {
        // 仅当使用检测方法对场解决方案进行性能分析时才取消注释以下 SecurityPermission
        // 特性，然后在代码准备进行生产时移除 SecurityPermission 特性
        // 特性。因为 SecurityPermission 特性会绕过针对您的构造函数的调用方的
        // 安全检查，不建议将它用于生产。
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public QuickLaunchToRightFixForPictureLibView()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected override void Render(HtmlTextWriter writer)
        {
            string path = HttpContext.Current.Request.Path;
            if (SPContext.Current.List != null
                && SPListTemplateType.PictureLibrary.Equals(SPContext.Current.List.BaseTemplate)
                && SPContext.Current.ViewContext != null
                && SPContext.Current.ViewContext.View != null
                && "1" == SPContext.Current.ViewContext.View.BaseViewID
                && !(path.ToLower().EndsWith("viewedit.aspx")))
            {
                writer.Write("<style type='text/css'>#contentthumbnail,#contentfilmstrip{width:75%!important;} .s4-ca{min-height:0px!important;}</style>");
                //writer.Write("<script language='javascript'> _spBodyOnLoadFunctionNames.push('ViewFooterScript()');</script>");
                writer.Write("<script type='text/javascript'> ViewFooterScript();</script>");
            }
        }
    }
}
