<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="HandlePaymentStatus.aspx.cs" Inherits="Pages_Special_FI_HandlePaymentStatus" %>

<%@ Register Src="../../../jQuery/ServerControls/TreeDropdown.ascx" TagName="TreeDropdown"
    TagPrefix="uc1" %>
<%@ Register Src="../../../Controls/v4/v3/PersonList.ascx" TagName="PersonList" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        .floatcontainer:after
        {
            content: ".";
            display: block;
            height: 0;
            font-size: 0;
            clear: both;
            visibility: hidden;
            zoom: 1.001;
        }
        .floatcontainer
        {
            zoom: 1.001;
            display: block;
        }
        /* Hides from IE Mac */* html .floatcontainer
        {
            height: 1%;
        }
        .floatcontainer
        {
            display: block;
        }
        /* End Hack */</style>

    <script type="text/javascript">
        function selectAll(elem) {
            $('.selectCheckbox').attr('checked', elem.checked);
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent" style="position: relative; zoom: 1">
        <h3 runat="server">
            Handle Payment Status</h3>
        <div style="width: 100%; height: 82px; z-index: 5; position: relative;">
            <div style="position: relative">
                Select Org:<uc1:TreeDropdown ID="DropOrganizations" runat="server" AutoPostBack="False"
                    DropDown="True" MultiSelect="False" />
            </div>
            <br />
            <br />
        </div>
        <div class="floatcontainer" style="z-index: 4; position: relative;">
            Select Payment Status:<asp:DropDownList ID="DropDownListStatusSearch" runat="server">
            </asp:DropDownList>
            <asp:Button ID="ButtonSearch" runat="server" Text="Search" OnClick="ButtonSearch_Click" />
            <br />
            <br />
            <div style="position: relative; clear: right; float: left; top: 0px; left: 0px; width: 100%">
                <div style="position: relative; clear: right; float: right; padding-bottom: 10px; vertical-align: top;">
                    <asp:Label runat="server" Text="Change all selected to:"></asp:Label>
                    <asp:DropDownList ID="DropDownListStatusChange" runat="server">
                    </asp:DropDownList>
                    <asp:Button ID="Button2" runat="server" Text="Change" onclick="ButtonChange_Click" />
                </div>
                <div style="position: relative; clear: right; float: right; width: 100%">
                    <asp:GridView ID="GridView1" runat="server" DataSourceID="ObjectDataSource1" AutoGenerateColumns="False"
                        Width="100%" OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" 
                        AllowSorting="True" PageSize="100">
                        <Columns>
                            <asp:BoundField DataField="Identity" HeaderText="Identity" ReadOnly="True" SortExpression="Identity" />
                            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                            <asp:BoundField DataField="Street" HeaderText="Street" SortExpression="Street" />
                            <asp:BoundField DataField="PostalCodeAndCity" HeaderText="PostalCode, City" ReadOnly="True"
                                SortExpression="PostalCodeAndCity" />
                            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                            <asp:BoundField DataField="Phone" HeaderText="Phone" SortExpression="Phone" />
                            <asp:BoundField DataField="Birthdate" HeaderText="Birth date" SortExpression="Birthdate" />
                            <asp:BoundField DataField="JoinedDate" HeaderText="Joined date" SortExpression="JoinedDate" />
                            <asp:BoundField DataField="ExpiresDate" HeaderText="Expires date" SortExpression="ExpiresDate" />
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <input id="CheckboxSelectAll" type="checkbox" onclick="selectAll(this)" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <input name="CheckboxSelect" id="Checkbox1" type="checkbox" class="selectCheckbox"
                                        value='<%# Eval("MembershipId")  %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="ui-widget-header" ForeColor="White" Height="25px" 
                            VerticalAlign="Middle" />
                    </asp:GridView>
                </div>
            </div>
        </div>
        <div>
        </div>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OnObjectCreating="ObjectDataSource1_ObjectCreating"
            SelectMethod="GetData" SortParameterName="sortBy" TypeName="ListPersonDataSource"
            DataObjectTypeName="ListPerson"></asp:ObjectDataSource>
        <br />
    </div>
</asp:Content>
