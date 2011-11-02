<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="EditPerson.aspx.cs" Inherits="Pages_v4_EditPerson" Title="Untitled Page" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
<script type="text/javascript">

</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">
    <pw4:PageTitle Icon="user_edit.png" Title="Edit Person" Description="Edits all aspects of a single person in the organization" runat="server" ID="PageTitle" />
    <telerik:RadTabStrip runat="server" ID="TabStrip" SelectedIndex="0" 
        Skin="Web20" MultiPageID="MultiPagePersonDetails" >
    <Tabs>
        <telerik:RadTab Text="Basic" runat="server" >
        </telerik:RadTab>
        <telerik:RadTab Text="Account" runat="server">
        </telerik:RadTab>
        <telerik:RadTab Text="Activism" runat="server">
        </telerik:RadTab>
        <telerik:RadTab Text="Memberships" runat="server">
        </telerik:RadTab>
        <telerik:RadTab Text="Financial" runat="server">
        </telerik:RadTab>
        <telerik:RadTab Text="Roles Responsibilities" runat="server">
        </telerik:RadTab>
        <telerik:RadTab Text="Candidacies" runat="server">
        </telerik:RadTab>
        <telerik:RadTab Text="Notes" runat="server">
        </telerik:RadTab>
        <telerik:RadTab Text="Log" runat="server">
        </telerik:RadTab>
    </Tabs>
</telerik:RadTabStrip>
<div class="CellFacts" width="100%">
<telerik:RadMultiPage runat="server" ID="MultiPagePersonDetails" SelectedIndex="0">
    <telerik:RadPageView ID="PagePersonDetailsBasic" runat="server">
    <asp:UpdatePanel ID="PanelBasic" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    
        <table border="0">
        <tr><td><asp:Label ID="LabelMemberNumber" runat="server">Member #</asp:Label>&nbsp;</td>
        <td><asp:TextBox ID="TextMemberNumber" runat="server" Enabled="false" Columns="8" /></td></tr>
        <tr><td><asp:Label ID="LabelName" runat="server">Full name</asp:Label>&nbsp;</td>
        <td><telerik:RadTextBox ID="TextName" runat="server" Columns="40" AutoPostBack="True" /></td></tr>
        <tr><td><asp:Label ID="LabelCountries" runat="server">Country&nbsp;</asp:Label></td>
        <td><asp:DropDownList ID="DropCountries" runat="server" AutoPostBack="True" /></td></tr>
        <tr><td><asp:Label ID="LabelStreet" runat="server">Street</asp:Label>&nbsp;</td>
        <td><asp:TextBox ID="TextStreet" runat="server" Columns="40" /></td></tr>
        <tr><td><asp:Label ID="LabelPostalCodeCity" runat="server">Postal Code, City</asp:Label>&nbsp;</td>
        <td> 
        <asp:UpdatePanel ID="UpdatePanelCity" runat="server">
                <ContentTemplate>
                    <asp:TextBox ID="TextPostalCode" runat="server" Columns="6"
                    AutoPostBack="True" ontextchanged="TextPostalCode_TextChanged" ></asp:TextBox>&nbsp;&nbsp;
                    <asp:TextBox ID="TextCity" Enabled="false" runat="server" Columns="30" AutoPostBack="True" />
                    <asp:DropDownList ID="DropCities" runat="server" AutoPostBack="true" Visible="false" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="TextPostalCode" EventName="TextChanged" />
                    <asp:AsyncPostBackTrigger ControlID="DropCountries" EventName="SelectedIndexChanged" />
                </Triggers>
                </asp:UpdatePanel>
        </td></tr>
        <tr><td><asp:Label ID="LabelEmail" runat="server">Email</asp:Label>&nbsp;</td>
        <td><asp:TextBox ID="TextEmail" runat="server" Columns="40" /></td></tr>
        <tr><td><asp:Label ID="LabelPhone" runat="server">Phone</asp:Label>&nbsp;</td>
        <td><asp:TextBox ID="TextPhone" runat="server" Columns="40" /></td></tr>
        <tr><td><asp:Label ID="LabelBirthdate" runat="server">Date of Birth</asp:Label>&nbsp;</td>
        <td><asp:TextBox ID="TextBirthdate" runat="server" Columns="40" /></td></tr>
        <tr><td><asp:Label ID="LabelGeography" runat="server">Geography</asp:Label>&nbsp;</td>
        <td><asp:Label ID="LabelGeographyLine" runat="server" /></td></tr>
        </table>
      </ContentTemplate>
      </asp:UpdatePanel>
    </telerik:RadPageView>
    <telerik:RadPageView ID="PagePersonDetailsAccount" runat="server">
        Account
    </telerik:RadPageView>
    <telerik:RadPageView ID="PagePersonDetailsActivism" runat="server">
        Activism
    </telerik:RadPageView>
    <telerik:RadPageView ID="PagePersonDetailsMemberships" runat="server">
        Memberships
    </telerik:RadPageView>
    <telerik:RadPageView ID="RadPageView1" runat="server">
        Financial
    </telerik:RadPageView>
    <telerik:RadPageView ID="RadPageView2" runat="server">
        Roles Responsibilities
    </telerik:RadPageView>
    <telerik:RadPageView ID="RadPageView3" runat="server">
        Candidacies
    </telerik:RadPageView>
    <telerik:RadPageView ID="RadPageView4" runat="server">
        Notes
    </telerik:RadPageView>
    <telerik:RadPageView ID="RadPageView5" runat="server">
        Log
    </telerik:RadPageView>
</telerik:RadMultiPage>
</div>

</asp:Content>

