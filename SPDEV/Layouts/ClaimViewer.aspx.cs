using System;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.IdentityModel.Claims;
using System.Linq;

namespace Security.Layouts.Security
{
    class ClaimTypeListItem
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }

    public partial class ClaimViewer : LayoutsPageBase
    {

        protected override void OnPreRender(EventArgs e)
        {
            var UserClaims = new List<ClaimTypeListItem>();

            IIdentity identity = this.Page.User.Identity;

            if (identity is IClaimsIdentity)
            {
                IClaimsIdentity claimsIdentity = (IClaimsIdentity)identity;
                foreach (var claim in claimsIdentity.Claims)
                {
                    UserClaims.Add(
                      new ClaimTypeListItem
                      {
                          ClaimType = claim.ClaimType,
                          ClaimValue = claim.Value
                      });
                }

                TokenID.Text = claimsIdentity.BootstrapToken.Id;
                TokenValidFrom.Text = claimsIdentity.BootstrapToken.ValidFrom.ToString();
                TokenValidTo.Text = claimsIdentity.BootstrapToken.ValidTo.ToString();

            }

            grdClaims.DataSource = UserClaims.OrderByDescending(claim => claim.ClaimType);
            grdClaims.DataBind();

        }

    }

}
