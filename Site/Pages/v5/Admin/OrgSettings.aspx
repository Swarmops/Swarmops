<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Admin.OrgSettings" Codebehind="OrgSettings.aspx.cs" %>
<%@ Import Namespace="Swarmops.Frontend" %>
<%@ Register tagPrefix="Swarmops5" tagName="FileUpload" src="~/Controls/v5/Base/FileUpload.ascx"  %>
<%@ Register tagPrefix="Swarmops5" tagName="DropDown" src="~/Controls/v5/Base/DropDown.ascx" %>
<%@ Register tagPrefix="Swarmops5" tagName="AjaxTextBox" src="~/Controls/v5/Base/AjaxTextBox.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('#divTabs').tabs();

            $('.EditCheck').switchbutton({
                checkedLabel: SwarmopsJS.unescape('<%= this.Localized_SwitchLabelOn_Upper %>'),
                uncheckedLabel: SwarmopsJS.unescape('<%= this.Localized_SwitchLabelOff_Upper %>')
            }).change(function() {

                if (suppressSwitchResponse) {
                    return;
                }

                $('#divUseAccountPlan').slideDown().fadeIn();

                var jsonData = {};
                jsonData.switchName = $(this).attr("rel");
                jsonData.switchValue = $(this).prop('checked');

                $.ajax({
                    type: "POST",
                    url: "/Pages/v5/Admin/OrgSettings.aspx/SwitchToggled",
                    data: $.toJSON(jsonData),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function(msg) {
                        if (msg.d.Success) {
                            if (msg.d.DisplayMessage.length > 2) {  // the 2 is rather arbitrary. Read as "if defined".
                                alertify.log(msg.d.DisplayMessage);
                            }
                            if (msg.d.RequireForex && !($('#CheckEnableForex').prop("checked"))) {
                                $("#CheckEnableForex").prop("checked", true).change();
                            }
                            if (jsonData.switchName == "BitcoinHot") {
                                if (jsonData.switchValue) {
                                    $('.bitcoinHotField').fadeIn();
                                } else {
                                    $('.bitcoinHotField').fadeOut();
                                }
                            }
                            if (jsonData.switchName == "Paypal") {
                                if (jsonData.switchValue) {
                                    $('.paypalAccountField').fadeIn();
                                } else {
                                    $('.paypalAccountField').fadeOut();
                                }
                            }
                        } else {
                            alertify.error(msg.d.DisplayMessage);
                            if (msg.d.RequireForex && !($('#CheckEnableForex').prop("checked"))) {
                                $("#CheckEnableForex").prop("checked", true).change();
                            }
                        }
                    },
                    error: function(msg) {
                        // TODO: Reset switch, protest server unreachable
                    }

                });
            });

            // Ask for initial data

            $.ajax({
                type: "POST",
                url: "/Pages/v5/Admin/OrgSettings.aspx/GetInitialData",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(msg) {
                    initializeSettings(msg.d);
                },
                error: function(msg) {
                    // TODO: try again? bail out?
                    alert("The server did not respond with the organization's current settings.");
                }

            });

        });

        function initializeSettings(orgSettings) {
            console.log(orgSettings);
            suppressSwitchResponse = true;
            $("#CheckEnableBitcoinCold").prop("checked", orgSettings.AccountBitcoinCold).change();
            $("#CheckEnableBitcoinHot").prop("checked", orgSettings.AccountBitcoinHot).change();
            $("#CheckEnablePaypal").prop("checked", orgSettings.AccountPaypal).change();
            $("#CheckEnableForex").prop("checked", orgSettings.AccountsForex).change();
            $("#CheckEnableVat").prop("checked", orgSettings.AccountsVat).change();
            $("#CheckParticipantFinancials").prop("checked", orgSettings.ParticipantFinancials).change();
            $("#<%=this.TextPaypalAccountAddress.ClientID%>_TextInput").val(orgSettings.PaypalAccountAddress);
            suppressSwitchResponse = false;

            if (orgSettings.AccountBitcoinHot) {
                $('.bitcoinHotField').show();
            }

            if (orgSettings.AccountPaypal) {
                $('.paypalAccountField').show();
            }

            $('#<%=this.TextGovtRegistrationId.ClientID%>_TextInput').val(orgSettings.GovernmentRegistrationId);
            $('#<%=this.DropTaxAuthority.ClientID%>').val(orgSettings.TaxAuthority);
            $('#<%=this.TextTaxPaymentOcr.ClientID%>_TextInput').val(orgSettings.TaxPaymentOcr);

            $('#<%=this.TextOpenLedgersDomain.ClientID%>_TextInput').val(orgSettings.OpenLedgersDomain);
            $('#<%=this.TextVanityDomain.ClientID%>_TextInput').val(orgSettings.VanityDomain);
        }

        var suppressSwitchResponse = false;

    </script>
        
    <style type="text/css">
        .IconEdit { cursor: pointer; }

        #IconCloseEdit { cursor: pointer; }

        .paypalAccountField { display: none; }

        .bitcoinHotField { display: none; }

        .CheckboxContainer {
            float: right;
            padding-top: 6px;
            padding-right: 8px;
        }
    </style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div id="divTabs" class="easyui-tabs" data-options="tabWidth:70,tabHeight:70">
        <div title="<img src='/Images/Icons/iconshock-switch-red-64px.png' />">
            <h2>Accounting Features</h2>
            <div class="entryFields">
                <label for="CheckEnableBitcoinCold"><asp:Literal ID="LiteralLabelBitcoinColdShort" runat="server" Text="Bitcoin Cold"/></label><div class="CheckboxContainer"><input type="checkbox" rel="BitcoinCold" class="EditCheck" id="CheckEnableBitcoinCold"/></div><br/>
                <label for="CheckEnableBitcoinHot"><asp:Literal ID="LiteralLabelBitcoinHotShort" runat="server" Text="Bitcoin Hot"/></label><div class="CheckboxContainer"><input type="checkbox" rel="BitcoinHot" class="EditCheck" id="CheckEnableBitcoinHot"/></div><br/>
                <Swarmops5:AjaxTextBox ID="TextDaysCashReserves" runat="server" CssClass="bitcoinHotField alignRight" ReadOnly="true" Placeholder="60-90" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback" Cookie="BitcoinReserves"  />
                <label for="CheckEnablePaypal"><asp:Literal ID="Literal1" runat="server" Text="Paypal"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Paypal" class="EditCheck" id="CheckEnablePaypal"/></div><br/>
                <Swarmops5:AjaxTextBox ID="TextPaypalAccountAddress" runat="server" Placeholder="paypal@example.org" CssClass="paypalAccountField" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback" Cookie="PaypalAccountAddress" />
                <label for="CheckEnableForex"><asp:Literal ID="Literal2" runat="server" Text="Forex Profit/Loss"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Forex" class="EditCheck" id="CheckEnableForex"/></div><br/>
                <label for="CheckEnableVat"><asp:Literal ID="Literal3" runat="server" Text="VAT In/Out"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Vat" class="EditCheck" id="CheckEnableVat"/></div><br/>
                <label for="CheckOpenFinancials"><asp:Literal ID="Literal4" runat="server" Text="Participant Financials"/></label><div class="CheckboxContainer"><input type="checkbox" rel="ParticipantFinancials" class="EditCheck" id="CheckOpenFinancials"/></div><br/>
            </div>
            <div class="entryLabels">
                Enable bitcoin coldwallet tracking?<br/>
                Enable bitcoin hotwallet autopay?<br/>
                <span class="bitcoinHotField">Days of coin reserves to keep hot<br/></span>
                Enable Paypal tracking and IPN?<br/>
                <span class="paypalAccountField">Paypal account mail address<br/></span>
                Enable foreign currency accounts?<br/>
                Enable Value Added Tax (VAT)?<br/>
                Enable Participant Financials?<br/>
            </div>
            <div id="divUseAccountPlan" style="display: none; width: 100%; text-align: center; margin-top: 20px; margin-bottom: 20px; border-top: 1px solid <%= CommonV5.GetColor (ColorType.Base, ColorVariant.Light) %>; border-bottom: 1px solid <%= CommonV5.GetColor (ColorType.Base, ColorVariant.Light) %>; background-color: <%= CommonV5.GetColor (ColorType.Base, ColorVariant.XLight) %>">
                Use the <a href="/Pages/v5/Ledgers/AccountPlan.aspx">Account Plan</a> page to set detailed parameters for these accounts, once enabled.
            </div>
        </div>
        <div title="<img src='/Images/Icons/iconshock-contacts-64px.png' />">
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
                <Swarmops5:DropDown ID="DropMemberNumber" runat="server" />&#8203;<br/>
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
                <asp:Label ID="LabelMemberNumber" runat="server" /><br />
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
                Logo, landscape 16x9 version [PNG]<br/>
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
        <div title="<img src='/Images/Icons/iconshock-buildings-256px.png' height='64' width='64' />">
            <h2>Regulatory stuff (optional)</h2>
            <div class="entryFields">
                <Swarmops5:AjaxTextBox runat="server" ID="TextGovtRegistrationId" Text="SE1234567890"  Cookie="GovernmentRegistrationId" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  />&#8203;<br/>
                <Swarmops5:DropDown ID="DropTaxAuthority" CssClass="DropTemp" runat="server"/>&#8203;<br/>
                <Swarmops5:AjaxTextBox runat="server" ID="TextTaxPaymentOcr" Text="1612345678900"  Cookie="TaxPaymentOcr" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  />&#8203;<br/>
            </div>
            <div class="entryLabels">
                Org registration #<br/>
                Tax authority<br/>
                Tax payment ID/OCR
            </div>
        </div>
        <div title="<img src='/Images/Icons/iconshock-star-gold-64px.png' />">
            <h2>Premium Features</h2>
            <div class="entryFields">
                <Swarmops5:AjaxTextBox runat="server" ID="TextVanityDomain" Cookie="VanityDomain" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  />&#8203;<br/>
                <Swarmops5:AjaxTextBox runat="server" ID="TextOpenLedgersDomain" Cookie="OpenLedgersDomain" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  />&#8203;<br/>
            </div>
            <div class="entryLabels">
                <asp:Label ID="LabelVanityDomain" runat="server" /><br/>
                <asp:Label ID="LabelOpenLedgersDomain" runat="server" /><br/>
            </div>

        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

