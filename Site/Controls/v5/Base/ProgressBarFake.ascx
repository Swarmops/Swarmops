<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgressBarFake.ascx.cs" CodeFile="ProgressBarFake.ascx.cs" Inherits="Swarmops.Frontend.Controls.v5.Base.ProgressBarFake" %>

<script type="text/javascript" language="javascript">
    
    function <%=this.ClientID%>_show() {
        $('#Div_<%=this.ClientID %>_Encapsulation').show();
        $('#Div_<%=this.ClientID %>_ProgressBar').progressbar({ value: 0, max: 100 });
        $('#Div_<%=this.ClientID %>_ProgressBar').progressbar({ value: 100 });
    }

    function <%=this.ClientID%>_fadeIn() {
        $('#Div_<%=this.ClientID %>_Encapsulation').slideDown().fadeIn();
        $('#Div_<%=this.ClientID %>_ProgressBar').progressbar({ value: 0, max: 100 });
        $('#Div_<%=this.ClientID %>_ProgressBar').progressbar({ value: 100 });
    }

    function <%=this.ClientID%>_fadeOut() {
        $('#Div_<%=this.ClientID %>_Encapsulation').slideUp().fadeOut();
    }

    function <%=this.ClientID%>_hide() {
        $('#Div_<%=this.ClientID %>_Encapsulation').hide();
    }


</script>

<div id="Div_<%=this.ClientID %>_Encapsulation" style="display:none">  <!-- hidden by default -->
    <h2><asp:Label runat="server" ID="LabelProcessingHeader" /></h2>
    <div id="Div_<%=this.ClientID %>_ProgressBar" style="width:100%"></div>
</div>
