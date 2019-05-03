using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Runtime.InteropServices;

namespace Sharepoint.FormsBasedAuthentication
{
    [Guid("ca57a7cc-ad72-4bda-a42b-b1426fc14743")]
    public class MyFBADiagnosticsService : SPDiagnosticsServiceBase
    {

        public static string AreaName = "SignForm and UserRegistration";

        public static string ServiceName = "SignForm and UserRegistration Diagnostics Service";

        public enum FBADiagnosticsCategory
        {
            General,
            Weixin,
            Test
        } // enum MyDiagnosticsCategory

        public MyFBADiagnosticsService()
            : base(ServiceName, SPFarm.Local)
        {
        } // ctor()

        public MyFBADiagnosticsService(string name, SPFarm parent)
            : base(name, parent)
        {
            // SPDiagnosticsServiceBase.GetLocal() wants the default ctor and this one
        } // ctor()

        protected override bool HasAdditionalUpdateAccess()
        {
            // Without this SPDiagnosticsServiceBase.GetLocal<MyDiagnosticsService>()
            // throws a SecurityException, see
            // http://share2010.wordpress.com/tag/sppersistedobject/
            return true;
        } // HasAdditionalUpdateAccess()

        public static MyFBADiagnosticsService Local
        {
            get
            {
                // SPUtility.ValidateFormDigest(); doesn't work here
                if (SPContext.Current != null)
                {
                    SPContext.Current.Web.AllowUnsafeUpdates = true;
                }
                // (Else assume this is called from a FeatureReceiver)
                return SPDiagnosticsServiceBase.GetLocal<MyFBADiagnosticsService>();
            }
        } // Local

        public void WriteTrace(ushort id, FBADiagnosticsCategory fbaDiagnosticsCategory, TraceSeverity traceSeverity, string message, params object[] data)
        {
            if (traceSeverity != TraceSeverity.None)
            {
                // traceSeverity==TraceSeverity.None would cause an ArgumentException:
                // "Specified value is not supported for the severity parameter."
                SPDiagnosticsCategory category = Local.Areas[AreaName].Categories[fbaDiagnosticsCategory.ToString()];
                Local.WriteTrace(id, category, traceSeverity, message, data);
            }
        } // LogMessage()

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsCategory> categories = new List<SPDiagnosticsCategory>();
            categories.Add(new SPDiagnosticsCategory(FBADiagnosticsCategory.General.ToString(), TraceSeverity.Verbose, EventSeverity.None));
            categories.Add(new SPDiagnosticsCategory(FBADiagnosticsCategory.Weixin.ToString(), TraceSeverity.Verbose, EventSeverity.None));
            categories.Add(new SPDiagnosticsCategory(FBADiagnosticsCategory.Test.ToString(), TraceSeverity.Medium, EventSeverity.None));

            SPDiagnosticsArea area = new SPDiagnosticsArea(AreaName, 0, 0, false, categories);

            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>();

            areas.Add(area);

            return areas;
        } // ProvideAreas()

    }
}
