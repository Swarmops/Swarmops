<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonBasicDetails.ascx.cs"
    Inherits="Controls_PersonBasicDetails" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
    

<script type="text/javascript">

    function OnTextChange(control) {
        document.getElementById('<%= ButtonSaveChanges.ClientID%>').disabled = false;
        control.style.color = "#A00000";
    }

    function OnTextChangePostals() {
        document.getElementById('<%= ButtonSaveChanges.ClientID%>').disabled = false;
        document.getElementById('<%= TextPostalCode.ClientID%>').style.color = "#A00000";
        document.getElementById('<%= TextCity.ClientID%>').style.color = "#A00000";
    }

    function OnChangeCountries() {
        document.getElementById('<%= ButtonSaveChanges.ClientID%>').disabled = false;
        document.getElementById('<%= DropCountries.ClientID%>').style.color = "#A00000";
    }
    function OnChangeMunicipality() {
        document.getElementById('<%= ButtonSaveChanges.ClientID%>').disabled = false;
        document.getElementById('<%= DropDownMunicipalities.ClientID%>').style.color = "#A00000";
    }

</script>

<asp:Panel ID="PanelDetails" runat="server" DefaultButton="ButtonSaveChanges"
    meta:resourcekey="PanelDetailsResource1">
    <table id="Table" runat="server" cellspacing="0" meta:resourcekey="TableResource1">
        <tr id="RowMemberNumber" runat="server" meta:resourcekey="RowMemberNumberResource1">
            <td id="CellNameMemberNumber" runat="server" meta:resourcekey="CellNameMemberNumberResource1">
                <asp:Label ID="LabelMemberNumber" runat="server" Text="Member#"
                    meta:resourcekey="LabelMemberNumberResource1"></asp:Label>
            </td>
            <td id="CellMemberNumberText" runat="server" class="EditCell"
                meta:resourcekey="CellMemberNumberTextResource1" colspan="2">
                <asp:TextBox ID="TextMemberNumber" runat="server" Columns="10"
                    ReadOnly="True" meta:resourcekey="TextMemberNumberResource1"></asp:TextBox>
            </td>
            <td id="TableCell6" runat="server" meta:resourcekey="TableCell6Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelMemberNumberMessage" runat="server"
                    meta:resourcekey="LabelMemberNumberMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="RowName" runat="server" meta:resourcekey="RowNameResource1">
            <td id="CellNameLabel" runat="server" meta:resourcekey="CellNameLabelResource1">
                <asp:Label ID="LabelName" runat="server" Text="Name" meta:resourcekey="LabelNameResource1"></asp:Label>
            </td>
            <td id="CellNameText" runat="server" class="EditCell" meta:resourcekey="CellNameTextResource1"
                colspan="2">
                <asp:TextBox ID="TextName" runat="server" Columns="30" meta:resourcekey="TextNameResource1"></asp:TextBox>
            </td>
            <td id="TableCell1" runat="server" meta:resourcekey="TableCell1Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelNameMessage" runat="server" meta:resourcekey="LabelNameMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="RowStreet" runat="server" meta:resourcekey="RowStreetResource1">
            <td id="CellStreetLabel" runat="server" meta:resourcekey="CellStreetLabelResource1">
                <asp:Label ID="LabelStreet" runat="server" Text="Street" meta:resourcekey="LabelStreetResource1"></asp:Label>
            </td>
            <td id="CellStreetText" runat="server" class="EditCell" meta:resourcekey="CellStreetTextResource1"
                colspan="2">
                <asp:TextBox ID="TextStreet" runat="server" Columns="30" meta:resourcekey="TextStreetResource1"></asp:TextBox>
            </td>
            <td id="TableCell2" runat="server" meta:resourcekey="TableCell2Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelStreetMessage" runat="server"
                    meta:resourcekey="LabelStreetMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="RowPostal" runat="server" meta:resourcekey="RowPostalResource1">
            <td id="CellPostalLabel" runat="server" meta:resourcekey="CellPostalLabelResource1">
                <asp:Label ID="LabelPostal" runat="server" Text="Postal Address"
                    meta:resourcekey="LabelPostalResource1"></asp:Label>
            </td>
            <td id="CellPostalText" runat="server" class="EditCell" meta:resourcekey="CellPostalTextResource1">
                <asp:TextBox ID="TextPostalCode" runat="server" Columns="7" meta:resourcekey="TextPostalCodeResource1"
                    AutoPostBack="True" OnTextChanged="TextPostalCode_Change"></asp:TextBox>
                &nbsp;&nbsp;
            </td>
            <td class="EditCell" runat="server">
                <asp:TextBox ID="TextCity" runat="server" Columns="20" meta:resourcekey="TextCityResource1" ></asp:TextBox>
            </td>
            <td id="TableCell3" runat="server" meta:resourcekey="TableCell3Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelPostalMessage" runat="server"
                    meta:resourcekey="LabelPostalMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="RowCountry" runat="server" meta:resourcekey="RowCountryResource1">
            <td id="CellCountryLabel" runat="server" meta:resourcekey="CellCountryLabelResource1">
                <asp:Label ID="LabelCountry" runat="server" Text="Country" meta:resourcekey="LabelCountryResource1"></asp:Label>
            </td>
            <td id="CellCountryDrop" runat="server" class="EditCell" meta:resourcekey="CellCountryDropResource1"
                colspan="2">
                <asp:DropDownList ID="DropCountries" runat="server" meta:resourcekey="DropCountriesResource1">
                </asp:DropDownList>
            </td>
            <td id="TableCell4" runat="server" meta:resourcekey="TableCell4Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelCountryMessage" runat="server"
                    meta:resourcekey="LabelCountryMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="RowGeo" runat="server" meta:resourcekey="RowCountryResource1">
            <td id="TableCell29" runat="server" meta:resourcekey="TableCell29Resource1">
                <asp:Label ID="LabelMunicipality" runat="server" Text="Municipality" meta:resourcekey="LabelMunicipalityResource1"></asp:Label>
            </td>
            <td id="TableCell30" runat="server" class="EditCell" meta:resourcekey="TableCell30Resource1"
                colspan="2">
                <asp:DropDownList ID="DropDownMunicipalities" runat="server"
                    meta:resourcekey="DropDownMunicipalitiesResource1">
                </asp:DropDownList>
            </td>
            <td id="TableCell31" runat="server" meta:resourcekey="TableCell4Resource1">
                &nbsp;&nbsp;<asp:Label ID="Label3" runat="server" meta:resourcekey="LabelCountryMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="RowEmail" runat="server" meta:resourcekey="RowEmailResource1">
            <td id="CellEmailLabel" runat="server" meta:resourcekey="CellEmailLabelResource1">
                <asp:Label ID="LabelEmail" runat="server" Text="e-Mail" meta:resourcekey="LabelEmailResource1"></asp:Label>
            </td>
            <td id="CellEmailText" runat="server" class="EditCell" meta:resourcekey="CellEmailTextResource1"
                colspan="2">
                <asp:TextBox ID="TextEmail" runat="server" Columns="30" meta:resourcekey="TextEmailResource1"></asp:TextBox>
            </td>
            <td id="TableCell5" runat="server" meta:resourcekey="TableCell5Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelEmailMessage" runat="server"
                    meta:resourcekey="LabelEmailMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="RowPartyEmail" runat="server" meta:resourcekey="RowPartyEmailResource1">
            <td id="CellPartyEmailLabel" runat="server" meta:resourcekey="CellPartyEmailLabelResource1">
                <asp:Label ID="LabelPartyEmail" runat="server" Text="Party e-Mail"
                    meta:resourcekey="LabelPartyEmailResource1"></asp:Label>
            </td>
            <td id="CellPartyEmailText" runat="server" class="EditCell" meta:resourcekey="CellPartyEmailTextResource1"
                colspan="2">
                <asp:TextBox ID="TextPartyEmail" Enabled="False" runat="server"
                    Columns="30" meta:resourcekey="TextPartyEmailResource1"></asp:TextBox>
            </td>
            <td id="TableCell27" runat="server" meta:resourcekey="TableCell27Resource1">
                &nbsp;&nbsp;<asp:Button runat="server" ID="ButtonSendNewPassword"
                    Enabled="False" Text="Send New Password" OnClick="ButtonSendNewPassword_Click"
                    meta:resourcekey="ButtonSendNewPasswordResource1" />
                <asp:Button ID="ButtonDeleteMail" runat="server" Enabled="False" 
                    OnClick="ButtonDeleteMail_Click" 
                    OnClientClick="return (confirm('Really delete Party eMail?'));" 
                    Text="Delete partyMail" Visible="False" />
            </td>
        </tr>
        <tr id="RowPhone" runat="server" meta:resourcekey="RowPhoneResource1">
            <td id="CellPhoneLabel" runat="server" meta:resourcekey="CellPhoneLabelResource1">
                <asp:Label ID="LabelPhone" runat="server" Text="Phone" meta:resourcekey="LabelPhoneResource1"></asp:Label>
            </td>
            <td id="CellPhoneText" runat="server" class="EditCell" meta:resourcekey="CellPhoneTextResource1"
                colspan="2">
                <asp:TextBox ID="TextPhone" runat="server" Columns="30" meta:resourcekey="TextPhoneResource1"></asp:TextBox>
            </td>
            <td id="TableCell7" runat="server" meta:resourcekey="TableCell7Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelPhoneMessage" runat="server"
                    meta:resourcekey="LabelPhoneMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="TableRow1" runat="server" meta:resourcekey="TableRow1Resource1">
            <td id="TableCell8" runat="server" meta:resourcekey="TableCell8Resource1">
                <asp:Label ID="LabelBirthdate" runat="server" Text="Birth Date"
                    meta:resourcekey="LabelBirthdateResource1"></asp:Label>
            </td>
            <td id="TableCell9" runat="server" class="EditCell" meta:resourcekey="TableCell9Resource1"
                colspan="2">
                <asp:TextBox ID="TextBirthdate" runat="server" Columns="30" meta:resourcekey="TextBirthdateResource1"></asp:TextBox>
                <!-- replace with drop-down calendar -->
            </td>
            <td id="TableCell10" runat="server" meta:resourcekey="TableCell10Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelBirthdateMessage" runat="server"
                    meta:resourcekey="LabelBirthdateMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="TableRow6" runat="server" meta:resourcekey="TableRow1Resource1">
            <td id="TableCell23" runat="server" meta:resourcekey="TableCell8Resource1">
                <asp:Label ID="Label1" runat="server" Text="Biological sex" meta:resourcekey="Label1Resource1"></asp:Label>
            </td>
            <td id="TableCell26" runat="server" class="EditCell" meta:resourcekey="TableCell26Resource1"
                colspan="2">
                <asp:RadioButtonList ID="DropGenders" runat="server" RepeatDirection="Horizontal"
                    meta:resourcekey="DropGendersResource1">
                    <asp:ListItem Value="Male" meta:resourcekey="ListItemResource1" Text="Male"></asp:ListItem>
                    <asp:ListItem Value="Female" meta:resourcekey="ListItemResource2" Text="Female"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td id="TableCell28" runat="server" meta:resourcekey="TableCell10Resource1">
                &nbsp;&nbsp;<asp:Label ID="Label2" runat="server" meta:resourcekey="LabelBirthdateMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="TableRowHandle" runat="server" meta:resourcekey="TableRowHandleResource1">
            <td id="TableCellHandle" runat="server" meta:resourcekey="TableCellHandleResource1">
                <asp:Label ID="LabelHandle" runat="server" Text="Forum Handle"
                    meta:resourcekey="LabelHandleResource1"></asp:Label>
            </td>
            <td id="TableCell24" runat="server" class="EditCell" meta:resourcekey="TableCell24Resource1"
                colspan="2">
                <asp:TextBox ID="TextHandle" runat="server" Columns="30" meta:resourcekey="TextHandleResource1"></asp:TextBox>
            </td>
            <td id="TableCell25" runat="server" meta:resourcekey="TableCell25Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelHandleMessage" runat="server"  meta:resourcekey="LabelHandleMessageResource1">(The user name you have separately registered in the forum.)</asp:Label>
            </td>
        </tr>
        <tr id="TableRow2" runat="server" meta:resourcekey="TableRow2Resource1">
            <td id="TableCell11" runat="server" meta:resourcekey="TableCell11Resource1">
                <asp:Label ID="LabelPersonalNumber" runat="server" Text="Personal Number"
                    meta:resourcekey="LabelPersonalNumberResource1"></asp:Label>
            </td>
            <td id="TableCell12" runat="server" class="EditCell" meta:resourcekey="TableCell12Resource1"
                colspan="2">
                <asp:TextBox ID="TextPersonalNumber" runat="server" Columns="30"
                    meta:resourcekey="TextPersonalNumberResource1"></asp:TextBox>
            </td>
            <td id="TableCell13" runat="server" meta:resourcekey="TableCell13Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelPersonalNumberMessage" runat="server"
                    Text="(optional, SE only)" meta:resourcekey="LabelPersonalNumberMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="TableRow3" runat="server" meta:resourcekey="TableRow3Resource1">
            <td id="TableCell14" runat="server" meta:resourcekey="TableCell14Resource1">
                <asp:Label ID="LabelBankName" runat="server" Text="Bank" meta:resourcekey="LabelBankNameResource1"></asp:Label>
            </td>
            <td id="TableCell15" runat="server" class="EditCell" meta:resourcekey="TableCell15Resource1"
                colspan="2">
                <asp:TextBox ID="TextBankName" runat="server" Columns="30" ReadOnly="True"
                    meta:resourcekey="TextBankNameResource1"></asp:TextBox>
            </td>
            <td id="TableCell16" runat="server" meta:resourcekey="TableCell16Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelBankNameMessage" runat="server"
                    Text="(optional)" meta:resourcekey="LabelBankNameMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="TableRow4" runat="server" meta:resourcekey="TableRow4Resource1">
            <td id="TableCell17" runat="server" meta:resourcekey="TableCell17Resource1">
                <asp:Label ID="LabelBankClearing" runat="server" Text="Bank Clearing"
                    meta:resourcekey="LabelBankClearing"></asp:Label>
            </td>
            <td id="TableCell18" runat="server" class="EditCell" meta:resourcekey="TableCell18Resource1"
                colspan="2">
                <asp:TextBox ID="TextBankClearing" runat="server" Columns="30"
                    ReadOnly="True" meta:resourcekey="TextBankAccountResource1"></asp:TextBox>
            </td>
            <td id="TableCell19" runat="server" meta:resourcekey="TableCell19Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelBankClearingMessage" runat="server"
                    Text="(optional) (four digits)"></asp:Label>
            </td>
        </tr>
        <tr id="Tr3" runat="server" meta:resourcekey="TableRow4Resource1">
            <td id="Td5" runat="server" meta:resourcekey="TableCell17Resource1">
                <asp:Label ID="LabelBankAccount" runat="server" Text="Account#"
                    meta:resourcekey="LabelBankAccountResource1"></asp:Label>
            </td>
            <td id="Td6" runat="server" class="EditCell" meta:resourcekey="TableCell18Resource1"
                colspan="2">
                <asp:TextBox ID="TextBankAccount" runat="server" Columns="30"
                    ReadOnly="True" meta:resourcekey="TextBankAccountResource1"></asp:TextBox>
            </td>
            <td id="Td7" runat="server" meta:resourcekey="TableCell19Resource1">
                &nbsp;&nbsp;<asp:Label ID="LabelBankAccountMessage" runat="server"
                    Text="(optional) (not including clearing number)" meta:resourcekey="LabelBankAccountMessageResource1"></asp:Label>
            </td>
        </tr>
        <tr id="Tr1" runat="server" meta:resourcekey="TableRow4Resource1">
            <td id="Td1" runat="server" meta:resourcekey="TableCell17Resource1">
                <asp:Label ID="Label4" runat="server" Text="Crypto key"
                    meta:resourcekey="LabelCryptoFingerprint"></asp:Label>
            </td>
            <td id="Td2" runat="server" class="EditCell" meta:resourcekey="TableCell18Resource1"
                colspan="2">
                <asp:TextBox ID="TextCryptoFingerprint" runat="server" Columns="30"
                    ReadOnly="True" meta:resourcekey="TextBankAccountResource1"></asp:TextBox>
            </td>
        </tr>
        <tr id="Tr2" runat="server" meta:resourcekey="TableRow4Resource1">
            <td id="Td3" runat="server" meta:resourcekey="TableCell17Resource1">
                <asp:Label ID="Label5" runat="server" Text="T-Shirt Size"
                    meta:resourcekey="LabelTShirtSize"></asp:Label>
            </td>
            <td id="Td4" runat="server" class="EditCell" meta:resourcekey="TableCell18Resource1"
                colspan="2">
                <asp:TextBox ID="TextTShirtSize" runat="server" Columns="30"
                    ReadOnly="True" meta:resourcekey="TextBankAccountResource1"></asp:TextBox>
            </td>
        </tr>
        <tr id="TableRow5" runat="server" meta:resourcekey="TableRow5Resource1">
            <td id="TableCell20" runat="server" meta:resourcekey="TableCell20Resource1">
                &nbsp;
            </td>
            <td id="TableCell21" runat="server" class="EditCell" meta:resourcekey="TableCell21Resource1"
                colspan="2">
                <asp:Button ID="ButtonSaveChanges" runat="server" Text="Save"
                    OnClick="ButtonSaveChanges_Click" Enabled="False" />
            </td>
            <td id="TableCell22" runat="server" meta:resourcekey="TableCell22Resource1">
                &nbsp;&nbsp;
            </td>
        </tr>
    </table>
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="TextPostalCode">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="DropDownMunicipalities" />
                    <telerik:AjaxUpdatedControl ControlID="TextCity" />
                    <telerik:AjaxUpdatedControl ControlID="LabelPostalMessage" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>
    <asp:Label ID="bottomMessage" runat="server" meta:resourcekey="bottomMessageResource1"></asp:Label>
    <asp:Panel ID="msgAskAboutPPAddress" runat="server" Height="200px"
        Style="position: absolute; top: 125px; text-align: center;
        vertical-align: middle; left: 151px; background-color: White;"
        Width="450px" Visible="False" BorderColor="White" BorderStyle="Ridge"
        meta:resourcekey="msgAskAboutPPAddressResource1">
        <div id="divMessage" runat="server" 
            style="text-align: center;
            height: 145px; vertical-align: middle; position: relative; top: 0px; left: 0px; width: 442px;">
        </div>
        <br />
        <div style="text-align: center; vertical-align: middle; position: relative;">
            <asp:Button ID="btnAndraMailJa" runat="server" Text="Yes" Width="60px"
                OnClick="btnAndraMailJa_Click" meta:resourcekey="btnAndraMailJaResource1" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnAndraMailNej" runat="server" Text="No" Width="60px"
                OnClick="btnAndraMailNej_Click" meta:resourcekey="btnAndraMailNejResource1" />
        </div>
        <br />
    </asp:Panel>
</asp:Panel>
