<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="sysweb" Namespace="System.Web.UI.WebControls" Assembly="System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" Inherits="Sharepoint.FormsBasedAuthentication.LoginForm1" MasterPageFile="~/_layouts/simple.master"  %>



<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
<%--    <script type="text/javascript"  charset="utf-8"
        src="http://connect.qq.com/qc_jssdk.js"
        data-appid="APPID"
        data-redirecturi="REDIRECTURI">

    </script>--%>
    <style>
        div.s4-simple-iconcont img{
            display:none;
        }

        div.s4-simple-iconcont {
            background-image: url("<%=qrUrl%>");
            height:100px;
            width:100px;
        }
    </style>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
	<SharePoint:EncodedLiteral runat="server"  EncodeMethod="HtmlEncode" Id="ClaimsFormsPageTitle" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
	<SharePoint:EncodedLiteral runat="server"  EncodeMethod="HtmlEncode" Id="ClaimsFormsPageTitleInTitleArea" Visible="false" />
    <sysweb:Label ID="QR" runat="server" Text="<%$Resources:MyResource,LoginFormQR%>"></sysweb:Label><asp:Label ID="guest" runat="server" Text="<%$Resources:MyResource,guest%>" Visible="true"></asp:Label>
</asp:Content>




<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <%--<asp:Image ID="WeixinQr" runat="server" />--%>

     <div id="SslWarning" style="color:red;display:none">
        <SharePoint:EncodedLiteral runat="server"  EncodeMethod="HtmlEncode" Id="ClaimsFormsPageMessage" />
    </div>


<%--    <span id="qqLoginBtn"></span>
    <script type="text/javascript">
        QC.Login({
            btnId:"qqLoginBtn"	//插入按钮的节点id
        });
    </script>--%>

     <asp:login id="signInControl" FailureText="<%$Resources:wss,login_pageFailureText%>" runat="server" width="100%">
	    <layouttemplate>
		<asp:label id="FailureText" class="ms-error" runat="server"/>
		<table width="100%">
		<tr>
			<td nowrap="nowrap"><SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,login_pageUserName%>" EncodeMethod='HtmlEncode'/></td>
			<td width="100%"><asp:textbox id="UserName" autocomplete="off" runat="server" class="ms-inputuserfield" width="99%" /></td>
		</tr>
		<tr>
			<td nowrap="nowrap"><SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,login_pagePassword%>" EncodeMethod='HtmlEncode'/></td>
			<td width="100%"><asp:textbox id="password" TextMode="Password" autocomplete="off" runat="server" class="ms-inputuserfield" width="99%"/></td>
		</tr>
		<tr>
			<td colspan="2" align="right"><asp:button id="login" commandname="Login" text="<%$Resources:wss,login_pagetitle%>" runat="server" /></td>
		</tr>
		<tr>
			<td colspan="2"><asp:checkbox id="RememberMe" text="<%$SPHtmlEncodedResources:wss,login_pageRememberMe%>" runat="server" /></td>
		</tr>
		</table>
	</layouttemplate>
 </asp:login>

    <br/>
    <sysweb:HyperLink ID="registerNewUser" runat="server" Font-Bold="true" Font-Underline="true" Text="<%$Resources:MyResource,RegisterNewUser%>"></sysweb:HyperLink>
    <br/>
    <br/>
    <sysweb:HyperLink ID="IforgetMyPassword" runat="server" Font-Bold="true" Font-Underline="true" Text="<%$Resources:MyResource,IForgetMyPassword%>"></sysweb:HyperLink>

</asp:Content>

