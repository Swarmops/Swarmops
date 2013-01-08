<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="UploadBankFiles.aspx.cs" Inherits="Swarmops.Site.Pages.Ledgers.UploadBankFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

<style type="text/css">
input.FileTypeImage
{
	width:90px;
	height:45px;
	border: 1px solid #1C397E !important;
	margin-right:20px;
	margin-top:7px;
	margin-bottom:6px;
	float:left;
}

input.FileTypeImageSelected
{
    box-shadow: 0px 0px 2px 2px #FFBC37;
    -moz-box-shadow: 0px 0px 2px 2px #FFBC37;
    -webkit-box-shadow: 0px 0px 2px 2px #FFBC37;
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
            $("#DivInstructions").css("display", "inline");
            $("#DivInstructions").animate({ "height": "0", "opacity": "0.0" }, 1000, function () { $("#DivInstructions").css('display', 'none'); });
            $("#<%= this.DropAccounts.ClientID %>").attr('disabled', 'disabled');
           
        }
    </script>

    <asp:UpdatePanel ID="PanelFileTypeAccount" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <h2><asp:Label ID="LabelSelectBankAndAccount" Text="Select Bank And Bookkeeping LOC" runat="server"/></h2>
            <h3><asp:Label ID="LabelSelectFileType" Text="Bank LOC" runat="server" /></h3>
            <asp:ImageButton OnClick="ButtonBankgiroSEFile_Click" CssClass="FileTypeImage" ID="ButtonBankgiroSEFile" runat="server" ImageUrl="~/Images/Ledgers/uploadbankfiles-type-bankgirose.png"/>
            <asp:ImageButton OnClick="ButtonPaypalFile_Click" CssClass="FileTypeImage" ID="ButtonPaypalFile" runat="server" ImageUrl="~/Images/Ledgers/uploadbankfiles-type-paypal.png"/>
            <asp:ImageButton OnClick="ButtonPaysonFile_Click" CssClass="FileTypeImage" ID="ButtonPaysonFile" runat="server" ImageUrl="~/Images/Ledgers/uploadbankfiles-type-payson.png"/>
            <asp:ImageButton OnClick="ButtonSebAccountFile_Click" CssClass="FileTypeImage" ID="ButtonSebFile" runat="server" ImageUrl="~/Images/Ledgers/uploadbankfiles-type-seb.png"/>
            <asp:HiddenField ID="HiddenFileType" runat="server"/>
            <div style="clear:both;margin-bottom:10px"></div>
            <div id="DivSelectAccount" <asp:Literal ID="LiteralSelectAccountDivStyle" runat="server" Text="style='opacity:0;display:none'" />>
                <h3 style="padding-top:8px"><asp:Label ID="LabelSelectAccount" runat="server" Text="Bookkeeping Account LOC" /></h3>
                <asp:DropDownList runat="server" ID="DropAccounts" OnSelectedIndexChanged="DropAccounts_SelectedIndexChanged" AutoPostBack="true"/>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ButtonSebFile" EventName="Click" />
            <asp:AsyncPostbackTrigger ControlID="ButtonBankgiroSEFile" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ButtonPaypalFile" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ButtonPaysonFile" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:Panel ID="PanelResults" Visible="false" runat="server">
        <h2><asp:Label ID="LabelImportResultsHeader" Text="Imported a Bank File LOC" runat="server" /></h2>
        <asp:Panel ID="PanelErrorImage" Visible="false" runat="server">
            <asp:Image ImageUrl="~/Images/Icons/iconshock-cross-96px.png" ImageAlign="Left" runat="server" />
        </asp:Panel>
        <asp:Literal ID="LiteralImportResults" Text="Import Results [LOC]" runat="server" />
        <p><asp:HyperLink runat="server" NavigateUrl="UploadBankFiles.aspx" ID="LinkUploadAnother" Text="Upload Another? [LOC]" />
    </asp:Panel>

    <div style="opacity:0;display:none" id="DivUploadProgress">
        <br/><h2><asp:Label ID="LabelProcessing" runat="server" Text="Processing Uploaded File... LOC" /></h2>
    </div>

    <div id="DivInstructions" <asp:Literal ID="LiteralDivInstructionsStyle" runat="server" Text="style='display:none'" />>
        <asp:UpdatePanel ID="PanelInstructions" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <br/><h2><asp:Label ID="LabelUploadH2Header" Text="Upload Bank File LOC" runat="server" /></h2>
                <div style="float:right;padding-left:10px;text-align:center;padding-top:5px"><a rel="leanModal" name="ModalDownloadInstructions" id="go" href="#ModalDownloadInstructions"><asp:Image ID="ImageDownloadInstructions" ImageUrl="~/Images/Ledgers/uploadbankfiles-seb-kontoutdrag-small.png" ImageAlign="Right" runat="server" /></a><small><br style="padding-bottom:2px"/><em><asp:Label ID="LabelClickImage" runat="server" /></em></small></div>
                <h3><asp:Label ID="LabelDownloadInstructions" Text="Download File From Bank LOC" runat="server" /></h3>
                <p><asp:Literal ID="LiteralDownloadInstructions"  runat="server" Text="LOC" /><asp:Literal ID="LiteralLastAccountRecord" runat="server" /></p>
                <div style="clear:both"></div>
                <h3><asp:Label ID="LabelUploadH3Header" Text="Upload File To Activizr LOC" runat="server" /></h3>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ButtonSebFile" EventName="Click" />
                <asp:AsyncPostbackTrigger ControlID="ButtonBankgiroSEFile" EventName="Click" />
                <asp:AsyncPostbackTrigger ControlID="DropAccounts" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="ButtonPaypalFile" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="ButtonPaysonFile" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>

        <input type="file" id="FileSelector" runat="server" name="file1" /> <asp:Button ID="Upload" OnClick="Submit_Click" Text="Upload LOC" OnClientClick="onBeginUpload();" runat="server" ValidationGroup="Upload" /> <asp:Label ID="LabelNoFileUploaded" Text="" runat="server" />
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
                <h2><asp:Label ID="LabelModalInstructionHeader" Text="Bank Screenshot LOC" runat="server" /></h2>
                <span style="text-align:center"><asp:Image runat="server" ID="ImageDownloadInstructionsFull" /></span><br /><br /><hr /><br />
                <asp:Literal ID="LiteralDownloadInstructionsModal" runat="server" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostbackTrigger ControlID="ButtonSebFile" EventName="Click" />
                <asp:AsyncPostbackTrigger ControlID="ButtonBankgiroSEFile" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="ButtonPaypalFile" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="ButtonPaysonFile" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <br/>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

