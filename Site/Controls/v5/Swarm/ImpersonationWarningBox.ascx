<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImpersonationWarningBox.ascx.cs" Inherits="Swarmops.Frontend.Controls.Swarm.ImpersonationWarningBox" %>

<asp:Panel runat="server" ID="PanelImpersonationWarning">
    
    <script>
        // This control will only ever be included once, so it's safe to put global script here

        function terminateImpersonation() {
            SwarmopsJS.ajaxCall("/Automation/SwarmFunctions.aspx/TerminateImpersonation", {}, function(result) {
                if (result.Success) {
                    // Non-impersonating auth coookie has been set, so reload the page
                    location.reload(true);
                }
            });
        }
        
        function flashOrangeHeader() {
            if (!_orangeFlashState) {
                $('h2.flashheader').addClass("orangeDark").removeClass("orange");
                _orangeFlashState = true;
                setTimeout("flashOrangeHeader();", 500);
            } else {
                $('h2.flashheader').removeClass("orangeDark").addClass("orange");
                _orangeFlashState = false;
                setTimeout("flashOrangeHeader();", 1500);
            }
        }

        $(document).ready(function() {
            flashOrangeHeader();
        });



        var _orangeFlashState = false;
    </script>

    <!-- this box is only visible on the Master when impersonation is active -->

    <h2 class="orange flashheader"><asp:Label ID="LabelImpersonationWarningHeader" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Literal ID="LiteralImpersonationWarningText" runat="server"/>
        </div>
    </div>

</asp:Panel>