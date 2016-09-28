<%@ Control Language="C#" AutoEventWireup="true" Inherits="Swarmops.Frontend.Controls.Swarm.AjaxComboPeople" Codebehind="AjaxComboPeople.ascx.cs" %>
<%@ Register tagPrefix="Swarmops5Workaround" tagName="ComboPeople" src="~/Controls/v5/Swarm/ComboPeople.ascx" %>

<script language="javascript" type="text/javascript">

    $(document).ready(function() {
        <%= ComboPeople.ClientID %>_placeholder(decodeURIComponent('<%=JavascriptEscape(this.Placeholder)%>'));
    });

    function <%=this.ClientID%>_pvt_onSelectionChange(newPersonId) {

        <%= ComboPeople.ClientID %>_backgroundColor ('#FFFFE0');
        SwarmopsJS.ajaxCall(
            "<%=this.AjaxCallbackUrl%>",
            { cookie: <%=this.ClientID%>_cookieValue },
            function(result) {
                <%= ComboPeople.ClientID %>_backgroundColor('#E0FFE0');
                <%= ComboPeople.ClientID %>_backgroundToWhite();
                <%=this.ClientID%>_selectedId = newPersonId;

                <% if (!String.IsNullOrEmpty(this.OnChange))
                   {
                       Response.Write (this.OnChange + "(newPersonId);");

                   }%>

            },
            function(error) {
                <%= ComboPeople.ClientID %>_backgroundColor ('#FFA0A0');
                <%= ComboPeople.ClientID %>_backgroundToWhite();
                alertify.error(decodeURIComponent('<asp:Literal ID="LiteralServerError" runat="server"/>'));
            });
    }

    function <%=this.ClientID%>_cookie(newValue) {
        if (newValue !== undefined) {
            <%=this.ClientID%>_cookieValue = newValue;
        }
        return <%=this.ClientID%>_cookieValue;
    }

    function <%=this.ClientID%>_clear() {
        <%=this.ComboPeople.ClientID%>_val('');
    }

    var <%=this.ClientID%>_cookieValue = '<%=this.Cookie%>';
    var <%=this.ClientID%>_selectedId = 0;

</script>

<Swarmops5Workaround:ComboPeople ID="ComboPeople" runat="server" />