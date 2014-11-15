<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DropDown.ascx.cs" Inherits="Swarmops.Frontend.Controls.v5.Base.DropDown" %>

    <script language="javascript" type="text/javascript">
        <script type="text/javascript">
            $(document).ready(function () {
                $('#<%=this.DropControl.ClientID %>').combobox({
                    animate: true,
                    height: 30
        });

        $('#<%=this.DropControl.ClientID %> span.combo input.combo-text').click(function () {
            $('#<%=this.DropControl.ClientID %> span.combo span span.combo-arrow').click();
        });

        $('#<%=this.DropControl.ClientID %>_SpanBudgets span.combo input.combo-text').keydown(function (e) {
            switch (e.which) {
                case 40: // down
                    $('#<%=this.DropControl.ClientID %>_SpanBudgets span.combo span span.combo-arrow').click();
                    break;

                default: return; // exit this handler for other keys
            }
            e.preventDefault(); // prevent the default action (scroll / move caret)
        });
    });
 </script>


<asp:DropDownList ID="DropControl" runat="server" />
