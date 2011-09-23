<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="ListMotions.aspx.cs" Inherits="Activizr.Pages.Governance.ListMotions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <telerik:RadTreeList ID="GridMotions" runat="server" Width="100%" DataKeyNames="Identity" ParentDataKeyNames="ParentIdentity" AutoGenerateColumns="false" Skin="WebBlue" OnItemCreated="GridMotions_ItemCreated" OnNeedDataSource="GridMotions_NeedDataSource">
        <Columns>
            <telerik:TreeListBoundColumn HeaderText="#" DataField="Designation" UniqueName="ListMotions_Grid_Designation" HeaderStyle-Width="50px" ItemStyle-Width="50px" />
            <telerik:TreeListTemplateColumn HeaderText="Title" UniqueName="ListMotions_Grid_Title" ItemStyle-Width="300px" HeaderStyle-Width="300px">
                <ItemTemplate>
                    <asp:Literal ID="LiteralTitle" runat="server" />
                </ItemTemplate>
            </telerik:TreeListTemplateColumn>
            <telerik:TreeListTemplateColumn HeaderText="Amendments" UniqueName="ListMotions_Grid_Amendments" ItemStyle-Width="75px" HeaderStyle-Width="75px">
                <ItemTemplate>
                    <asp:Literal ID="LiteralAmendments" runat="server" />
                </ItemTemplate>
            </telerik:TreeListTemplateColumn>
            <telerik:TreeListTemplateColumn HeaderText="Amended" UniqueName="ListMotions_Grid_Amended" ItemStyle-Width="60px" HeaderStyle-Width="60px">
                <ItemTemplate>
                    &ndash;
                </ItemTemplate>
            </telerik:TreeListTemplateColumn>
            <telerik:TreeListTemplateColumn HeaderText="Carried" UniqueName="ListMotions_Grid_Carried" ItemStyle-Width="60px" HeaderStyle-Width="60px">
                <ItemTemplate>
                    &ndash;
                </ItemTemplate>
            </telerik:TreeListTemplateColumn>
        </Columns>
    </telerik:RadTreeList>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue"><asp:Label ID="LabelSidebarInfo" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelSidebarLookingAt" runat="server" /> <b><asp:Label ID="LabelMeetingName" runat="server" /></b>. <a href="#"><asp:Label ID="LabelChangeMeeting" runat="server" /></a>
        </div>
    </div>   
    
    <h2 class="blue"><asp:Label ID="LabelSidebarActions" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <div class="link-row-encaps" onclick="document.location='/Pages/v5/Governance/AddMotion.aspx';" >
                <div class="link-row-icon" style="background-image:url('/Images/PageIcons/iconshock-motion-add-16px.png')"></div>
                <asp:Label ID="LabelActionAddMotion" runat="server" />
            </div>
            <div class="link-row-encaps" onclick="document.location='/Pages/v5/Governance/Vote.aspx';" >
                <div class="link-row-icon" style="background-image:url('/Images/PageIcons/iconshock-vote-16px.png')"></div>
                <asp:Label ID="LabelActionVote" runat="server" />
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

