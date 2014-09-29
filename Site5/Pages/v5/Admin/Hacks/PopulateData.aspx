<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="PopulateData.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Admin.Hacks.PopulateData" %>
<%@ Register src="~/Controls/v5/Base/FileUpload.ascx" tagname="FileUpload" tagprefix="Swarmops5" %>
<%@ Register TagPrefix="Swarmops5" TagName="ExternalScripts" Src="~/Controls/v5/UI/ExternalScripts.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ExternalScripts1" Package="easyui" Control="tree" runat="server" />
    <!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
    <script src="/Scripts/jquery.fileupload/jquery.iframe-transport.js" type="text/javascript" language="javascript"></script>
    <!-- The basic File Upload plugin -->
    <script src="/Scripts/jquery.fileupload/jquery.fileupload.js" type="text/javascript" language="javascript"></script>
	<link rel="stylesheet" type="text/css" href="/Style/v5-easyui-elements.css">

    <script type="text/javascript">

        $(document).ready(function () {
 
        });




    </script>
</asp:Content>


<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <asp:TextBox runat="server" ID="TextData" TextMode="MultiLine" Rows="20" Width="100%" />
    
    <asp:Button ID="ButtonProcess" runat="server" CssClass="buttonAccentColor NoInputFocus" OnClick="ButtonProcess_Click" Text="Process"/>
    <div style="clear:both"></div>
</asp:Content>



<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

