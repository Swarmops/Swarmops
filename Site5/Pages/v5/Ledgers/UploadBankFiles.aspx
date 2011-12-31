<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="UploadBankFiles.aspx.cs" Inherits="Activizr.Site.Pages.Ledgers.UploadBankFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

<style type="text/css">
input.FileTypeImage
{
	width:160px;
	height:70px;
	border: 1px solid #1C397E !important;
	margin-right:11px;
	margin-top:7px;
	margin-bottom:6px;
	float:left;
}

input.FileTypeImageSelected
{
    box-shadow: 0px 0px 4px 4px #FFBC37;
    -moz-box-shadow: 0px 0px 4px 4px #FFBC37;
    -webkit-box-shadow: 0px 0px 4px 4px #FFBC37;
    border: 1px solid #C78B15 !important;
}

input.UnselectedType
{
	opacity:0.2;
}

div.Invisible
{
	display:none;
	opacity:0;
}

div.Visible
{
	dispay:inline;
	opacity:1;
}

div.BankUploadInstructionsImage
{
	float:right;
}

#lean_overlay {
    position: fixed;
    z-index:10000;
    top: 0px;
    left: 0px;
    height:100%;
    width:100%;
    background: #000;
    display: none;
}

#ModalDownloadInstructions
{
	display:none;
	background:white;
	width:720px;
    position: fixed;
    z-index: 11000;
    left: 50%;
    margin-left: -360px;
    top: 160px;
    opacity: 1;
    padding: 10px;
    border-radius: 5px;
    -moz-border-radius: 5px;
    -webkit-border-radius: 5px;
    box-shadow: 0px 0px 4px rgba(0,0,0,0.7);
    -webkit-box-shadow: 0 0 4px rgba(0,0,0,0.7);
    -moz-box-shadow: 0 0px 4px rgba(0,0,0,0.7);
}
</style>

<script type="text/javascript">
    if (!(jQuery().leanModal)) {
        alert('LeanModal plug-in has not been successfully loaded!');
    }
</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">

    <script type="text/javascript">
        var uploadInProgress = 0;

        function onClientProgressUpdating(progressArea, args) {
            //alert(JSON.stringify(args._progressData));
            //alert(args._progrssData.PrimaryPercent);
            $("#ProgressBar").css('width', args._progressData.PrimaryPercent);
            progressArea.updateVerticalProgressBar(args.get_progressBarElement(), args.get_progressValue());

            if (uploadInProgress == 0) {
                uploadInProgress = 1;
                onBeginUpload();
            }

            // args.set_cancel(true);

        }
        
        function onBeginUpload() {
            $("#DivUploadProgress").css("display", "inline");
            $("#DivUploadProgress").fadeTo('slow', 1.0);
            $("#DivInstructions").animate({ height: 'toggle' }, 1000);
            $("#DivInstructions").fadeTo('slow', 0.0);
            $("#<%= this.DropAccounts.ClientID %>").attr('disabled', 'disabled');
           
        }
    </script>

    <asp:UpdatePanel ID="PanelFileTypeAccount" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <h2><asp:Label ID="LabelSelectBankUploadFilter" Text="Select Bank And Bookkeeping" runat="server"/></h2>
            <h3><asp:Label ID="LabelBank" Text="Bank" runat="server" /></h3>
            <asp:ImageButton OnClick="ButtonSebAccountFile_Click" CssClass="FileTypeImage" ID="ButtonSebAccountFile" runat="server" ImageUrl="~/Images/Ledgers/uploadbankfiles-type-seb-kontoutdrag.png"/>
            <asp:ImageButton CssClass="FileTypeImage" ID="ButtonSebPaymentFile" runat="server" ImageUrl="~/Images/Ledgers/uploadbankfiles-type-seb-bankgirofil.png"/>
            <asp:HiddenField ID="HiddenFileType" runat="server"/>
            <div style="clear:both;margin-bottom:10px"></div>
            <div id="DivSelectAccount" <asp:Literal ID="LiteralSelectAccountDivStyle" runat="server" Text="style='opacity:0;display:none;'" />>
            <h3><asp:Label ID="LabelSelectAccount" runat="server" Text="Bookkeeping Account" /></h3>
            <asp:DropDownList runat="server" ID="DropAccounts" OnSelectedIndexChanged="DropAccounts_SelectedIndexChanged" AutoPostBack="true"/>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ButtonSebAccountFile" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:Panel ID="PanelResults" Visible="false" runat="server">
        <h2><asp:Label ID="LabelProcessingResults" Text="Imported a Bank File" runat="server" /></h2>
        <p>Todo...</p>
    </asp:Panel>

    <div style="opacity:0;display:none" id="DivUploadProgress">
        <br/><h2><asp:Label ID="LabelProcessing" runat="server" Text="Processing Uploaded File..." /></h2>
    </div>

    <div style="opacity:0;display:none;padding-top:20px" id="DivInstructions">
        <asp:UpdatePanel ID="PanelInstructions" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <br/><h2><asp:Label ID="LabelUploadH2Header" Text="Upload Bank File" runat="server" /></h2>
                <a rel="leanModal" name="ModalDownloadInstructions" id="go" href="#ModalDownloadInstructions"><asp:Image ID="ImageDownloadInstructions" ImageUrl="~/Images/Ledgers/uploadbankfiles-seb-kontoutdrag-small.png" ImageAlign="Right" runat="server" /></a>
                <h3><asp:Label ID="LabelDownloadInstructions" Text="Download File From Bank" runat="server" /></h3>
                <p><asp:Literal ID="LiteralDownloadInstructions"  runat="server" Text="Fooo!!!" /><asp:Literal ID="LiteralLastAccountRecord" runat="server" /></p><p><asp:Label ID="LabelClickImage" runat="server" /></p>
                <div style="clear:both"></div>
                <h3><asp:Label ID="LabelUploadH3Header" Text="Upload File To Activizr" runat="server" /></h3>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ButtonSebAccountFile" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>

        <input type="file" id="file1" runat="server" name="file1" /> <asp:Button ID="Upload" OnClick="Submit_Click" Text="Upload" OnClientClick="onBeginUpload();" runat="server" />
    </div>

    <telerik:RadProgressManager ID="RadProgressManager1" runat="server" />
    <telerik:RadProgressArea ID="ProgressIndicator" runat="server" Width="100%" ProgressIndicators="TotalProgressBar">
        <ProgressTemplate>
            <div style="width:100%;border:1px solid #E0E0E0;border-radius:5px;box-shadow:1px 1px 3px 3px #888">
                <div id="PrimaryProgressBarInnerDiv" runat="server" style="width:1%;margin:3px;border:1px solid #88F;border-radius:2px;height:2px;background-color:#CCF"></div>
            </div>
        </ProgressTemplate>
    </telerik:RadProgressArea>

    <div id="ModalDownloadInstructions">
        <asp:UpdatePanel ID="PanelModalInstructions" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <h2><asp:Label ID="LabelModalInstructionHeader" Text="Bank Screenshot" runat="server" /></h2>
                <span style="text-align:center"><asp:Image runat="server" ID="ImageDownloadInstructionsFull" /></span><br /><br /><hr /><br />
                <asp:Literal ID="LiteralDownloadInstructionsModal" runat="server" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostbackTrigger ControlID="ButtonSebAccountFile" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <br/>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue"><asp:Label ID="LabelSidebarInfo" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelUploadBankFilesInfo" runat="server" />
        </div>
    </div>
    
    <h2 class="blue"><asp:Label ID="LabelSidebarActions" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content"><!--
            <div class="link-row-encaps" onclick="document.location='/Pages/v5/Governance/Vote.aspx';" >
                <div class="link-row-icon" style="background-image:url('/Images/PageIcons/iconshock-vote-16px.png')"></div>
                <asp:Label ID="LabelActionVote" runat="server" />
            </div>
            <div class="link-row-encaps" onclick="document.location='/Pages/v5/Governance/ListMotions.aspx';" >
                <div class="link-row-icon" style="background-image:url('/Images/PageIcons/iconshock-motions-16px.png')"></div>
                <asp:Label ID="LabelActionListMotions" runat="server" />
            </div>-->
        </div>
    </div>
    
    <h2 class="orange"><asp:Label ID="LabelSidebarTodo" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelActionItemsHere" runat="server" />
        </div>
    </div>


</asp:Content>

