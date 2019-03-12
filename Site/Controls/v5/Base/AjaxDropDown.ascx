<%@ Control Language="C#" AutoEventWireup="true" Inherits="Swarmops.Frontend.Controls.Base.AjaxDropDown" CodeFile="AjaxDropDown.ascx.cs" CodeBehind="AjaxDropDown.ascx.cs" %>
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

    if (!String.IsNullOrEmpty(this.OnClientChange) || !String.IsNullOrEmpty(this.AjaxCallbackUrl))
    {
        Response.Write(",\r\nonChange: function(newVal, oldVal) { ");

        if (string.IsNullOrEmpty(AjaxCallbackUrl))
        {
            // Only client-side notification, no server involved
            Response.Write(this.OnClientChange + "(oldVal, newVal);");
        }
        else
        {
            Response.Write("$(this).css('background-color', '#FFFFA0');"); // set background yellow indicating processing

            Response.Write(
                @"var jsonData = {}; " +
                @"jsonData.newValue = " + this.ClientID + "_val(); " +
                @"jsonData.cookie = '" + this.Cookie + "';" +

                @"SwarmopsJS.ajaxCall('" + this.AjaxCallbackUrl + "'," +
                    @"jsonData," +
                    @"$.proxy(function(msg) { " +
                    @"  if (msg.Success == false) { " +
                    @"    if (msg.DisplayMessage != null) { " +
                    @"       alertify.error(msg.DisplayMessage); " +
                    @"    } else { " +
                    @"      alertify.error(SwarmopsJS.unescape('" + this.Localized_AjaxGeneralErrorSettingValue + "')); " +
                    @"    } " +
                    @"      $(this).css('background-color', '#FFA0A0'); " + // set background red indicating error "
                    @"    } else { " +
                    @"      if (msg.DisplayMessage != null) { " +
                    @"        alertify.log(msg.DisplayMessage); " +
                    @"      } " +
                    @"      $(this).css('background-color', '#E0FFE0'); "); // set background green indicating success


            if (!string.IsNullOrEmpty(this.OnClientChange))
            {
                // There should be a callback as well once the server call returns with success
                if (string.IsNullOrEmpty (this.Cookie))
                {
                    Response.Write (this.OnClientChange + "(msg.NewValue);"); // JavaScript callback on successful change
                }
                else
                {
                    Response.Write (this.OnClientChange + "(msg.NewValue, '" + this.Cookie + "');"); // JavaScript callback on successful change
                }
            }

                        
            Response.Write(
                    @"    } " +
                    @"    $(this).animate({ backgroundColor: '#FFFFFF' }, 250); " +  // animate background back to white
                    @"  }, this), " +
                    @"  $.proxy(function(msg) { " +
                    @"        alertify.error(SwarmopsJS.unescape('" + this.Localized_AjaxCallException + ">')); " +
//                     @"        $(this).val(_initVal_" + this.TextInput.ClientID + "); " +   // TODO: Need failure scenario
                    @"        $(this).css('background-color', '#FFA0A0'); " +
                    @"        $(this).animate({ backgroundColor: '#FFFFFF' }, 250); " +
                    @"    }, this) " +
                    @");");  // ends the SwarmopsJS.ajaxCall
        }


        Response.Write(" }"); // ends the onChange function
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
