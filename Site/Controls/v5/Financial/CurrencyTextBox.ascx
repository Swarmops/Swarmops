<%@ Control Language="C#" AutoEventWireup="true" Inherits="Swarmops.Frontend.Controls.Financial.CurrencyTextBox" Codebehind="CurrencyTextBox.ascx.cs" %>
<%@ Import Namespace="Swarmops.Common.Enums" %>

<!-- TODO: Add nice autocomplete stuff -->

 <% if (this.Layout == LayoutDirection.Vertical) { %><div class="stacked-input-control"><% } %>
    <asp:TextBox ID="TextInput" runat="server" CssClass="alignRight" /><asp:HiddenField ID="NativeCurrency" runat="server"/><asp:HiddenField ID="NativeAmount" runat="server" />
 <% if (this.Layout == LayoutDirection.Vertical) { %></div><% } %>


<script type="text/javascript">

    function <%=this.ClientID%>_focus() {
        $('#<%=this.TextInput.ClientID%>').focus();
    }

    function <%=this.ClientID%>_setValue(newValue) {
        $('#<%=this.TextInput.ClientID%>').val(newValue);
    }

    function <%=this.ClientID%>_val() {
        return $('#<%=this.TextInput.ClientID%>').val();
    }

    function <%=this.ClientID%>_initialize(initValue) {
        _initVal_<%=this.TextInput.ClientID%> = initValue;
        <%=this.ClientID%>_setValue(initValue);
        // <%=this.ClientID%>_enable();
    }

    $(document).ready(function() {
        $('#<%=this.ClientID%>_TextInput').blur(function() {
            alert('currency blur');
        });

    });

</script>
