<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="Vote.aspx.cs" Inherits="Activizr.Pages.Governance.Vote" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">

<h2>SIMULERAT VÅRMÖTE 2011</h2>

<h3><asp:Label ID="LabelPointsOfOrderHeader" runat="server" /></h3>

<p><asp:Label ID="LabelPointsOfOrderTemp" runat="server" /></p>

<h3><asp:Label ID="LabelMotionsHeader" runat="server" /></h3>

<div style="float:left;padding-bottom:10px;width:200px"><asp:Label ID="LabelVoteAmendmentsLabel" runat="server" /><br /><asp:Label ID="LabelVoteMotionsLabel" runat="server" /></div>
<div style="float:left;padding-left:10px;padding-bottom:10px"><a href="VoteAmendments.aspx?MeetingId=1"><asp:Label ID="LabelVoteHere" runat="server" /></a><br />Öppnar 2011-04-25</div>
<div style="float:left;padding-left:10px;padding-bottom:10px">&nbsp;<br />&nbsp;</div>
<div class="break"></div>

<p><asp:LinkButton ID="LabelCreateVotingList" runat="server" /></p>

<h3><asp:Label ID="LabelElectionsHeader" runat="server" /></h3>

<div style="float:left;padding-bottom:10px;width:200px">Styrelsens sammankallande<br />Styrelsen t.o.m. 2013<br />Styrelsen t.o.m. 2012 (fyllnadsval)<br />Mötespresidium</div>
<div style="float:left;padding-left:10px;padding-bottom:10px">Öppnar 2011-05-03<br />Öppnar 2011-05-03<br />Öppnar 2011-05-03<br />Öppnar 2011-05-03</div>
<div style="float:left;padding-left:10px;padding-bottom:10px">&nbsp;<br />&nbsp;</div>
<div class="break"></div>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue"><asp:Label ID="LabelSidebarInfo" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelVotingInfo" runat="server" />
        </div>
    </div>
    
    <h2 class="blue"><asp:Label ID="LabelSidebarActions" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <div class="link-row-encaps" onclick="document.location='/Pages/v5/Governance/ListMotions.aspx';" >
                <div class="link-row-icon" style="background-image:url('/Images/PageIcons/iconshock-motions-16px.png')"></div>
                <asp:Label ID="LabelActionListMotions" runat="server" />
            </div>
        </div>
    </div>
    
    <h2 class="orange"><asp:Label ID="LabelSidebarTodo" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelActionItemsHere" runat="server" />
        </div>
    </div>  
</asp:Content>
