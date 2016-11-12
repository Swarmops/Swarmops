<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="Add.aspx.cs" Inherits="Pages_Members_Add" Title="PirateWeb - Add Member" meta:resourcekey="PageResource1" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <pw4:PageTitle Icon="user_edit.png" Title="Add member" Description="Add a new member"
            runat="server" ID="PageTitle" meta:resourcekey="PageTitle"/>
        <asp:Panel runat="server" ID="WhateverPanel" DefaultButton="ButtonAdd" 
            meta:resourcekey="WhateverPanelResource1">
    <table cellpadding="0" cellspacing="0">
    <tr>
    <td>
        <asp:Label ID="LabelOrganization" runat="server" Text="Organisation" 
            meta:resourcekey="LabelOrganizationResource1"></asp:Label></td>
        <td class="EditCell">
            <asp:DropDownList ID="DropOrganizations" runat="server" AutoPostBack="True" 
                OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged" 
                meta:resourcekey="DropOrganizationsResource1">
            </asp:DropDownList>
        </td>
        <td></td>
    </tr>
    <tr>
        <td><asp:Label ID="LabelName" runat="server" Text="Name" 
                meta:resourcekey="LabelNameResource1"></asp:Label></td>
        <td class="EditCell"><asp:TextBox ID="TextName" Columns="25" runat="server" 
                meta:resourcekey="TextNameResource1"></asp:TextBox></td>
        <td>&nbsp;&nbsp;
            <asp:Label ID="LabelNameMessage" runat="server" 
                meta:resourcekey="LabelNameMessageResource1"></asp:Label></td>
    </tr>
    <tr>
      <td><asp:Label ID="LabelStreet" runat="server" Text="Street" 
              meta:resourcekey="LabelStreetResource1"></asp:Label></td>
      <td class="EditCell"><asp:TextBox ID="TextStreet" Columns="25" runat="server" 
              meta:resourcekey="TextStreetResource1"></asp:TextBox></td>
      <td>&nbsp;&nbsp;
          <asp:Label ID="LabelStreetMessage" runat="server" 
              meta:resourcekey="LabelStreetMessageResource1"></asp:Label></td>
    </tr>
    <tr>
        <td><asp:Label ID="LabelPostal" runat="server" Text="Postal Code, City" 
                meta:resourcekey="LabelPostalResource1"></asp:Label></td>
        <td class="EditCell"><asp:TextBox ID="TextPostalCode" Columns="4" runat="server" 
                meta:resourcekey="TextPostalCodeResource1"></asp:TextBox>&nbsp;<asp:TextBox 
                ID="TextCity" Columns="16" runat="server" meta:resourcekey="TextCityResource1"></asp:TextBox></td>
        <td>&nbsp;&nbsp;
            <asp:Label ID="LabelPostalMessage" runat="server" 
                meta:resourcekey="LabelPostalMessageResource1"></asp:Label></td>
    </tr>
    <tr>
        <td style="height: 32px"><asp:Label ID="LabelCountry" runat="server" Text="Country" 
                meta:resourcekey="LabelCountryResource1"></asp:Label></td>
        <td class="EditCell" style="height: 32px">
            <asp:DropDownList ID="DropCountries" runat="server" 
                meta:resourcekey="DropCountriesResource1">
            </asp:DropDownList>
        </td>
        <td></td>
    </tr>
    <tr>
        <td><asp:Label ID="LabelEmail" runat="server" Text="e-mail" 
                meta:resourcekey="LabelEmailResource1"></asp:Label></td>
        <td class="EditCell"><asp:TextBox ID="TextEmail" Columns="25" runat="server" 
                meta:resourcekey="TextEmailResource1"></asp:TextBox></td>
        <td>&nbsp;&nbsp;
            <asp:Label ID="LabelEmailMessage" runat="server" 
                meta:resourcekey="LabelEmailMessageResource1"></asp:Label></td>
    </tr>
    <tr>
        <td><asp:Label ID="LabelPhone" runat="server" Text="Phone" 
                meta:resourcekey="LabelPhoneResource1"></asp:Label></td>
        <td class="EditCell"><asp:TextBox ID="TextPhone" Columns="25" runat="server" 
                meta:resourcekey="TextPhoneResource1"></asp:TextBox></td>
        <td>&nbsp;&nbsp;
            <asp:Label ID="LabelPhoneMessage" runat="server" 
                meta:resourcekey="LabelPhoneMessageResource1"></asp:Label></td>
    </tr>
    <tr>
        <td><asp:Label ID="LabelBirthdate" runat="server" Text="Birthdate" 
                meta:resourcekey="LabelBirthdateResource1"></asp:Label></td>
        <td class="EditCell"><asp:TextBox ID="TextBirthDate" Columns="25" runat="server" 
                meta:resourcekey="TextBirthDateResource1"></asp:TextBox></td>
        <td>&nbsp;&nbsp;
            <asp:Label ID="LabelBirthDateMessage" runat="server" Text="Like 1972-Jan-21" 
                meta:resourcekey="LabelBirthDateMessageResource1"></asp:Label></td>
    </tr>
    <tr>
        <td><asp:Label ID="LabelGender" runat="server" Text="Gender" 
                meta:resourcekey="LabelGenderResource1"></asp:Label></td>
        <td class="EditCell">
            <asp:DropDownList ID="DropGender" runat="server" 
                meta:resourcekey="DropGenderResource1">
                <asp:ListItem Selected="True" Value="Unknown" 
                    meta:resourcekey="ListItemResource1">-- Select One --</asp:ListItem>
                <asp:ListItem meta:resourcekey="ListItemResource2">Male</asp:ListItem>
                <asp:ListItem meta:resourcekey="ListItemResource3">Female</asp:ListItem>
            </asp:DropDownList></td>
        <td>&nbsp;&nbsp;
            <asp:Label ID="LabelGenderMessage" runat="server" 
                meta:resourcekey="LabelGenderMessageResource1"></asp:Label></td>
    </tr>
    <tr>
        <td><asp:Label ID="LabelPersonalNumber" runat="server" Text="Personal #" 
                meta:resourcekey="LabelPersonalNumberResource1"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
        <td class="EditCell"><asp:TextBox ID="TextPersonalNumber" Columns="25" 
                runat="server" meta:resourcekey="TextPersonalNumberResource1"></asp:TextBox></td>
        <td>&nbsp;&nbsp;
            <asp:Label ID="LabelPersonalNumberMessage" runat="server" 
                meta:resourcekey="LabelPersonalNumberMessageResource1"></asp:Label></td>
    </tr>
    <tr>
        <td><asp:Label ID="LabelBank" runat="server" Text="Bank" 
                meta:resourcekey="LabelBankResource1"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
        <td class="EditCell"><asp:TextBox ID="TextBank" Columns="25" runat="server" 
                meta:resourcekey="TextBankResource1"></asp:TextBox></td>
        <td>&nbsp;&nbsp;
            <asp:Label ID="LabelBankMessage" runat="server" Text="(optional)" 
                meta:resourcekey="LabelBankMessageResource1"></asp:Label></td>
    </tr>
    <tr>
        <td><asp:Label ID="LabelBankAccount" runat="server" Text="Bank account" 
                meta:resourcekey="LabelBankAccountResource1"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
        <td class="EditCell"><asp:TextBox ID="TextBankAccount" Columns="25" runat="server" 
                meta:resourcekey="TextBankAccountResource1"></asp:TextBox></td>
        <td>&nbsp;&nbsp;
            <asp:Label ID="LabelBankAccountMessage" runat="server" Text="(optional)" 
                meta:resourcekey="LabelBankAccountMessageResource1"></asp:Label></td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td class="EditCell">&nbsp;<asp:Button ID="ButtonAdd" runat="server" 
                Text="Add member" OnClick="ButtonAdd_Click" 
                meta:resourcekey="ButtonAddResource1" /></td>
        <td>&nbsp;</td>
    </tr>
    </table>
    </asp:Panel>
    </div>
</asp:Content>

