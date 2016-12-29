<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AjaxToggleSlider.ascx.cs" Inherits="Swarmops.Frontend.Controls.Base.AjaxToggleSlider" %>

<script type="text/javascript">

    var _initVal_<%=this.SliderCheckbox.ClientID%> = <%= this.InitialValue.ToString().ToLower() %>; 

    $(document).ready(function () {

        $('#<%=this.ClientID%>_sliderContainer').toggles(
            { 
                on: <%= this.InitialValue.ToString().ToLower() %>, 
                checkbox: $('#<%=this.SliderCheckbox.ClientID%>'), 
                text:
                {
                    on: SwarmopsJS.unescape('<%= this.Localized_SwitchLabelOn_Upper %>'), 
                    off: SwarmopsJS.unescape('<%= this.Localized_SwitchLabelOff_Upper %>')
                } 
            })
            .on('toggle', function(e, active) {

                if (active != _initVal_<%=this.SliderCheckbox.ClientID%>) {

                    // The AJAX call expects the following function prototype:
                    //
                    // [WebMethod]
                    // public static AjaxInputCallbackResult FunctionName (bool newValue, string cookie);
                    //
                    // The page must include IncludedControls.JsonParameters and IncludedControls.ColorAnimation among its script.

                    // $(this).css('background-color', '#FFFFA0'); -- TODO: Find a way to mark ongoing update for slider

                    if ("" != "<%=this.AjaxCallbackUrl%>") { // if there's a direct callback AJAX url

                        var jsonData = {};
                        jsonData.newValue = active;
                        jsonData.cookie = '<%=this.Cookie%>';

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
                                    _initVal_<%=this.SliderCheckbox.ClientID%> = msg.NewValue;
                                }
                                $(this).val(msg.NewValue);
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
                                $(this).val(_initVal_<%=this.SliderCheckbox.ClientID%>);
                                $(this).css('background-color', '#FFA0A0');
                                $(this).animate({ backgroundColor: "#FFFFFF" }, 250);
                            }, this)
                        );
                    } else {
                        // if there's no AJAX callback URL given, the javascript handler must take care of it
                        // Resharper marks this code as unused because it falsely thinks the if condition above is always true

                        _initVal_<%=this.SliderCheckbox.ClientID%> = active;
                        <%=this.OnChange%> (active, '<%=this.Cookie%>', '<%=this.ClientID%>'); // JavaScript callback on successful change
                    }
                }
            });
        }
    );


    function <%=this.ClientID%>_disable() {
        $('#<%=this.SliderCheckbox.ClientID%>').attr ('disabled', 'disabled');
    }

    function <%=this.ClientID%>_enable() {
        $('#<%=this.SliderCheckbox.ClientID%>').removeAttr('disabled');
    }

    function <%=this.ClientID%>_setValue(newValue) {
        $('#<%=this.ClientID%>_sliderContainer').data('toggles').toggle(newValue, true, true);
    }

    function <%=this.ClientID%>_initialize(initValue) {
        _initVal_<%=this.SliderCheckbox.ClientID%> = initValue;
        <%=this.ClientID%>_setValue(initValue);
        <%=this.ClientID%>_enable();
    }



</script>

<div class="stacked-input-control"><asp:Label runat="server" ID="ToggleSliderLabel"></asp:Label>
<div class="toggle-iphone toggle-padding" id="<%=this.ClientID %>_sliderContainer"><asp:Checkbox ID="SliderCheckbox" runat="server"/></div></div>
