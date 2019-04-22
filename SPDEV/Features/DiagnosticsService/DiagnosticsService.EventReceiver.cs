using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Sharepoint.FormsBasedAuthentication;

namespace SPDEV.Features.DiagnosticsService
{
    /// <summary>
    /// 此类用于处理在激活、停用、安装、卸载和升级功能的过程中引发的事件。
    /// </summary>
    /// <remarks>
    /// 附加到此类的 GUID 可能会在打包期间使用，不应进行修改。
    /// </remarks>

    [Guid("8a985f6a-e125-40af-bdc1-be60d16d03e9")]
    public class DiagnosticsServiceEventReceiver : SPFeatureReceiver
    {
        // 取消对以下方法的注释，以便处理激活某个功能后引发的事件。

        //public override void FeatureActivated(SPFeatureReceiverProperties properties)
        //{

        //}


        // 取消对以下方法的注释，以便处理在停用某个功能前引发的事件。

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


        // 取消对以下方法的注释，以便处理在安装某个功能后引发的事件。

        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
            MyFBADiagnosticsService.Local.Update();
        }


        // 取消对以下方法的注释，以便处理在卸载某个功能前引发的事件。

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            MyFBADiagnosticsService.Local.Delete();
        }

        // 取消对以下方法的注释，以便处理在升级某个功能时引发的事件。

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
