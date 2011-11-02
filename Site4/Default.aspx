<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" 
    CodeFile="Default.aspx.cs" Inherits="_Default" Title="Welcome to PirateWeb" meta:resourcekey="PageResource1" CodePage="65001" %>
<%@ Register Src="~/Controls/v4/Financial/ListBudgets.ascx" TagName="ListBudgets" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/ListOfficerTodos.ascx" TagName="ListOfficerTodos" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/ListRecentActivism.ascx" TagName="ListRecentActivism" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/ListLocalContacts.ascx" TagName="ListLocalContacts" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>



<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server" >

    <pw4:PageTitle Icon="dashboard.png" Title="Dashboard" Description="Your personalized information and tasks summary"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
    
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
    <tr valign="top">
    <td width="50%">
     
    <asp:Panel ID="PanelBallotActivism" runat="server" Visible="false">
    
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle"><img src="Images/Public/Fugue/icons-shadowless/exclamation-diamond-frame.png" style="position:relative;top:3px" /> Valsedelsaktivism</span><br />
            <div class="DivGroupBoxContents">
               <a href="http://data.piratpartiet.se/valsedelbokning/" target="_blank" >Boka in dig för att hjälpa till med valsedelsdistributionen</a><br />
               <a href="/ValsedelAdmin/default.aspx" target="_blank" >Administration för funktionärer.</a>
               <a href="http://data.piratpartiet.se/charts/RelativeDistroStatus.aspx?nav=y" target="_blank" >Bokningsstatistik.</a>
            </div>
        </div>
    
    </asp:Panel>
   
    <asp:Panel ID="PanelTodo" runat="server">
    
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle"><img src="Images/Public/Fugue/icons-shadowless/exclamation-diamond-frame.png" style="position:relative;top:3px" /> Open Todos</span><br />
            <div class="DivGroupBoxContents">
                <pw4:ListOfficerTodos ID="ListOfficerTodos" runat="server" />
            </div>
        </div>
    
    </asp:Panel>

    <asp:Panel ID="PanelBudgets" runat="server">
    
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle"><img src="Images/Public/Fugue/icons-shadowless/moneys.png" style="position:relative;top:3px" /> Available Funds</span><br />
            <div class="DivGroupBoxContents">
                <pw4:ListBudgets ID="ListBudgets" runat="server" />
            </div>
        </div>
    
    </asp:Panel>

    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle"><img src="Images/Public/Fugue/icons-shadowless/mobile-phone.png" style="position:relative;top:3px" /> Your Local Contacts</span><br />
        <div class="DivGroupBoxContents">
            <pw4:ListLocalContacts ID="ListLocalContacts" runat="server" />
        </div>
    </div>

    
    
    </td>
    <td width="50%">
    
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle"><img src="Images/Public/Fugue/icons-shadowless/smiley-draw.png" style="position:relative;top:3px" /> Recent Activism</span><br />
        <div class="DivGroupBoxContents">
            <pw4:ListRecentActivism ID="ListRecentActivism" runat="server" />            
        </div>
    </div>
    
    </td>
    </tr>
    </table>

    </div>

</asp:Content>
