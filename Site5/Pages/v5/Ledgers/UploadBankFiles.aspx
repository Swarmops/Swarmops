<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="UploadBankFiles.aspx.cs" Inherits="Activizr.Site.Pages.Ledgers.UploadBankFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
<style type="text/css">
div.FileTypeImage
{
	width:160px;
	height:70px;
	border: 1px solid #1C397E;
	margin-right:13px;
	margin-top:7px;
	margin-bottom:6px;
	float:left;
}

div.FileTypeImageSelected
{
	/* box-shadow orange */
}

div.BankUploadInstructionsImage
{
	float:right;
}

</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label ID="LabelSelectBankUploadFilter" Text="Select File Type To Upload" runat="server"/></h2>
    <asp:ImageButton CssClass="FileTypeImage" ID="ButtonSebAccountFile" runat="server" ImageUrl="~/Images/Ledgers/uploadbankfiles-type-seb-kontoutdrag.png"/>
    <asp:ImageButton CssClass="FileTypeImage" ID="ButtonSebPaymentFile" runat="server" ImageUrl="~/Images/Ledgers/uploadbankfiles-type-seb-bankgirofil.png"/>
    <div style="clear:both;margin-bottom:10px"></div>
    <asp:Label ID="LabelSelectToContinue" Text="Select to continue." runat="server" />
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

