<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="PlaceInternalVote.aspx.cs" Inherits="Pages_v4_Admin_PlaceInternalVote" Title="Vote In Poll - PirateWeb" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Import Namespace="Activizr.Logic.Pirates"%>



<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

<pw4:PageTitle Icon="chart_pie.png" Title="Place internal vote" Description="Vote in an internal poll" runat="server" ID="PageTitle" />


<div class="DivMainContent">
    <asp:Panel ID="PanelPollList" runat="server" Visible="false">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Tillgängliga voteringar</span><br />
        <div class="DivGroupBoxContents">
            <p style="line-height:120%">Du har inte klickat på en direktlänk till en viss votering. Välj vilken votering du vill rösta i.</p>
            
             <telerik:RadGrid ID="GridPolls" runat="server" AllowMultiRowSelection="False" AutoGenerateColumns="False"
                    GridLines="None" OnItemCreated="GridPolls_ItemCreated">
                    <MasterTableView DataKeyNames="Identity">
                        <Columns>
                            <telerik:GridBoundColumn HeaderText="Votering" DataField="Name" meta:resourcekey="GridPollsColumnName"
                                UniqueName="ColumnName" />
                            <telerik:GridBoundColumn HeaderText="Röstning börjar" DataField="VotingOpens"  DataFormatString="{0:yyyy-MM-dd HH:mm}" meta:resourcekey="GridPollsVotingOpens"
                                UniqueName="ColumnName" />
                            <telerik:GridTemplateColumn UniqueName="Voting" HeaderText="Röstning öppen?" meta:resourcekey="GridPollsVotingOpen">
                                <ItemTemplate>
                                    <asp:Label ID="LabelVotingOpen" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn HeaderText="Röstning stänger" DataField="VotingCloses" DataFormatString="{0:yyyy-MM-dd HH:mm}" meta:resourcekey="GridPollsVotingCloses"
                                UniqueName="ColumnName" />
                            <telerik:GridTemplateColumn UniqueName="ManageColumn" meta:resourcekey="GridPollsPlaceVote">
                                <ItemTemplate>
                                    <asp:HyperLink ID="LinkVote" runat="server" Text="Rösta..." meta:resourcekey="ManageLinkResource1"></asp:HyperLink>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings>
                        <Selecting AllowRowSelect="False" />
                    </ClientSettings>
                </telerik:RadGrid>
        </div>
    </div>
    </asp:Panel>
    <asp:Panel ID="PanelPollIntro" runat="server">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Du röstar i "<asp:Label ID="LabelPollName1" runat="server" />".</span><br />
        <div class="DivGroupBoxContents">
            <p style="line-height:120%">Till vänster, så finns de tillgängliga kandidaterna (i slumpvis ordning). Du röstar genom att dra kandidater till den högra boxen, som är din röst. Den består av minst en kandidat. Om den inbördes ordningen på dina kandidater räknas, så är din förstakandidat den som du helst vill se på posten.</p>
        </div>
    </div>
    </asp:Panel>
    <asp:Panel ID="PanelVoting" runat="server">
    <div class="DivGroupBox" style="float:left">
        <span class="DivGroupBoxTitle">Candidates</span><br />
        <div class="DivGroupBoxContents">
                    <asp:UpdatePanel ID="UpdateCandidates" runat="server" UpdateMode="Conditional">
            <ContentTemplate>

            <telerik:RadListBox runat="server" ID="ListCandidates" AllowTransfer="true" 
                TransferToID="ListVote" TransferMode="Move"
            AllowReorder="false" EnableDragAndDrop="true" AllowTransferDuplicates="false" 
                Height="400px" Width="280px" ButtonSettings-Position="Right" ButtonSettings-VerticalAlign="Middle"  
                ButtonSettings-ShowTransfer="true" ButtonSettings-ShowTransferAll="false" 
 >
 
                
<ButtonSettings Position="Bottom" ShowTransfer="False" ShowTransferAll="False"></ButtonSettings>
            </telerik:RadListBox>
            
                        </ContentTemplate>
            <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ListCandidates" EventName="Transferred" />
            <asp:AsyncPostBackTrigger ControlID="ListVote" EventName="Transferred" />
            </Triggers>
            </asp:UpdatePanel>

            
        </div>
    </div>
    <div class="DivGroupBox" style="float:left">
        <span class="DivGroupBoxTitle">Your picks, in order</span><br />
        <div class="DivGroupBoxContents">
    <asp:UpdatePanel ID="UpdateVotes" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

            <telerik:RadListBox runat="server" ID="ListVote" AllowTransfer="true" 
                TransferMode="Move"
            AllowReorder="true" EnableDragAndDrop="true"
                Height="400px" Width="300px" ButtonSettings-Position="Left" ButtonSettings-VerticalAlign="Middle"
                ButtonSettings-ShowTransfer="true" ButtonSettings-ShowTransferAll="false" 
                ButtonSettings-ShowDelete="true"
                 >
            </telerik:RadListBox>
            
            <div style="text-align:right"><asp:Button ID="ButtonVote" runat="server" 
                    Text="Save vote" Enabled="true" onclick="ButtonVote_Click" /></div>

    </ContentTemplate>
    <Triggers>

    <asp:PostBackTrigger ControlID="ButtonVote" />
    </Triggers>
    </asp:UpdatePanel>

        </div>
    </div>

    
    <br style="clear:both"/>&nbsp;
    </asp:Panel>
    <asp:Panel ID="PanelComplete" runat="server" Visible="false">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Röstningen genomförd</span><br />
        <div class="DivGroupBoxContents">
            Din röst är registrerad. Tack för att du deltar i valet!
            <asp:Panel ID="PanelCode" runat="server" Visible="false"><span style="line-height:120%">Du har möjlighet att ändra din röst fram till valets slut, om du vill. Skriv upp koden <b><asp:Label ID="LabelReference" runat="server" /></b>. Just här och nu är den <b>enda</b> gången som denna kod visas; skriv upp den för senare bruk om du vill ändra din röst. Koden kommer också att listas bredvid din röst i den slutliga röstredovisningen.</span></asp:Panel>
            <asp:Panel ID="PanelEnterCode" runat="server" Visible="false">Om du vill <b>ändra</b> din röst, skriv in röstningskoden här: <asp:TextBox ID="TextVerificationCode" runat="server" /> <asp:Button ID="ButtonChangeVote" runat="server" OnClick="ButtonChangeVote_Click" Text="Ändra" /></asp:Panel>
        </div>
    </div>
    </asp:Panel>
    <asp:Panel ID="PanelNoVote" runat="server" Visible="false">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Ingen rösträtt</span><br />
        <div class="DivGroupBoxContents">
            Beklagar, men du har ingen rösträtt i denna votering.<br />
            (Röstlängden fastställdes samtidigt som omröstningen påbörjades.)
        </div>
    </div>
    </asp:Panel>
    <asp:Panel ID="PanelVotingClosed" runat="server" Visible="false">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Röstningen är inte öppen.</span><br />
        <div class="DivGroupBoxContents">
            <p style="line-height:120%">Det går inte att rösta i voteringen du valt. Det kan bero på att den inte öppnat ännu, eller på att den har stängt.</p>
        </div>
    </div>
    </asp:Panel>
</div>

</asp:Content>

