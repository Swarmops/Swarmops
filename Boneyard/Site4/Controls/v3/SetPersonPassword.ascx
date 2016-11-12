<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SetPersonPassword.ascx.cs"
    Inherits="Controls_SetPersonPassword" %>
<asp:Panel ID="PasswordControl" runat="server" meta:resourcekey="PasswordControlResource1">
    <asp:Table ID="Table" runat="server" CellSpacing="0" meta:resourcekey="TableResource1">
        <asp:TableRow ID="TableRow2" runat="server" meta:resourcekey="TableRow2Resource1">
            <asp:TableCell ID="TableCell9" runat="server" meta:resourcekey="TableCell9Resource1">
            &nbsp;&nbsp;
            </asp:TableCell>
            <asp:TableCell ID="TableCell10" runat="server" CssClass="EditCell" meta:resourcekey="TableCell10Resource1">
                <b>
                    <asp:Literal ID="SetNewPasswordText" runat="server" meta:resourcekey="SetNewPasswordTextResource1"
                        Text="Set New Password"></asp:Literal>
                </b>
            </asp:TableCell>
            <asp:TableCell ID="TableCell11" runat="server" meta:resourcekey="TableCell11Resource1">
            &nbsp;&nbsp;
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow3" runat="server" meta:resourcekey="RowPassword1Resource1">
            <asp:TableCell ID="TableCell12" runat="server" meta:resourcekey="CellNamePassword1Resource1">
                <asp:Label ID="Label1" runat="server" Text="Current Password:" meta:resourcekey="LabelOldPassword"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell13" runat="server" CssClass="EditCell" meta:resourcekey="CellPassword1TextResource1">
                <asp:TextBox ID="TextOldPassword" runat="server" Columns="30" TextMode="Password"
                    meta:resourcekey="TextPassword1Resource1"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell ID="TableCell14" runat="server" meta:resourcekey="TableCell6Resource1">
                &nbsp;&nbsp;<asp:CustomValidator ValidationGroup="SetPassword" ID="RequiredFieldValidator3"
                    CssClass="ErrorMessage" runat="server" ControlToValidate="TextPassword1" Text="Wrong current password"
                    SetFocusOnError="True" meta:resourcekey="RequiredFieldValidator2Resource1" />
            </asp:TableCell></asp:TableRow>
        <asp:TableRow runat="server" meta:resourcekey="TableRowResource1">
            <asp:TableCell runat="server" meta:resourcekey="TableCellResource1">&nbsp;</asp:TableCell></asp:TableRow>
        <asp:TableRow ID="RowPassword1" runat="server" meta:resourcekey="RowPassword1Resource1">
            <asp:TableCell ID="CellNamePassword1" runat="server" meta:resourcekey="CellNamePassword1Resource1">
                <asp:Label ID="LabelPassword1" runat="server" Text="New password:" meta:resourcekey="LabelPassword1Resource1"></asp:Label>
            </asp:TableCell><asp:TableCell ID="CellPassword1Text" runat="server" CssClass="EditCell"
                meta:resourcekey="CellPassword1TextResource1">
                <asp:TextBox ID="TextPassword1" runat="server" Columns="30" TextMode="Password" meta:resourcekey="TextPassword1Resource1"></asp:TextBox>
            </asp:TableCell><asp:TableCell ID="TableCell6" runat="server" meta:resourcekey="TableCell6Resource1">
                &nbsp;&nbsp;<asp:RequiredFieldValidator ValidationGroup="SetPassword" ID="RequiredFieldValidator2"
                    CssClass="ErrorMessage" runat="server" ControlToValidate="TextPassword1" Text="Required"
                    SetFocusOnError="True" meta:resourcekey="RequiredFieldValidator2Resource1" />
            </asp:TableCell></asp:TableRow>
        <asp:TableRow ID="RowPassword2" runat="server" meta:resourcekey="RowPassword2Resource1">
            <asp:TableCell ID="CellPassword2Label" runat="server" meta:resourcekey="CellPassword2LabelResource1">
                <asp:Label ID="LabelPassword2" runat="server" Text="repeat new:" meta:resourcekey="LabelPassword2Resource1"></asp:Label>
            </asp:TableCell><asp:TableCell ID="CellPassword2Text" runat="server" CssClass="EditCell"
                meta:resourcekey="CellPassword2TextResource1">
                <asp:TextBox ID="TextPassword2" runat="server" Columns="30" TextMode="Password" meta:resourcekey="TextPassword2Resource1"></asp:TextBox>
            </asp:TableCell><asp:TableCell ID="TableCell2" runat="server" meta:resourcekey="TableCell2Resource1">
                &nbsp;&nbsp;<asp:RequiredFieldValidator ValidationGroup="SetPassword" ID="RequiredFieldValidator1"
                    CssClass="ErrorMessage" runat="server" ControlToValidate="TextPassword2" Text="Required"
                    SetFocusOnError="True" meta:resourcekey="RequiredFieldValidator1Resource1" />
                <asp:CompareValidator ControlToValidate="TextPassword2" ControlToCompare="TextPassword1"
                    Text="Passwords must match!" ValidationGroup="SetPassword" CssClass="ErrorMessage"
                    SetFocusOnError="True" runat="server" meta:resourcekey="CompareValidatorResource1" />
            </asp:TableCell></asp:TableRow>
        <asp:TableRow ID="TableRow1" runat="server" meta:resourcekey="TableRow1Resource1">
            <asp:TableCell ID="TableCell1" runat="server" meta:resourcekey="TableCell1Resource1">
            &nbsp;
            </asp:TableCell><asp:TableCell ID="TableCell3" runat="server" CssClass="EditCell"
                meta:resourcekey="TableCell3Resource1">
                <asp:Button ID="ButtonSetPassword" runat="server" ValidationGroup="SetPassword" Text="Set password"
                    OnClick="ButtonSetPassword_Click" meta:resourcekey="ButtonSetPasswordResource1" />
            </asp:TableCell><asp:TableCell ID="TableCell4" runat="server" meta:resourcekey="TableCell4Resource1">
            &nbsp;
            </asp:TableCell></asp:TableRow>
        <asp:TableRow ID="TableRowRandomize" runat="server" meta:resourcekey="TableRowRandomizeResource1">
            <asp:TableCell ID="TableCell5" runat="server" meta:resourcekey="TableCell5Resource1">
            &nbsp;
            </asp:TableCell><asp:TableCell ID="TableCell7" runat="server" CssClass="EditCell"
                meta:resourcekey="TableCell7Resource1">
                <asp:Button ID="ButtonRandomizePassword" runat="server" Text="Generate random password"
                    ValidationGroup="None" OnClick="ButtonRandomizePassword_Click" meta:resourcekey="ButtonRandomizePasswordResource1" />
            </asp:TableCell><asp:TableCell ID="TableCell8" runat="server" meta:resourcekey="TableCell8Resource1">
            &nbsp;
            </asp:TableCell></asp:TableRow>
    </asp:Table>
</asp:Panel>
