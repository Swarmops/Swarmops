<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="VoteAmendments.aspx.cs" Inherits="Activizr.Pages.Governance.VoteAmendments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">

    <div style="float:right"><asp:DropDownList ID="DropRecommendations" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropRecommendations_SelectedIndexChanged" /></div><h2><asp:Label ID="LabelAmendmentsForMeeting" runat="server" /></h2>

    <telerik:RadTreeList ID="GridAmendmentsVote" runat="server" Width="100%" DataKeyNames="Identity" ParentDataKeyNames="ParentIdentity" AutoGenerateColumns="false" Skin="WebBlue" OnItemCreated="GridAmendmentsVote_ItemCreated" OnNeedDataSource="GridAmendments_NeedDataSource">
        <Columns>
            <telerik:TreeListBoundColumn HeaderText="#" DataField="Designation" UniqueName="ListMotions_Grid_Designation" HeaderStyle-Width="70px" ItemStyle-Width="70px" />
            <telerik:TreeListBoundColumn HeaderText="Title / Text" UniqueName="VoteAmendments_Grid_TitleText" ItemStyle-Width="400px" HeaderStyle-Width="400px" DataField="Text" />
            <telerik:TreeListTemplateColumn HeaderText="Yes" UniqueName="VoteAmendments_Grid_Yes" ItemStyle-Width="30px" HeaderStyle-Width="30px" ItemStyle-BackColor="#CCFFCC" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" >
                <ItemTemplate>
                    <asp:RadioButton ID="RadioYes" runat="server" />
                </ItemTemplate>
            </telerik:TreeListTemplateColumn>
            <telerik:TreeListTemplateColumn HeaderText="No" UniqueName="VoteAmendments_Grid_No" ItemStyle-Width="30px" HeaderStyle-Width="30px" ItemStyle-BackColor="#FFD8D8" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:RadioButton ID="RadioNo" runat="server" />
                </ItemTemplate>
            </telerik:TreeListTemplateColumn>
            <telerik:TreeListTemplateColumn HeaderText="Abstain" UniqueName="VoteAmendments_Grid_Abstain" ItemStyle-Width="30px" HeaderStyle-Width="30px" ItemStyle-BackColor="#F0F0DD" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:RadioButton ID="RadioAbstain" runat="server" />
                </ItemTemplate>
            </telerik:TreeListTemplateColumn>
            <telerik:TreeListTemplateColumn HeaderText="Recommend" UniqueName="VoteAmendments_Grid_Recommend" ItemStyle-Width="50px" HeaderStyle-Width="50px">
                <ItemTemplate>
                    <asp:Label ID="LabelRecommendation" runat="server" />
                </ItemTemplate>
            </telerik:TreeListTemplateColumn>
        </Columns>
    </telerik:RadTreeList>

    <div style="float:right;margin-top:5px;margin-bottom:3px"><asp:Button ID="ButtonSaveVote" runat="server" Enabled="false" /></div>
    <div class="break"></div>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue"><asp:Label ID="LabelSidebarInfo" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelInfo" runat="server" />
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

