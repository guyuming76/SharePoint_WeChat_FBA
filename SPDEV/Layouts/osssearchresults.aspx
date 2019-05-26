<%@ Assembly Name="Microsoft.Office.Server.Search, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%> <%@ Page Language="C#" DynamicMasterPageFile="~masterurl/custom.master" Inherits="Microsoft.Office.Server.Search.Internal.UI.OssSearchResults"   EnableViewState="false"    %> <%@ Import Namespace="Microsoft.Office.Server.Search.Internal.UI" %> <%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Import Namespace="Microsoft.SharePoint" %> <%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 

<%@ Register Tagprefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Register Tagprefix="SearchWC" Namespace="Microsoft.Office.Server.Search.WebControls" Assembly="Microsoft.Office.Server.Search, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SPSWC" Namespace="Microsoft.SharePoint.Portal.WebControls" Assembly="Microsoft.SharePoint.Portal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="MSSWC" Namespace="Microsoft.SharePoint.Portal.WebControls" Assembly="Microsoft.Office.Server.Search, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
    <SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,searchresults_pagetitle%>" EncodeMethod='HtmlEncode'/> : <% SPHttpUtility.HtmlEncode(Request.QueryString["k"], Response.Output); %>
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
<style type="text/css">
    <SharePoint:UIVersionedContent UIVersion="3" runat="server">
        <ContentTemplate>
            .ms-titlearea 
            {
                padding-top: 0px !important;
            }

            .ms-areaseparatorright {
                padding-right: 6px;
            }
            td.ms-areaseparatorleft{
                border-right:0px;
            }
            div.ms-areaseparatorright{
                border-left:0px !important;
            }
        </ContentTemplate>
    </SharePoint:UIVersionedContent>

    <SharePoint:UIVersionedContent UIVersion="4" runat="server">
        <ContentTemplate>
            body #s4-leftpanel {
            background: none !important;
            display:none;
            }
            .s4-ca {
             margin-left: 0px !important;
            }
            
.srchctr_leftcell
{
	width:0px!important;
}

.srchctr_mainleftcell
{
	padding-left:5px!important;
}

.srch-Page A:link{
    font-size:2.5em!important;
}

        </ContentTemplate>
    </SharePoint:UIVersionedContent>
</style> 

</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderTitleAreaClass" runat="server">
<SharePoint:UIVersionedContent UIVersion="3" runat="server">
<ContentTemplate>
ms-searchresultsareaseparator
</ContentTemplate>
</SharePoint:UIVersionedContent>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea"  runat="server">
<SharePoint:UIVersionedContent UIVersion="4" runat="server">
<ContentTemplate>
<label><asp:literal runat="server" Text="<%$Resources:Microsoft.Office.Server.Search, Search_SitePage_Title%>" /></label>
</ContentTemplate>
</SharePoint:UIVersionedContent>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderNavSpacer" runat="server">
</asp:Content>
<%--<asp:Content ContentPlaceHolderId="PlaceHolderBodyAreaClass" runat="server">
<SharePoint:UIVersionedContent UIVersion="3" runat="server">
<ContentTemplate>
ms-formareaframe
</ContentTemplate>
</SharePoint:UIVersionedContent>
</asp:Content>--%>

<asp:Content ContentPlaceHolderID="PlaceHolderTitleBreadcrumb"  runat="server">
<SharePoint:UIVersionedContent UIVersion="3" runat="server">
<ContentTemplate>
    <a name="mainContent"></a>
    <table width="100%" cellpadding="2" cellspacing="0" border="0">
     <tr>
      <td height="25px"><img src="/_layouts/images/blank.gif" width="1" height="1" alt=""></td>
     </tr>
     <tr>
      <td>
        <MSSWC:SearchBoxEx id="SearchBox" runat="server" 
	      GoImageUrl="/_layouts/images/gosearch.gif" 
	      GoImageUrlRTL="/_layouts/images/gosearch.gif"
              UseSiteDefaults = "true" 
          DropDownModeEx=ShowDD_DefaultURL
          ScopeDisplayGroupName = ""
          FrameType="None" 
          ShouldTakeFocusIfEmpty=true />
      </td>
     </tr>
     <tr>
      <td height="10" colspan="8"><IMG SRC="/_layouts/images/blank.gif" width=1 height=1 alt=""></td>
     </tr>
    </table>
</ContentTemplate>
</SharePoint:UIVersionedContent>

<%--<SharePoint:UIVersionedContent UIVersion="4" runat="server">
<ContentTemplate>
<SharePoint:ListSiteMapPath
	runat="server"
	SiteMapProviders="SPSiteMapProvider,SPContentMapProvider"
	RenderCurrentNodeAsLink="false"
	PathSeparator=""
	CssClass="s4-breadcrumb"
	NodeStyle-CssClass="s4-breadcrumbNode"
	CurrentNodeStyle-CssClass="s4-breadcrumbCurrentNode"
	RootNodeStyle-CssClass="s4-breadcrumbRootNode"
	NodeImageOffsetX=0
	NodeImageOffsetY=144
	NodeImageWidth=10
	NodeImageHeight=10
	NodeImageUrl="/_layouts/images/fgimg.png"
	HideInteriorRootNodes="true"
	SkipLinkText="" />		
</ContentTemplate>
</SharePoint:UIVersionedContent>--%>

</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderSearchArea"  runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server"> 
<SharePoint:UIVersionedContent UIVersion="3" runat="server">
<ContentTemplate>
      <table width="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
         <td id="UpperLeftCell" align="left">
          <SearchWC:SearchSummaryWebPart runat="server" FrameType="None"/>
         </td>
         <td id="UpperRightCell" align="right">
           <SearchWC:CoreResultsWebPart runat="server" ChromeType="None" ShowMessages=false ShowSearchResults=false uselocationvisualization=false />
         </td>
        </tr>
        <tr>
         <td id="MidLeftCell" align="left">
                   <SearchWC:SearchStatsWebPart runat="server" FrameType="None"/>         
         </td>
         <td id="MidRightCell" align="right">
                    <SearchWC:SearchPagingWebPart runat="server" FrameType="None"/>
         </td>
        </tr>
        <tr>
         <td id="LowerCell" colspan=2 valign="top">  
        <SearchWC:CoreResultsWebPart runat="server" ShowActionLinks=false ChromeType="None"/>     
         </td>
        </tr>
        <tr>
         <td id="LowerCell" colspan="2">  
           <SearchWC:SearchPagingWebPart runat="server" FrameType="None"/>        
         </td>
        </tr>
      </table>
    </ContentTemplate>
</SharePoint:UIVersionedContent>

<SharePoint:UIVersionedContent UIVersion="4" runat="server">
<ContentTemplate>
     <table width="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td class = "ms-osssearch-SearchAreaTd" style="PADDING-LEFT:10px">
                <div id="s4-searcharea" class="s4-search s4-rp s4-searchbox">
                    <MSSWC:SearchBoxEx id="SearchBox2" runat="server" 
                      GoImageUrl="/_layouts/images/gosearch15.png"
                      GoImageActiveUrl="/_layouts/images/gosearchhover15.png"
                      GoImageUrlRTL="/_layouts/images/gosearchrtl15.png"
                      GoImageActiveUrlRTL="/_layouts/images/gosearchrtlhover15.png"
                      DropDownModeEx=ShowDD_DefaultContextual
                      TextBoxWidth="250"
                      UseSiteDefaults = "true"
                      FrameType="None" 
                      ShouldTakeFocusIfEmpty=true 
                      UseSiteDropDownMode=false
                      QueryPromptString=""/>
                </div>
                <MSSWC:SearchNotification  runat="server"/>
            </td>
            <td class = "ms-osssearch-SearchAreaTd">&nbsp;</td>
        </tr>
      <tr>

         <td class="srchctr_mainleftcell" id="MainLeftCell">
		 <div class="srch-maintop">     
                     <span class="srch-maintopleft">        
                         <SearchWC:SearchStatsWebPart runat="server" FrameType="None"/>
                     </span>
<%--                     <span class="srch-maintopright">
                         <SearchWC:CoreResultsWebPart runat="server" ChromeType="None" ShowMessages=false ShowSearchResults=false UseLocationVisualization=true />
                     </span>--%>
                 </div>
                 <div class="srch-sitesearchmaintop">
                    <SearchWC:CoreResultsWebPart runat="server" ShowActionLinks=false ChromeType="None" DefaultSort="Modified_Date" />     
                 </div>
                 <div class="srch-mainbottom"> 
                    <SearchWC:SearchPagingWebPart runat="server" FrameType="None"/>        
                 </div>
        </td>
            <td class="srchctr_leftcell" id="LeftCell">
                <div class="srch-refinearea">
                    <SearchWC:RefinementWebPart runat="server" FrameType="None" UseDefaultConfiguration="true" />
                </div>
         </td>
          </tr>
      </table>
</ContentTemplate>
</SharePoint:UIVersionedContent>

</asp:Content>

<asp:Content contentplaceholderid="SPNavigation" runat="server"/>

