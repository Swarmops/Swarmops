<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DropDown.ascx.cs" Inherits="Swarmops.Frontend.Controls.v5.Base.DropDown" %>
<%@ Import Namespace="System.Threading" %>

<script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('#<%=this.DropControl.ClientID %>').combobox({
                editable: false,
                height: 32,
                panelWidth: 300,
                panelAlign: '<%= Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft? "right": "left" %>',
                width: 300<% 
                if (!String.IsNullOrEmpty(this.OnClientChange))
                {
                    Response.Write(",\r\nonChange: function(newVal, oldVal) { " + this.OnClientChange + "(oldVal, newVal); }");
                }
                           
                %>
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
