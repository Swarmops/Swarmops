<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="ViewMotion.aspx.cs" Inherits="Swarmops.Pages.Governance.ViewMotion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div>
        <div class="displaylabel displayheader"><asp:Label ID="LabelMotionDesignation" runat="server" /></div><div class="displaysubstance displayheader"><b><asp:Label ID="LabelMotionTitle" runat="server" /></b></div>
        <div class="break"></div>
    </div>
    <div>
        <div class="displaylabel"><asp:Label ID="LabelMotionTextLabel" runat="server" /></div><div class="displaysubstance"><asp:Label ID="LabelMotionText" runat="server" /></div>
        <div class="break"></div>
    </div>
    <div>
        <div class="displaylabel"><asp:Label ID="LabelMotionDecisionsLabel" runat="server" /></div><div class="displaysubstance"><asp:Literal ID="LiteralMotionDecisions" runat="server" /></div>
        <div class="break"></div>
    </div>
    <div style="margin-bottom:5px">
        <div class="displaylabel"><asp:Label ID="LabelDiscussLabel" runat="server" /></div><div class="displaysubstance"><asp:HyperLink ID="LinkDiscussion" runat="server" NavigateUrl="#" /></div>
        <div class="break"></div>
    </div>
    
    <asp:Repeater ID="RepeaterAmendments" runat="server">
        <ItemTemplate>
            <hr />
                <div>
                    <div class="displaylabel"><%=GetGlobalResourceObject("Pages.Governance", "ViewMotion_Amendment").ToString() %> #<%# Eval("Designation") %></div><div class="displaysubstance"><%# Eval("Title") %></div>
                    <div class="break"></div>
                </div>
                <div>
                    <div class="displaylabel"><%=GetGlobalResourceObject("Pages.Governance", "ViewMotion_AmendmentText").ToString() %></div><div class="displaysubstance"><%# Eval("Text") %></div>
                    <div class="break"></div>
                </div>
                <div style="margin-bottom:5px">
                    <div class="displaylabel"><%=GetGlobalResourceObject("Pages.Governance", "ViewMotion_AmendmentProposedChange").ToString() %></div><div class="displaysubstance"><%# Eval("DecisionPoint") %></div>
                    <div class="break"></div>
                </div>
        </ItemTemplate>
    </asp:Repeater>

    <hr />

    <div style="margin-bottom:5px">
        <div class="displaylabel"><asp:Label ID="LabelAmendmentCount" runat="server" /></div><div class="displaysubstance"><asp:HyperLink ID="LinkAddAmendment" runat="server" NavigateUrl="#" /></div>
        <div class="break"></div>
    </div>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue"><asp:Label ID="LabelSidebarInfo" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelViewingMotionInfo" runat="server" /> <b><asp:Label ID="LabelMeetingName" runat="server" /></b>. <a href="#"><asp:Label ID="LabelWrongMeeting" runat="server" /></a>
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

