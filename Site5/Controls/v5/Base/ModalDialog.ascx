<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ModalDialog.ascx.cs" Inherits="Swarmops.Frontend.Controls.v5.Base.ModalDialog" %>

<script type="text/javascript" language="javascript">
    $(document).ready(function() {

        $('#<%=this.ClientID %>_iconClose').click(function () {
            <%=this.ClientID%>_close();
        });

        $('#<%=this.ClientID %>_divModalCover').click (function (e) {  // click on cover
            if (e.target === this) {                                   // but not inside dialog: trigger only on this element
                <%=this.ClientID%>_close();
            }
        });

        $('#<%=this.ClientID%>').showModalDialog = <%=this.ClientID%>_open;
    });

    function <%=this.ClientID%>_open() {
        $('#<%=this.ClientID%>_divModalCover').fadeIn();
    }

    function <%=this.ClientID%>_close() {
            $('#<%=this.ClientID %>_divModalCover').fadeOut(100); // fast - 1/4 of normal time

            <%
                if (!string.IsNullOrEmpty (this.OnClientClose))
                {
                    Response.Write(this.OnClientClose + "();");
                }
            %>
    }



</script>

<div id="<%=this.ClientID %>_divModalCover" class="modalCover">
    <div id="<%=this.ClientID %>_divModalWrap" class="modalWrap">
        <div id="<%=this.ClientID %>_divModalBox" class="box modal">
            <div class="content" style="overflow: hidden">
                <asp:PlaceHolder runat="server" ID="PlaceHolderDialog" />
            </div>
        </div>
        <div id="<%=this.ClientID %>_iconClose" class="modalIconClose"></div>
    </div>
</div>