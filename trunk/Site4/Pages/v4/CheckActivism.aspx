<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="CheckActivism.aspx.cs" Inherits="Pages_v4_CheckActivism" Title="Check Activism - PirateWeb" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <pw4:PageTitle Icon="bell.png" Title="Check Activist Status" Description="Check if mail belongs to activist"
            runat="server" ID="PageTitle"></pw4:PageTitle>
        <asp:Literal ID="litLeadText" runat="server" 
            meta:resourcekey="litLeadTextResource1" Text="e-mail to check: "></asp:Literal><asp:TextBox
            Columns="60" ID="TextBox1" runat="server" 
            meta:resourcekey="TextBox1Resource1"></asp:TextBox><asp:Button ID="Button1" runat="server"
                Text="Submit" />
        <asp:Label ID="litResult" runat="server" Font-Size="Large" 
            meta:resourcekey="litResultResource1"></asp:Label>
        <asp:Literal ID="litResultNotAllowed" runat="server" Visible="False" 
            meta:resourcekey="litResultNotAllowedResource1" 
            Text="You are not allowed to use this function."></asp:Literal>
        <asp:Literal ID="litResultTemplate" runat="server" Visible="False" Mode="PassThrough"
            Text="&lt;br /&gt;&lt;br /&gt;The address is used for {0} Memberships, out of wich {1} are Activists &lt;br /&gt;
      and for {2} separate Activist registration(s)
      " meta:resourcekey="litResultTemplateResource1"></asp:Literal>
    </div>
</asp:Content>
