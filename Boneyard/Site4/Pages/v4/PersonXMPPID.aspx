<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="PersonXMPPID.aspx.cs" Inherits="Pages_v4_PersonXMPPID" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTreeDropDown.ascx" TagName="FinancialAccountTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        td
        {
            vertical-align: middle;
        }
    </style>

    <script type="text/javascript">
    
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <pw4:PageTitle Icon="group_key.png" Title="Handle Chat Identity" Description="Handle your identity for PP's XMPP chat server" runat="server" ID="PageTitle" />
    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Jabber Identity</span>
            <br />
            <div class="DivGroupBoxContentsNormal">
                <div>
                    <asp:Panel ID="PanelGeneralMember" runat="server">
                        <table style="width: 100%;" id="InputTable">
                            <tr class="Member">
                                <td>
                                    <asp:Literal ID="Literal5" runat="server">Enter the id you would like to use:</asp:Literal>
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBoxJabberID" runat="server"></asp:TextBox>@piratechat.net &nbsp;
                                </td>
                                <td>
                                    <asp:Panel ID="Panel1" runat="server" ForeColor="Red" Font-Bold="true">
                                        <asp:Label ID="LabelOccupied" runat="server" Visible="false">Sorry, that ID is not available.</asp:Label>
                                        <asp:Label ID="LabelChars" runat="server" Visible="false">Sorry, that ID contains illegal characters.</asp:Label>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr class="Member">
                                <td>
                                    <asp:Literal ID="Literal4" runat="server">Enter a password:</asp:Literal>
                                </td>
                                <td>
                                    <asp:TextBox ID="Password1" runat="server"></asp:TextBox>
                                </td>
                                <td rowspan="2">
                                    <asp:Panel ID="Panel2" runat="server" ForeColor="Red" Font-Bold="true">
                                        <asp:Label ID="LabelBadPw" runat="server" Visible="false">Sorry, you are not allowed to use that as a password for this.</asp:Label>
                                        <asp:Label ID="LabelEmpty" runat="server" Visible="false">Sorry, you must enter a password.</asp:Label>
                                        <asp:Label ID="LabelMatch" runat="server" Visible="false">Both must match.</asp:Label>
                                    </asp:Panel>
                                    <asp:Literal ID="Literal1" runat="server">(For security reasons, not the one you are using for PirateWeb.)</asp:Literal>
                                </td>
                            </tr>
                            <tr class="Member">
                                <td>
                                    <asp:Literal ID="Literal3" runat="server">Enter again:</asp:Literal>
                                </td>
                                <td>
                                    <asp:TextBox ID="Password2" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                                <td colspan="2">
                                    <asp:Button ID="ButtonOK" runat="server" Text="OK" Width="75px" OnClick="ButtonOK_Click" />
                                    &nbsp;
                                    <asp:Label ID="LabelError" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </div>
            </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">
                <asp:Literal ID="Literal6" runat="server">Explanantions</asp:Literal>
            </span>
            <br />
            <div class="DivGroupBoxContentsNormal">
                <asp:Literal ID="Literal2" runat="server"> 
                    This page allows you to create and handle an Identity on the Pirate Party XMPP-server,
                   xmpp.piratpartiet.se. <br />
                   <br />
                   To use the server to chat with other users, you need an identity, what is generally 
                   known as a JabberID (After the original client that used this protocol)
                   <br />
                   <br />
                   You create this identity on this page. Just enter a name you wish to use and 
                   a password for it and press "Create Account".<br />
                   <br />
                   An example on how to configure a chat client can be found <a href="http://wiki.piratpartiet.se/XMPP/Gajim ">here</a>.<br />    
                    <br />
                   Please note, if you already have a JabberID on another server, that can be used as well 
                   if you do not want to be public in the chat about being a pirate <br />
                   <br />
                   (Note: You are not allowed to use the same password as you use for PirateWeb, 
                   since that would be a security risk. Please come up with something completely different.)
 
                </asp:Literal>
            </div>
        </div>
    </div>
</asp:Content>
