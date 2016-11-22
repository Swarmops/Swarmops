<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="AjaxTextBox.ascx.cs" Inherits="Swarmops.Frontend.Controls.Base.AjaxTextBox" %>

<%-- ReSharper disable HeuristicallyUnreachableCode --%>
<%-- ReSharper disable once ConditionIsAlwaysConst --%>
<script type="text/javascript">

    var _initVal_<%=this.TextInput.ClientID%> = $('#<%=this.TextInput.ClientID%>').val();

    $(document).ready(function () {
        $('#<%=this.TextInput.ClientID%>').keydown(function (e) {

            if (e.keyCode != 9) { // tab key

                // reset timer if set
                if (<%=this.ClientID%>_onChangingTimer != null) {
                    clearTimeout(<%=this.ClientID%>_onChangingTimer);
                    <%=this.ClientID%>_onChangingTimer = setTimeout(function() { <%=this.ClientID%>_fireOnChanging(); }, 500);
                } else {
                    // otherwise set new timer with a more generous timeout on keydown in general
                    <%=this.ClientID%>_onChangingTimer = setTimeout(function() { <%=this.ClientID%>_fireOnChanging(); }, 500);
                }

                <%
                    if (!string.IsNullOrEmpty(this.OnKeyDown))
                    {
                        Response.Write(this.OnKeyDown + "(e.keyCode);"); 
                    }
                %>
            }
        });

        // OnChange event, AjaxCallback event
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
                
                <%
                    if (!string.IsNullOrEmpty(this.OnChange) || !string.IsNullOrEmpty(this.AjaxCallbackUrl))
                    {
                        Response.Write("$(this).css('background-color', '#FFFFA0');"); // set background yellow indicating processing
                    }
                %>


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
                                $(this).css('background-color', '#FFA0A0'); // set background red indicating error
                            } else {
                                if (msg.DisplayMessage != null) {
                                    alertify.log(msg.DisplayMessage);
                                }
                                $(this).css('background-color', '#E0FFE0'); // set background green indicating success
                                _initVal_<%=this.TextInput.ClientID%> = msg.NewValue;
                            }
                            $(this).val(_initVal_<%=this.TextInput.ClientID%>);
                            $(this).animate({ backgroundColor: "#FFFFFF" }, 250); // animate background back to white
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

    function <%=this.ClientID%>_fireOnChanging() {
        <%=this.ClientID%>_onChangingTimer = null;

        <%
            if (!string.IsNullOrEmpty(this.OnChanging))
            {
                Response.Write(this.OnChanging + "($('#" + this.TextInput.ClientID + "').val(), '" + this.Cookie + "', '" + this.ClientID + "');"); // cookie may be empty and that's ok 
            }
        %>
    }

    var <%=this.ClientID%>_onChangingTimer = null;

    function <%=this.ClientID%>_updateSuccessAnimate(newValue) {
        $('#<%=this.TextInput.ClientID%>').css('background-color', '#E0FFE0');
        $('#<%=this.TextInput.ClientID%>').animate({ backgroundColor: "#FFFFFF" }, 500);
        <%=this.ClientID%>_initialize(newValue);
    }

    function <%=this.ClientID%>_updateFailAnimate(newValue) {
        $('#<%=this.TextInput.ClientID%>').css('background-color', '#FFA0A0');
        $('#<%=this.TextInput.ClientID%>').animate({ backgroundColor: "#FFFFFF" }, 1000);
        <%=this.ClientID%>_initialize(newValue);
    }

    function <%=this.ClientID%>_updateProgressAnimate() {
        $('#<%=this.TextInput.ClientID%>').css('background-color', '#FFFFA0');
    }


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
        <%=this.ClientID%>_enable();
    }



</script>

<asp:TextBox ID="TextInput" runat="server" />
