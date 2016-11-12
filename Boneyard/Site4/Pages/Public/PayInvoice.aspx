<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4-menuless.master" AutoEventWireup="true" CodeFile="PayInvoice.aspx.cs" Inherits="Pages_Public_PayInvoice" Title="PirateWeb - view and pay invoices" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle ID="PageTitle" runat="server" Title="View/Pay Invoice" Description="View and pay invoices to the Pirate Parties" Icon="user_star.png" />

    <div class="DivMainContent">

    <asp:Panel ID="PanelPaypalReturn" runat="server" Visible="false">

    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Thank you for your payment</span><br />
    <div class="DivGroupBoxContents">
            It appears you have returned here after paying an invoice using Paypal. Thank you for your payment! Receipts will appear in your mail shortly. If you wish to pay another invoice, enter its reference number below.
    </div>
    
    </div>
    </asp:Panel>


    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Enter invoice reference</span><br />
    <div class="DivGroupBoxContents">
            <asp:Label ID="LabelInvoiceReferencePrompt" runat="server">Invoice reference number:</asp:Label>&nbsp;<asp:TextBox ID="TextInvoiceReference" runat="server" />&nbsp;<asp:Button 
                ID="ButtonLookup" runat="server" Text="Lookup / Pay" 
                onclick="ButtonLookup_Click" />&nbsp;<asp:Label ID="LabelReferenceNotFound" runat="server" />
    </div>
    
    </div>
    
    <asp:Panel ID="PanelInvoice" runat="server" Visible="false">
    
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Invoice #<asp:Label ID="LabelInvoiceReference" runat="server" /></span><br />
    <div class="DivGroupBoxContents">
    
    <div style="float:left;width:100px">Issuer:<br />Recipient:<br />Amount due:<br />Invoice Image:<br />Specification:</div>
    <asp:Label ID="LabelIssuer" runat="server" /><br />
    <asp:Label ID="LabelRecipient" runat="server" /><br />
    <asp:Label ID="LabelAmount" runat="server" /><br />
    <asp:HyperLink ID="LinkInvoiceImage" runat="server" Text="Invoice Image" Target="_blank" /><br />
    <table border="0" cellpadding="0" cellspacing="0">
    <asp:Repeater ID="RepeaterItems" runat="server">
        <ItemTemplate>
            <tr><td><%# Eval("Description") %>&nbsp;&nbsp;&nbsp;</td><td align="right"><%# ((decimal)Eval("Amount")).ToString("N2", new CultureInfo("sv-SE")) %></td></tr>
        </ItemTemplate>
    </asp:Repeater>
    </table>
    <div style="float:left;width:100px">Pay now?</div><span style="position:relative;top:2px;left:-20px"><asp:Literal ID="LiteralPaypalButton" runat="server" /></span>
    
    </div>
    
    </div>
  
    
    </asp:Panel>
    
    </div>

</asp:Content>

