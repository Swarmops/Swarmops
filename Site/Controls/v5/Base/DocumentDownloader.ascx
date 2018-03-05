<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DocumentDownloader.ascx.cs" CodeBehind="DocumentDownloader.ascx.cs" Inherits="Swarmops.Frontend.Controls.Base.DocumentDownloader" %>

<iframe id="<%=this.ClientID %>_iframe" style="display:none"></iframe>

<script>
    
    function downloadDocument(docId, docName) {
        document.getElementById('<%=this.ClientID%>_iframe').src = '/Pages/v5/Support/DownloadUploadedDocument.aspx?DocId=' + docId + '&DocName=' + encodeURIComponent(docName);
    };

</script>
