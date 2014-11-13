<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="EditOrganization.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Admin.EditOrganization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script src="/Scripts/jquery.switchButton.js" language="javascript" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="/Style/jquery.switchButton.css" />

    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('#divTabs').tabs();
            $('.EditCheck').switchbutton({
                checkedLabel: '<%=Resources.Global.Global_On.ToUpperInvariant() %>',
                uncheckedLabel: '<%=Resources.Global.Global_Off.ToUpperInvariant() %>',
            });

        });
    </script>
        
    <style>
	    .IconEdit {
		    cursor: pointer;
	    }
	    #IconCloseEdit {
		    cursor: pointer;
		    
	    }
	    .CheckboxContainer {
		    float: right; padding-top: 6px;padding-right: 8px;
	    }
    </style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div id="divTabs" class="easyui-tabs" data-options="tabWidth:70,tabHeight:70">
        <div title="<img src='/Images/Icons/iconshock-switch-red-64px.png' />">
            <h2>Accounting Features</h2>
            <div class="entryFields">
                <label for="CheckAccountBitcoinCold"><asp:Literal ID="LiteralLabelBitcoinColdShort" runat="server" Text="Bitcoin Cold"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Active" class="EditCheck" id="CheckEnableBitcoinCold"/></div><br/>
                <label for="CheckAccountBitcoinHot"><asp:Literal ID="LiteralLabelBitcoinHotShort" runat="server" Text="Bitcoin Hot"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Active" class="EditCheck" id="CheckboxEnableBitcoinHot"/></div><br/>
                <asp:TextBox runat="server" ID="TextDaysCashReserves" />&#8203;<br/>
                <label for="CheckAccountPaypal"><asp:Literal ID="Literal1" runat="server" Text="Paypal"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Active" class="EditCheck" id="CheckboxEnablePaypal"/></div><br/>
                <label for="CheckAccountForeign"><asp:Literal ID="Literal2" runat="server" Text="Forex Profit/Loss"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Active" class="EditCheck" id="CheckboxEnableForeignProfitLoss"/></div><br/>
            </div>
            <div class="entryLabels">
                Enable bitcoin coldwallet tracking?<br/>
                Enable bitcoin hotwallet autopay?<br/>
                Days of coin reserves to keep hot<br/>
                Enable Paypal tracking and IPN?<br/>
                Enable foreign currency accounts?<br/>
            </div>
        </div>
        <div title="<img src='/Images/Icons/iconshock-group-diversified-64px.png' />">
            <h2>Members policy</h2>
            work in progress
        </div>
        <div title="<img src='/Images/Icons/iconshock-colorswatch-64px.png' />">
            <h2>Communications profile and branding</h2>
            work in progress
        </div>
        <div title="<img src='/Images/Icons/iconshock-mail-open-64px.png' />">
            <h2>Mail transmission settings</h2>
            <div class="entryFields">
                <asp:TextBox runat="server" ID="TextMailDomain" />&#8203;<br/>
                <asp:TextBox runat="server" ID="TextMailAccounting" />&#8203;<br/>
                <asp:TextBox runat="server" ID="TextMailHR" />&#8203;<br/>
                <asp:TextBox runat="server" ID="TextMailOfficers" />&#8203;<br/>
                <asp:TextBox runat="server" ID="TextSmtpServer" />&#8203;<br/>
            </div>
            <div class="entryLabels">
                Mail Domain (e.g. swarmops.com)<br/>
                Accounting sender<br/>
                Human Resources sender<br/>
                User Notification sender<br/>
                SMTP server (default if blank)
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

