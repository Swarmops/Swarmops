<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AjaxToggleSlider.ascx.cs" Inherits="Swarmops.Frontend.Controls.Base.AjaxToggleSlider" %>

<script type="text/javascript">

    var _initVal_<%=this.TextInput.ClientID%> = $('#<%=this.TextInput.ClientID%>').val();

    $(document).ready(function () {

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

                $(this).css('background-color', '#FFFFA0');

                if ("" != "<%=this.AjaxCallbackUrl%>") { // if there's a direct callback AJAX url

                    SwarmopsJS.ajaxCall(
                        "<%=this.AjaxCallbackUrl%>",
                        jsonData,
                        $.proxy(function(msg) {
                            if (msg.Success == false) {
                                if (msg.DisplayMessage != null) {
                                    alertify.error(msg.DisplayMessage);
                                } else {
                                    alertify.error(SwarmopsJS.unescape('<%= this.Localized_AjaxGeneralErrorSettingValue %>'));
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
                            alertify.error(SwarmopsJS.unescape('<%= this.Localized_AjaxCallException %>'));
                            $(this).val(_initVal_<%=this.TextInput.ClientID%>);
                            $(this).css('background-color', '#FFA0A0');
                            $(this).animate({ backgroundColor: "#FFFFFF" }, 250);
                        }, this)
                    );
                } else {
                    // if there's no AJAX callback URL given, the javascript handler must take care of it
                    // Resharper marks this code as unused because it falsely thinks the if condition above is always true

                    <%=this.OnChange%> (currentValue, '<%=this.Cookie%>', '<%=this.ClientID%>'); // JavaScript callback on successful change
                }
            }
        });

    });


    function <%=this.ClientID%>_disable() {
        $('#<%=this.TextInput.ClientID%>').attr ('disabled', 'disabled');
    }

    function <%=this.ClientID%>_enable() {
        $('#<%=this.TextInput.ClientID%>').removeAttr('disabled');
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

<asp:Label runat="server" ID="ToggleSliderLabel"></asp:Label>
<div class="toggle-iphone toggle-padding"><asp:Checkbox ID="SliderCheckbox" runat="server"/></div>
