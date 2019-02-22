<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Admin.Hacks.Temp_SetOrganizationAdminAccess" Codebehind="Temp-SetOrganizationAdminAccess.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="entryFields">
        <asp:TextBox runat="server" ID="TextPeopleWriteAccessList" CssClass="alignRight" />&nbsp;<br/>
        <asp:TextBox runat="server" ID="TextPeopleReadAccessList" />&nbsp;<br/>
        <asp:Button ID="ButtonSave" runat="server" CssClass="button-accent-color" OnClick="ButtonSave_Click" Text="Save"/>
    </div>
    <div class="entryLabels">
        Read/write access, person IDs<br/>
        Read-only access, person IDs<br/>
    </div>
    <div style="clear:both"></div>
    <hr/><br/>
    
    <p><strong>People with read/write access:</strong> <asp:Label runat="server" ID="LabelPeopleWriteAccessList" /></p>
    <p><strong>People with read-only access:</strong> <asp:Label runat="server" ID="LabelPeopleReadAccessList" /></p>
    
    <br/>
    <p>List the people IDs, separated by spaces, who should have administrative read-write and read-only privileges, respectively to sensitive functionality (bookkeeping, et cetera). This applies installation-wide, to all organizations running on this Swarmops.</p>
    <p><strong>This is a temporary access mechanism</strong> to get Swarmops usage off the ground. Real, fine-grained, responsibility-based access control is scheduled to be implemented by the <em>Swarmops Orange</em> release (Jun 30, 2015).</p>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

