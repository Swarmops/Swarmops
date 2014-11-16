<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="EditOrganization.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Admin.EditOrganization" %>
<%@ Register tagPrefix="Swarmops5" tagName="FileUpload" src="~/Controls/v5/Base/FileUpload.ascx"  %>
<%@ Register tagPrefix="Swarmops5" tagName="DropDown" src="~/Controls/v5/Base/DropDown.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    

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
                <asp:TextBox runat="server" ID="TextDaysCashReserves" CssClass="alignRight" Text="60-90" />&#8203;<br/>
                <label for="CheckAccountPaypal"><asp:Literal ID="Literal1" runat="server" Text="Paypal"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Active" class="EditCheck" id="CheckboxEnablePaypal"/></div><br/>
                <label for="CheckAccountForeign"><asp:Literal ID="Literal2" runat="server" Text="Forex Profit/Loss"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Active" class="EditCheck" id="CheckboxEnableForeignProfitLoss"/></div><br/>
                <label for="CheckAccountVat"><asp:Literal ID="Literal3" runat="server" Text="VAT In/Out"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Active" class="EditCheck" id="CheckboxEnableVat"/></div><br/>
            </div>
            <div class="entryLabels">
                Enable bitcoin coldwallet tracking?<br/>
                Enable bitcoin hotwallet autopay?<br/>
                Days of coin reserves to keep hot<br/>
                Enable Paypal tracking and IPN?<br/>
                Enable foreign currency accounts?<br/>
                Enable Value Added Tax (VAT)?<br/>
            </div>
        </div>
        <div title="<img src='/Images/Icons/iconshock-group-diversified-64px.png' />">
            <h2>Participant policy</h2>
            <div class="entryFields">
                <Swarmops5:DropDown ID="DropMembersWhen" CssClass="DropTemp" runat="server"/>&#8203;<br/>
                <Swarmops5:DropDown ID="DropMembersWhere" CssClass="DropTemp" runat="server"/>&#8203;<br/>
                <Swarmops5:DropDown ID="DropMembershipDuration" CssClass="DropTemp" runat="server"/>&#8203;<br/>
                <Swarmops5:DropDown ID="DropMembersChurn" CssClass="DropTemp" runat="server"/>&#8203;<br/>
                <asp:TextBox ID="TextMembershipCost" runat="server" CssClass="alignRight" Text="0" />&#8203;<br/>
                <asp:TextBox ID="TextRenewalCost" runat="server" CssClass="alignRight" Text="0" />&#8203;<br/>
                <Swarmops5:DropDown ID="DropRenewalDateEffect" CssClass="DropTemp" runat="server"/>&#8203;<br/>
                <Swarmops5:DropDown ID="DropRenewalsAffect" CssClass="DropTemp" runat="server"/>&#8203;<br/>
                <Swarmops5:DropDown ID="DropRenewalReminder" CssClass="DropTemp" runat="server"/>&#8203;<br/>
            </div>
            <div class="entryLabels">
                <asp:Label ID="LabelParticipationEntry" runat="server" /><br/>
                <asp:Label ID="LabelParticipationOrg" runat="server" /><br/>
                <asp:Label ID="LabelParticipationDuration" runat="server" /><br/>
                <asp:Label ID="LabelParticipationChurn" runat="server" /><br/>
                <asp:Label ID="LabelParticipationCost" runat="server" /><br/>
                <asp:Label ID="LabelRenewalCost" runat="server" /><br/>
                <asp:Label ID="LabelRenewalDateEffect" runat="server" /><br/>
                <asp:Label ID="LabelRenewalsAffect" runat="server" /><br/>
                <asp:Label ID="LabelRenewalReminder" runat="server" /><br/>
            </div>
        </div>
        <div title="<img src='/Images/Icons/iconshock-colorswatch-64px.png' />">
            <h2>Communications profile and branding</h2>
            <div class="entryFields">
                <Swarmops5:FileUpload ID="UploadLogoLandscape" DisplayCount="1" runat="server" />
                <Swarmops5:FileUpload ID="UploadLogoSquare" DisplayCount="1" runat="server" />
                &nbsp;<br/>
                <Swarmops5:FileUpload ID="UploadInvoiceTemplate" DisplayCount="1" runat="server" />
                <Swarmops5:FileUpload ID="UploadInvoiceCover" DisplayCount="1" runat="server" />
                <Swarmops5:FileUpload ID="UploadParticipantMailTemplate" DisplayCount="1" runat="server" />
                <Swarmops5:FileUpload ID="UploadOfficerMailTemplate" DisplayCount="1" runat="server" />
                <Swarmops5:FileUpload ID="UploadRenewalMailTemplate" DisplayCount="1" runat="server" />
            </div>
            <div class="entryLabels">
                Logo, landscape version [PNG]<br/>
                Logo, square version [PNG]<br/>
                <h2>MAIL TEMPLATES</h2>
                Outbound invoice [SVG]<br/>
                Invoice cover letter [TXT]<br/>
                Mail to participants [HTML]<br/>
                Mail to officers [HTML]<br/>
                Renewal letter [HTML]<br/>
            </div>
        </div>
        <div title="<img src='/Images/Icons/iconshock-mail-open-64px.png' />">
            <h2>Mail transmission settings</h2>
            <div class="entryFields">
                <asp:TextBox runat="server" ID="TextMailDomain" Text="example.com" />&#8203;<br/>
                <asp:TextBox runat="server" ID="TextMailAccounting" Text="Accounting" />&#8203;<br/>
                <asp:TextBox runat="server" ID="TextMailHR" Text="Human Resources" />&#8203;<br/>
                <asp:TextBox runat="server" ID="TextMailOfficers" Text="Swarmops Notification" />&#8203;<br/>
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

