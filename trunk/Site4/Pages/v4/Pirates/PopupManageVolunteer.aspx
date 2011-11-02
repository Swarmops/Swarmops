<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="PopupManageVolunteer.aspx.cs"
    Inherits="Pages_v4_PopupManageVolunteer" meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>This title is replaced in Page_Load</title>
    <link href="/Style/PirateWeb-v4.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        function CloseAndRebind(args) {
            GetRadWindow().Close();
            GetRadWindow().BrowserWindow.refreshGrid(args);
        }

        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz as well)

            return oWindow;
        }

        function CancelEdit() {
            GetRadWindow().Close();
        }
    </script>

</head>
<body style="background-color: #f8e7ff; padding-left: 10px; padding-top: 10px; padding-right: 10px;
    padding-bottom: 10px">
    <form id="form1" runat="server">
    <div>
        <b>
            <asp:Label ID="LabelVolunteerName" runat="server" 
            meta:resourcekey="LabelVolunteerNameResource1" /></b>
        <asp:Literal ID="Literal1" runat="server" meta:resourcekey="Literal1Resource1" 
            Text=" in the geography "></asp:Literal>
        <b>
            <asp:Label ID="LabelVolunteerGeography" runat="server" 
            meta:resourcekey="LabelVolunteerGeographyResource1" /></b>
        <asp:Literal ID="Literal2" runat="server" meta:resourcekey="Literal2Resource1" 
            Text="volunteers for:"></asp:Literal>
        <br />
        <asp:CheckBoxList ID="ChecksVolunteerRoles" runat="server" 
            meta:resourcekey="ChecksVolunteerRolesResource1" />
        <hr />
        <p>
            <asp:Literal ID="Literal3" runat="server" meta:resourcekey="Literal3Resource1" 
                Text="&lt;b&gt;Call this person&lt;/b&gt; on phone &lt;b&gt;"></asp:Literal>
            <asp:Label ID="LabelVolunteerPhone" runat="server" 
                meta:resourcekey="LabelVolunteerPhoneResource1" />
            <asp:Literal ID="Literal4" runat="server" meta:resourcekey="Literal4Resource1" Text="
        &lt;/b&gt; and assess if he or she is fit for the volunteer duties. 
        &lt;b&gt;Check the roles above&lt;/b&gt; that you assigned him or her to 
        (if any; if somebody appears to not fit the role, do not assign). 
        &lt;b&gt;Describe&lt;/b&gt; your assessment below. "></asp:Literal>
            <asp:Literal ID="LiteralMemberFound" runat="server" 
                meta:resourcekey="LiteralMemberFoundResource1" /></p>
        <asp:Literal ID="LiteralFound" runat="server" Visible="False" 
            meta:resourcekey="LiteralFoundResource1" 
            Text="This member &lt;b&gt;has been located&lt;/b&gt; in the member roster. Roles you assign &lt;b&gt;WILL&lt;/b&gt; be assigned to this member automatically."></asp:Literal>
        <asp:Literal ID="LiteralNotFound" runat="server" Visible="False" 
            meta:resourcekey="LiteralNotFoundResource1" 
            Text="This member &lt;b&gt;WAS NOT&lt;/b&gt; autolocated in the member roster. Roles you assign &lt;b&gt;WILL NOT&lt;/b&gt; be auto-assigned to this volunteer; you will need to &lt;b&gt;MANUALLY LOCATE&lt;/b&gt; his or her member profile and set them."></asp:Literal>
        <asp:TextBox CssClass="FullWidth" TextMode="MultiLine" Rows="4" Columns="40" ID="TextAssessment"
            runat="server" 
            Text="Write a short assessment and your basis for decision here." 
            meta:resourcekey="TextAssessmentResource1" /><br />
        <div style="text-align: right">
            <asp:Button ID="ButtonClose" runat="server" Text="Done, close this volunteer" 
                OnClick="ButtonClose_Click" meta:resourcekey="ButtonCloseResource1" /></div>
    </div>
    </form>
</body>
</html>
