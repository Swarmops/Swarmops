<%@ Control Language="C#" AutoEventWireup="true" Inherits="Swarmops.Frontend.Controls.Base.DropDown" CodeFile="DropDown.ascx.cs" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="Swarmops.Common.Enums" %>

<script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('#<%=this.ClientID %>_DropControl').combobox({
                editable: false,
                height: 32,
                width: 324,
                panelAlign: '<%= Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft? "right": "left" %>',
                panelWidth: 300<% 
                if (!String.IsNullOrEmpty(this.OnClientChange))
                {
                    Response.Write(",\r\nonChange: function(newVal, oldVal) { " + this.OnClientChange + "(oldVal, newVal); }");
                }
                           
                %>
            });

            $('#<%=this.ClientID %>_DropControl + span.combo > input.combo-text').click(function () {
                $('#<%=this.ClientID %>_DropControl + span.combo > span > span.combo-arrow').click();
            });

            $('#<%=this.ClientID %>_DropControl + span.combo > input.combo-text').keydown(function (e) {
                switch (e.which) {
                    case 40: // down
                        var panel = $('#<%=this.ClientID %>_DropControl').combo('panel');
                        if (!panel.is(':visible')) {
                            $('#<%=this.ClientID %>_DropControl + span.combo > span > span.combo-arrow').click();
                            panel.focus();
                        }
                        break;

                    default: return; // exit this handler for other keys
                }
                e.preventDefault(); // prevent the default action (scroll / move caret)
            });
        });

        function <%=this.ClientID %>_val(newValue) {
            if (newValue === undefined) {
                // getter
                return $('#<%=this.ClientID %>_DropControl').combobox('getValue');
            } else {
                // setter
                $('#<%=this.ClientID %>_DropControl').combobox('setValue', newValue);
            }
        }

        function <%=this.ClientID %>_text(newText) {
            if (newText === undefined) {
                // getter
                return $('#<%=this.ClientID %>_DropControl').combobox('getText');
            } else {
                // setter
                $('#<%=this.ClientID %>_DropControl').combobox('setText', newText);
            }
        }

        function <%=this.ClientID %>_loadUrl(url) {
            $('#<%=this.ClientID %>_DropControl').combobox('reload', url);
        }

        function <%=this.ClientID %>_loadJson(jsonData) {
            $('#<%=this.ClientID %>_DropControl').combobox({
                data: $.parseJSON(jsonData),
                valueField: 'id',
                textField: 'text',
                groupField: 'group'
                });
        }

        function <%=this.ClientID %>_loadData(jsData) {
            $('#<%=this.ClientID %>_DropControl').combobox({
                data: jsData,
                valueField: 'id',
                textField: 'text',
                groupField: 'group'
            });
        }

    </script>


<% if (this.Layout == LayoutDirection.Vertical){ %><div class="stacked-input-control"><% } %>
    <span id="<%=this.ClientID %>_SpanDrop"><asp:DropDownList ID="DropControl" runat="server" /></span>
<% if (this.Layout == LayoutDirection.Vertical) { %></div><% } %>
