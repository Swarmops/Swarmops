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

        if (!<%=this.ClientID%>_initialized) {
            $('#<%=this.ClientID%>_hiddenInitArea div.divInitializedContent').appendTo('#<%=this.ClientID%>_divModalBox div.content');
            <%=this.ClientID%>_initialized = true;
        }

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

    $(document).ready(function() {
        // assuming this runs AFTER the page doc.ready, when easyUI layout happens

        $('#<%=this.ClientID %>_hiddenInitArea').css('display', 'none'); // remove from layout
    });

    var <%=this.ClientID%>_initialized = false;


</script>

<div id="<%=this.ClientID %>_divModalCover" class="modalCover">
    <div id="<%=this.ClientID %>_divModalWrap" class="modalWrap">
        <div id="<%=this.ClientID %>_divModalBox" class="box modal">
            <div class="content">
            </div>
        </div>
        <div id="<%=this.ClientID %>_iconClose" class="modalIconClose"></div>
    </div>
</div>


<div id="<%=this.ClientID %>_hiddenInitArea" class="box modal" style="visibility:hidden; width: 960px"> <!-- width necessary for correct init -->
    <div class="divInitializedContent" >
        <asp:PlaceHolder runat="server" ID="PlaceHolderDialog" />
    </div>
</div>