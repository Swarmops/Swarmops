<%@ Control Language="C#" AutoEventWireup="true" Inherits="Swarmops.Frontend.Controls.Base.DateTextBox" Codebehind="DateTextBox.ascx.cs" %>
<%@ Import Namespace="Swarmops.Common.Enums" %>

<!-- TODO: Add nice autocomplete stuff -->

 <% if (this.Layout == LayoutDirection.Vertical) { %><div class="stacked-input-control"><% } %>
    <asp:TextBox ID="TextInput" runat="server" CssClass="" /><asp:HiddenField ID="InterpretedDate" runat="server"/>
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

    $(document).ready(function () {

        $('#<%=this.ClientID%>_TextInput').blur(function () {

            var dateText = $('#<%=this.ClientID%>_TextInput').val();

            var jsonData = {};
            jsonData.input = dateText;

            SwarmopsJS.ajaxCall('/Automation/Formatting.aspx/InterpretDateString',
                jsonData,
                function (data) {
                    console.log(data);
                    if (data.Success) {
                        $('#<%=this.ClientID%>_TextInput').val(data.PresentedValue);
                        $('#<%=this.ClientID%>_InterpretedDate').val(data.InterpretedValue);
                    } else {
                        $('#<%=this.ClientID%>_TextInput').val('');
                        $('#<%=this.ClientID%>_InterpretedDate').val('');
                        $('#<%=this.TextInput.ClientID%>').focus();
                    }
                },
                function(data) {
                    console.log('ERROR');
                    console.log(data);
                });
            
        });


    });

</script>
