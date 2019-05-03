<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClaimViewer.aspx.cs" Inherits="Security.Layouts.Security.ClaimViewer" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
       <div style="margin-bottom:8px;">

   <h2>SAML Token for Current User(this.Page.User.Identity)</h2>
       BootstrapToken.TokenID:<asp:Label ID="TokenID" runat="server">claimsIdentity.BootstrapToken.ID</asp:Label><br />
       BootstrapToken.TokenValidFrom: <asp:Label ID="TokenValidFrom" runat="server">claimsIdentity.BootstrapToken.ValidFrom</asp:Label><br />
       BootstrapToken.TokenValidTo: <asp:Label ID="TokenValidTo" runat="server">claimsIdentity.BootstrapToken.ValidTo</asp:Label><br />
       
    <asp:GridView ID="grdClaims" runat="server" AutoGenerateColumns="true" CssClass="gridTable" RowStyle-Wrap="true" AlternatingRowStyle-Wrap="true" Width="800px" >
      <HeaderStyle CssClass="gridHeaderRow" />
      <RowStyle CssClass="gridRow" />
      <AlternatingRowStyle CssClass="gridAlternatingRow" />
    </asp:GridView>

    <asp:Label ID="status" runat="server" />

  </div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    ClaimViewer
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    MyClaimViewer
</asp:Content>
