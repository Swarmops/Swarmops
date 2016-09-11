<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AjaxTextBox.ascx.cs" Inherits="Swarmops.Frontend.Controls.v5.Base.AjaxTextBox" %>

<script type="text/javascript">
    $(document).ready(function () {

        var _initVal_<%=this.TextInput.ClientID%> = $('#<%=this.TextInput.ClientID%>').val();

        $('#<%=this.TextInput.ClientID%>').blur(function() {

            var currentValue = $('#<%=this.TextInput.ClientID%>').val();

            if (currentValue != _initVal_<%=this.TextInput.ClientID%>) {
                var jsonData = {};
                jsonData.newValue = currentValue;
                jsonData.cookie = '<%=this.Cookie%>';

                // The AJAX call expects the following function prototype:
                //
                // [WebMethod]
                // public static AjaxInputCallbackResult FunctionName (string newValue, string cookie);
                //
                // The page must include IncludedControls.JsonParameters and IncludedControls.ColorAnimation among its script.

                $(this).css('background-color', '#FFFFE0');
                SwarmopsJS.ajaxCall(
                    "<%=this.AjaxCallbackUrl%>",
                    jsonData,
                    $.proxy(function(msg) {
                        if (msg.Success == false) {
                            if (msg.DisplayMessage != null) {
                                alertify.error(msg.DisplayMessage);
                            } else {
                                alertify.error("There was an error attempting to set this value."); // TODO: Localize
                            }
                            $(this).css('background-color', '#FFA0A0');
                        } else {
                            if (msg.DisplayMessage != null) {
                                alertify.log(msg.DisplayMessage);
                            }
                            $(this).css('background-color', '#E0FFE0');
                            _initVal_<%=this.TextInput.ClientID%> = msg.NewValue;
                        }
                        $(this).val(_initVal_<%=this.TextInput.ClientID%>);
                        $(this).animate({ backgroundColor: "#FFFFFF" }, 250);
                        <%

                            if (!string.IsNullOrEmpty(this.OnChange))
                            {
                                if (string.IsNullOrEmpty (this.Cookie))
                                {
                                    Response.Write (this.OnChange + "(msg.NewValue);"); // JavaScript callback on successful change
                                }
                                else
                                {
                                    Response.Write (this.OnChange + "(msg.NewValue, '" + this.Cookie + "');"); // JavaScript callback on successful change
                                }
                            }
    
                        %>
                    }, this),
                    $.proxy(function(msg) {
                        alertify.error("<%= Resources.Global.Error_AjaxCallException %>");
                        $(this).val(_initVal_<%=this.TextInput.ClientID%>);
                        $(this).css('background-color', '#FFA0A0');
                        $(this).animate({ backgroundColor: "#FFFFFF" }, 250);
                    }, this)
                );
            }
        });

    });


    function <%=this.ClientID%>_disable() {
        $('#<%=this.TextInput.ClientID%>').attr ('disabled', 'disabled');
    }

    function <%=this.ClientID%>_disableClear() {
        <%=this.ClientID%>_disable();
        <%=this.ClientID%>_clear();
    }

    function <%=this.ClientID%>_enable() {
        $('#<%=this.TextInput.ClientID%>').removeAttr('disabled');
    }

    function <%=this.ClientID%>_clear() {
        $('#<%=this.TextInput.ClientID%>').val('');
    }

    function <%=this.ClientID%>_setValue(newValue) {
        $('#<%=this.TextInput.ClientID%>').val(newValue);
    }

    function <%=this.ClientID%>_initialize(initValue) {
        _initVal_<%=this.TextInput.ClientID%> = initValue;
        <%=this.ClientID%>_setValue(initValue);
        <%=this.ClientID%>_enable();
    }



</script>

<asp:TextBox ID="TextInput" runat="server" />
