<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DropDown.ascx.cs" Inherits="Swarmops.Frontend.Controls.v5.Base.DropDown" %>
<%@ Import Namespace="System.Threading" %>

    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('#<%=this.ClientID %>_DropDown').combobox({
                editable: false,
                height: 32,
                panelAlign: '<%= Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft? "right": "left" %>',
                panelWidth: 300<% 
                if (!String.IsNullOrEmpty(this.OnClientChange))
                {
                    Response.Write(",\r\nonChange: function(newVal, oldVal) { " + this.OnClientChange + "(oldVal, newVal); }");
                }
                           
                %>
            });

            $('#<%=this.ClientID %>_DropDown + span.combo > input.combo-text').click(function () {
                $('#<%=this.ClientID %>_DropDown + span.combo > span > span.combo-arrow').click();
            });

            $('#<%=this.ClientID %>_DropDown + span.combo > input.combo-text').keydown(function (e) {
                switch (e.which) {
                    case 40: // down
                        var panel = $('#<%=this.ClientID %>_DropDown').combo('panel');
                        if (!panel.is(':visible')) {
                            $('#<%=this.ClientID %>_DropDown + span.combo > span > span.combo-arrow').click();
                            panel.focus();
                        }
                        break;

                    default: return; // exit this handler for other keys
                }
                e.preventDefault(); // prevent the default action (scroll / move caret)
            });
        });

        function <%=this.ClientID %>_DropDown_val(newValue) {
            if (newValue === undefined) {
                // getter
                return $('#<%=this.ClientID %>_DropDown').combobox('getValue');
            } else {
                // setter
                $('#<%=this.ClientID %>_DropDown').combobox('setValue', newValue);
            }
        }

        function <%=this.ClientID %>_DropDown_text(newText) {
            if (newText === undefined) {
                // getter
                return $('#<%=this.ClientID %>_DropDown').combobox('getText');
            } else {
                // setter
                $('#<%=this.ClientID %>_DropDown').combobox('setText', newText);
            }
        }

        function <%=this.ClientID %>_DropDown_loadUrl(url) {
            $('#<%=this.ClientID %>_DropDown').combobox('reload', url);
        }

        function <%=this.ClientID %>_DropDown_loadJson(jsonData) {
            $('#<%=this.ClientID %>_DropDown').combobox({
                data: $.parseJSON(jsonData),
                valueField: 'id',
                textField: 'text',
                groupField: 'group'
                });
        }

        function <%=this.ClientID %>_DropDown_loadData(jsData) {
            $('#<%=this.ClientID %>_DropDown').combobox({
                data: jsData,
                valueField: 'id',
                textField: 'text',
                groupField: 'group'
            });
        }

    </script>


<span id="<%=this.ClientID %>_SpanDrop"><select class="easyui-combo" url="<%=this.DataUrl %>" name="DropDown" id="<%=this.ClientID %>_DropDown" valueField="value" textField="text" groupField="group" animate="true" style="width:324px"></select></span>
