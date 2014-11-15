<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DropDown.ascx.cs" Inherits="Swarmops.Frontend.Controls.v5.Base.DropDown" %>

    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('#<%=this.DropControl.ClientID %>').combobox({
                editable: false,
                height: 30,
                width: 300
            });
            $('#<%=this.DropControl.ClientID %>').combobox('setValue', '<%=this.DropControl.SelectedValue%>');

            $('#<%=this.DropControl.ClientID %> + span.combo > input.combo-text').click(function () {
                $('#<%=this.DropControl.ClientID %> + span.combo > span > span.combo-arrow').click();
            });

            $('#<%=this.DropControl.ClientID %> + span.combo > input.combo-text').keydown(function (e) {
                switch (e.which) {
                    case 40: // down
                        var panel = $('#<%=this.DropControl.ClientID %>').combo('panel');
                        if (!panel.is(':visible')) {
                            $('#<%=this.DropControl.ClientID %> + span.combo > span > span.combo-arrow').click();
                            panel.focus();
                        }
                        break;

                    default: return; // exit this handler for other keys
                }
                e.preventDefault(); // prevent the default action (scroll / move caret)
            });
        });
    </script>


<asp:DropDownList ID="DropControl" runat="server" />
