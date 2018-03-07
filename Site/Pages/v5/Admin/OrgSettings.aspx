<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Admin.OrgSettings" CodeBehind="OrgSettings.aspx.cs" CodeFile="OrgSettings.aspx.cs" %>
<%@ Import Namespace="Swarmops.Frontend" %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('#divTabs').tabs();

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

            /* Switches */

            <%=this.ToggleBitcoinCold.ClientID%>_initialize(orgSettings.Switches.AccountBitcoinCold);
            <%=this.ToggleBitcoinHot.ClientID%>_initialize(orgSettings.Switches.AccountBitcoinHot);
            <%=this.TogglePaypal.ClientID%>_initialize(orgSettings.Switches.AccountPaypal);
            <%=this.ToggleForex.ClientID%>_initialize(orgSettings.Switches.AccountsForex);
            <%=this.ToggleVat.ClientID%>_initialize(orgSettings.Switches.AccountsVat);
            <%=this.ToggleOpenFinancials.ClientID%>_initialize(orgSettings.Switches.ParticipantFinancials);

            $("#<%=this.TextPaypalAccountAddress.ClientID%>_TextInput").val(orgSettings.Switches.PaypalAccountAddress);

            if (orgSettings.Switches.AccountPaypal) {
                $('.paypalAccountField').show();
            }

            if (orgSettings.Switches.AccountsVat) {
                $('.enableVatField').show();
                <%=this.DropVatReportFrequency.ClientID%>_val(orgSettings.Switches.VatReportFrequency);
            }

            /* Participation settings */

            <%=this.ToggleAskStreet.ClientID%>_initialize(orgSettings.Participation.AskParticipantStreet);
            <%=this.DropParticipationDuration.ClientID%>_val(orgSettings.Participation.Duration);
            <%=this.DropParticipationEntry.ClientID%>_val(orgSettings.Participation.Entry);

            if (orgSettings.Participation.Entry == "ApplicationApproval") {
                $('.enableApplicationField').show();
                <%=this.TextApplicationScoreQualify.ClientID%>_initialize(orgSettings.Participation.ApplicationQualifyingScore);
            }

            if (orgSettings.Participation.Duration != 1440) {
                $('.enableExpiryRenewalField').show();
            }

            /* Messaging settings */

            <%=this.TextApplicationCompleteMail.ClientID%>_initialize(orgSettings.Messages.ApplicationCompleteMail);
            <%=this.TextParticipationAcceptedMail.ClientID%>_initialize(orgSettings.Messages.ParticipationAcceptedMail);
            <%=this.TextShortOrgInfo.ClientID%>_initialize(orgSettings.Messages.SidebarOrgInfo);
            <%=this.TextSignupFirstPage.ClientID%>_initialize(orgSettings.Messages.SignupFirstPage);
            <%=this.TextSignupLastPage.ClientID%>_initialize(orgSettings.Messages.SignupLastPage);

            /* Regulatory crap */

            $('#<%=this.TextGovtRegistrationId.ClientID%>_TextInput').val(orgSettings.GovernmentRegistrationId);
            $('#<%=this.DropTaxAuthority.ClientID%>').val(orgSettings.TaxAuthority);
            $('#<%=this.TextTaxPaymentOcr.ClientID%>_TextInput').val(orgSettings.TaxPaymentOcr);

            /* Premium stuff */

            <%=this.TextOpenLedgersDomain.ClientID%>_initialize(orgSettings.OpenLedgersDomain);
            <%=this.TextVanityDomain.ClientID%>_initialize(orgSettings.VanityDomain);
        }

        var suppressSwitchResponse = false;
        var organizationBitcoinNative = <%=this.OrganizationBitcoinNative%>;

        function onChangeBitcoinEnable(newValue, cookie) {
            if (newValue && !organizationBitcoinNative) {
                // Bitcoin has been enabled and the organization isn't bitcoin native

                <%=this.ToggleForex.ClientID%>_setValueAndCallback(true);
            }

            if (cookie == "BitcoinHot") {
                if (newValue) {
                    /*$('.bitcoinHotField').show(); save for later */ 
                } else {
                    $('.bitcoinHotField').hide();
                }
            }
        }

        function onChangePaypalEnable(newValue) {
            if (newValue) {
                $('.paypalAccountField').show();
            } else {
                $('.paypalAccountField').hide();
            }
        }


        function onChangeVatEnable(newValue) {
            if (newValue) {
                $('.enableVatField').show();
            } else {
                $('.enableVatField').hide();
            }
        }

        function onChangeApplicationEnable(newValue) {
            if (newValue != "Application") {
                $(".enableApplicationField").show();
            } else {
                $(".enableApplicationField").hide();
            }
        }

        function onChangeRenewalEnable(newValue) {
            if (newValue != 1440) {
                $(".enableExpiryRenewalField").show();
            } else {
                $(".enableExpiryRenewalField").hide();
            }
        }

        function onFileUploaded(guid, cookie) {
            var jsonData = {};
            jsonData.guid = guid;
            jsonData.cookie = cookie;

            alertify.success("onFileUploaded");

            SwarmopsJS.ajaxCall(
                '/Automation/OrgFunctions.aspx/AdminUploads',
                jsonData,
                function(data) {
                    if (data.Success) {
                        // Some special cases
                        if (cookie == "LogoLandscape") {
                            // Replace the main sidebar logo
                            $('img#ctl00_ImageLogo').attr("src", "/Pages/v5/Support/StreamUpload.aspx?DocId=" + data.ObjectIdentity);
                        }
                    } else {
                        alertify.error("The server says the file was not accepted. Try again?");
                    }
                });

        }

    </script>
        
    <style type="text/css">
        .IconEdit { cursor: pointer; }

        #IconCloseEdit { cursor: pointer; }

        .paypalAccountField { display: none; }

        .bitcoinHotField { display: none; }

        .enableVatField { display: none; }

        .enableApplicationField { display: none; }

        .enableExpiryRenewalField { display: none; }

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
                <Swarmops5:AjaxToggleSlider ID="ToggleBitcoinCold" Cookie="BitcoinCold" OnChange="onChangeBitcoinEnable" Label="Bitcoin Cold" runat="server" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/SwitchToggled"/>
                <Swarmops5:AjaxToggleSlider ID="ToggleBitcoinHot" Cookie="BitcoinHot" OnChange="onChangeBitcoinEnable" Label="Bitcoin Hot" runat="server" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/SwitchToggled"/>
                <div class="bitcoinHotField"><Swarmops5:AjaxTextBox ID="TextDaysCashReserves" runat="server" CssClass="alignRight" ReadOnly="true" Placeholder="60-90" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback" Cookie="BitcoinReserves"  /></div>
                <Swarmops5:AjaxToggleSlider ID="TogglePaypal" runat="server" Cookie="Paypal" OnChange="onChangePaypalEnable" Label="Paypal"  AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/SwitchToggled"/>
                <div class="paypalAccountField"><Swarmops5:AjaxTextBox ID="TextPaypalAccountAddress" runat="server" Placeholder="paypal@example.org" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback" Cookie="PaypalAccountAddress" /></div>
                <Swarmops5:AjaxToggleSlider ID="ToggleForex" runat="server" Cookie="Forex" Label="Forex Profit/Loss" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/SwitchToggled"/>
                <Swarmops5:AjaxToggleSlider ID="ToggleVat" Cookie="Vat" runat="server" OnChange="onChangeVatEnable" Label="VAT Reports" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/SwitchToggled"/>
                <div class="enableVatField"><Swarmops5:DropDown ID="DropVatReportFrequency" runat="server" ReadOnly="true" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback" Cookie="VatReportFrequency"  /></div>
                <Swarmops5:AjaxToggleSlider ID="ToggleOpenFinancials" Cookie="ParticipantFinancials" runat="server" Label="Participant Financials"  AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/SwitchToggled"/>
            </div>
            <div class="entryLabels">
                Enable bitcoin coldwallet tracking?<br/>
                Enable bitcoin hotwallet autopay?<br/>
                <span class="bitcoinHotField">Days of coin reserves to keep hot<br/></span>
                Enable Paypal tracking and IPN?<br/>
                <span class="paypalAccountField">Paypal account mail address<br/></span>
                Enable foreign currency accounts?<br/>
                Enable Value Added Tax (VAT)?<br/>
                <span class="enableVatField">VAT report frequency<br/></span>
                Enable Participant Financials?<br/>
            </div>
            <div id="divUseAccountPlan" style="display: none; width: 100%; text-align: center; margin-top: 20px; margin-bottom: 20px; border-top: 1px solid <%= CommonV5.GetColor (ColorType.Base, ColorVariant.Light) %>; border-bottom: 1px solid <%= CommonV5.GetColor (ColorType.Base, ColorVariant.Light) %>; background-color: <%= CommonV5.GetColor (ColorType.Base, ColorVariant.XLight) %>">
                Use the <a href="/Ledgers/AccountPlan">Account Plan</a> page to set detailed parameters for these accounts, once enabled.
            </div>
        </div>
        <div title="<img src='/Images/Icons/iconshock-contacts-64px.png' />">
            <h2>Participant policy</h2>
            <div class="entryFields">
                <Swarmops5:AjaxDropDown ID="DropParticipationEntry" CssClass="DropTemp" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback" OnClientChange="onChangeApplicationEnable" Cookie="ParticipationEntry" runat="server"/>
                <div class="enableApplicationField"><Swarmops5:AjaxTextBox ID="TextApplicationScoreQualify" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback" CssClass="alignRight" Cookie="ApplicationQualifyingScore" runat="server"/></div>
                <Swarmops5:DropDown ID="DropMembersWhere" CssClass="DropTemp" runat="server"/>
                <Swarmops5:AjaxDropDown ID="DropParticipationDuration" CssClass="DropTemp" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback" OnClientChange="onChangeRenewalEnable" Cookie="ParticipationDuration"  runat="server"/>
                <div class="enableExpiryRenewalField"><Swarmops5:DropDown ID="DropMembersChurn" CssClass="DropTemp" runat="server"/></div>
                <div class="stacked-input-control"><asp:TextBox ID="TextMembershipCost" runat="server" CssClass="alignRight" Text="0" /></div>
                <div class="enableExpiryRenewalField"><div class="stacked-input-control"><asp:TextBox ID="TextRenewalCost" runat="server" CssClass="alignRight" Text="0" /></div>
                <Swarmops5:DropDown ID="DropRenewalDateEffect" CssClass="DropTemp" runat="server"/>
                <Swarmops5:DropDown ID="DropRenewalsAffect" CssClass="DropTemp" runat="server"/>
                <Swarmops5:DropDown ID="DropRenewalReminder" CssClass="DropTemp" runat="server"/></div>
                <Swarmops5:DropDown ID="DropMemberNumber" runat="server" />
                <Swarmops5:AjaxToggleSlider ID="ToggleAskStreet" runat="server" Cookie="AskParticipantStreet" Label="Ask Street" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/SwitchToggled"/>
            </div>
            <div class="entryLabels">
                <asp:Label ID="LabelParticipationEntry" runat="server" /><br/>
                <div class="enableApplicationField">Application Score required to qualify<br/></div>
                <asp:Label ID="LabelParticipationOrg" runat="server" /><br/>
                <asp:Label ID="LabelParticipationDuration" runat="server" /><br/>
                <div class="enableExpiryRenewalField"><asp:Label ID="LabelParticipationChurn" runat="server" /><br/></div>
                <asp:Label ID="LabelParticipationCost" runat="server" /><br/>
                <div class="enableExpiryRenewalField"><asp:Label ID="LabelRenewalCost" runat="server" /><br/>
                <asp:Label ID="LabelRenewalDateEffect" runat="server" /><br/>
                <asp:Label ID="LabelRenewalsAffect" runat="server" /><br/>
                <asp:Label ID="LabelRenewalReminder" runat="server" /><br/></div>
                <asp:Label ID="LabelMemberNumber" runat="server" /><br />
                Ask for participant's street address?
            </div>
        </div>
        <div title="<img src='/Images/Icons/iconshock-mail-open-64px.png' />">
            <h2>Welcome messages (paste long messages here)</h2>
            <div class="entryFields">
                <Swarmops5:AjaxTextBox ID="TextShortOrgInfo" runat="server" Cookie="SidebarOrgInfo" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  />
                <Swarmops5:AjaxTextBox ID="TextSignupFirstPage" runat="server" Cookie="SignupFirstPage" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  />
                <Swarmops5:AjaxTextBox ID="TextSignupLastPage" runat="server" Cookie="SignupLastPage" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  />
                <div class="enableApplicationField"><Swarmops5:AjaxTextBox ID="TextApplicationCompleteMail" runat="server" Cookie="ApplicationCompleteMail" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  /></div>
                <Swarmops5:AjaxTextBox ID="TextParticipationAcceptedMail" runat="server" Cookie="ParticipationAcceptedMail" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  />
            </div>
            <div class="entryLabels">
                Sidebar short organization info<br/>
                Self-signup first page<br/>
                Self-signup last page<br/>
                <div class="enableApplicationField">Application complete mail<br/></div>
                Citizenship complete mail<br/>
            </div>
        </div>
        <div title="<img src='/Images/Icons/iconshock-colorswatch-64px.png' />">
            <h2>Communications profile and branding</h2>
            <div class="entryFields">
                <Swarmops5:FileUpload ID="UploadLogoLandscape" DisplayCount="1" runat="server" ClientUploadCompleteCallback="onFileUploaded" Cookie="LogoLandscape" />
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
        <!--
        <div title="<img src='/Images/Icons/iconshock-mail-open-64px.png' />">
            <h2>Mail transmission settings</h2>
            <div class="entryFields">
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextMailDomain" Text="example.com" /></div>
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextMailAccounting" Text="Accounting" /></div>
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextMailHR" Text="Human Resources" /></div>
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextMailOfficers" Text="Swarmops Notification" /></div>
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextSmtpServer" /></div>
            </div>
            <div class="entryLabels">
                Mail Domain (e.g. swarmops.com)<br/>
                Accounting sender<br/>
                Human Resources sender<br/>
                User Notification sender<br/>
                SMTP server (default if blank)
            </div>
        </div>-->
        <div title="<img src='/Images/Icons/iconshock-buildings-256px.png' height='64' width='64' />">
            <h2>Regulatory stuff (optional)</h2>
            <div class="entryFields">
                <Swarmops5:AjaxTextBox runat="server" ID="TextGovtRegistrationId" Text="SE1234567890"  Cookie="GovernmentRegistrationId" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  />
                <Swarmops5:DropDown ID="DropTaxAuthority" CssClass="DropTemp" runat="server"/>
                <Swarmops5:AjaxTextBox runat="server" ID="TextTaxPaymentOcr" Text="1612345678900"  Cookie="TaxPaymentOcr" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  />
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
                <Swarmops5:AjaxTextBox runat="server" ID="TextVanityDomain" Cookie="VanityDomain" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  />
                <Swarmops5:AjaxTextBox runat="server" ID="TextOpenLedgersDomain" Cookie="OpenLedgersDomain" AjaxCallbackUrl="/Pages/v5/Admin/OrgSettings.aspx/StoreCallback"  />
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

