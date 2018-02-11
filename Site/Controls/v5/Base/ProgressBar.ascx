<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgressBar.ascx.cs" CodeFile="ProgressBar.ascx.cs" Inherits="Swarmops.Frontend.Controls.v5.Base.ProgressBar" %>

<script type="text/javascript" language="javascript">
    
    function <%=this.ClientID%>_show() {
        $('#Div_<%=this.ClientID %>_Encapsulation').show();
    }

    function <%=this.ClientID%>_fadeIn() {
        $('#Div_<%=this.ClientID %>_Encapsulation').slideDown().fadeIn();
    }

    function <%=this.ClientID%>_fadeOut() {
        $('#Div_<%=this.ClientID %>_Encapsulation').slideUp().fadeOut();
    }

    function <%=this.ClientID%>_hide() {
        $('#Div_<%=this.ClientID %>_Encapsulation').hide();
    }

    function <%=this.ClientID%>_reset() {
        $('#Div_<%=this.ClientID %>_ProgressBar').progressbar({ value: 0 });
    }

    function <%=this.ClientID%>_begin() {
        $('#Div_<%=this.ClientID %>_ProgressBar').progressbar({ value: 0, max: 100 });
        <%=this.ClientID%>_nextPollTimer = setTimeout(function() { <%= this.ClientID%>_progressFallbackPoll(); }, 2500);
        progressUpdateCallback_<%=this.GuidToken%>(1); // Initializes a small piece of bar
    }

    function progressUpdateCallback_<%=this.GuidToken%>(newProgress)
    {
        // This function primarily gets called via a socket handler invoked on the master page, but also from our fallback polling.

        alertify.log(newProgress);  // debug

        if (newProgress > <%=this.ClientID%>_lastProgress) {

            if (newProgress < 100) {
                // The 100% complete case is dealt with later

                $("#Div_<%=this.ClientID %>_ProgressBar .progressbar-value").animate(
                {
                    width: newProgress + "%"
                }, { queue: false });
            }

            if (<%=this.ClientID%>_lastProgress < 50 && newProgress >= 50) {
                // check for halfway callback

                <% if (!String.IsNullOrEmpty(this.OnClientProgressHalfwayCallback)) { %>
            
                    <%=this.OnClientProgressHalfwayCallback%>('<%=this.Guid%>');
            
                <% } %>
            }

            if (<%=this.ClientID%>_lastProgress < 100 && newProgress >= 100) {

                $("#Div_<%=this.ClientID %>_ProgressBar .progressbar-value").animate(
                    {
                        width: "100%"
                    }, { queue: false });

                // check for completion callback

                <% if (!String.IsNullOrEmpty(this.OnClientProgressCompleteCallback)) { %>
            
                    <%=this.OnClientProgressCompleteCallback%>('<%=this.Guid%>');
            
                <% } %>

                // cancel any outstanding timer

                if (<%=this.ClientID%>_nextPollTimer != null) {
                    clearTimeout(<%=this.ClientID%>_nextPollTimer);
                    <%=this.ClientID%>_nextPollTimer = null;
                }
            }

            <%=this.ClientID%>_lastProgress = newProgress;

        }

    }

    var <%=this.ClientID%>_lastProgress = 0;
    var <%=this.ClientID%>_nextPollTimer = null;

    function <%= this.ClientID%>_progressFallbackPoll() {
        <%=this.ClientID%>_nextPollTimer = null;

        alertify.log("ProgressPollTimer"); // debug

        SwarmopsJS.ajaxCall(
            "/Automation/Json-ByGuid.aspx/GetNonsocketProgress",
            { guid: '<%= this.Guid%>' },
            function(result) {

                if (result.Success) {

                    // Call our ordinary progress updater from the poller

                    progressUpdateCallback_<%=this.GuidToken%>(result.DisplayMessage);

                    // If the progress is less than 100, schedule another poll

                    if (result.DisplayMessage < 100) {
                        <%=this.ClientID%>_nextPollTimer = setTimeout(function() { <%= this.ClientID%>_progressFallbackPoll(); }, 2500);
                    }
                }
            }
        );
    }


</script>

<div id="Div_<%=this.ClientID %>_Encapsulation" style="display:none">  <!-- hidden by default -->
    <h2><asp:Label runat="server" ID="LabelProcessingHeader" /></h2>
    <div id="Div_<%=this.ClientID %>_ProgressBar" style="width:100%"></div>
</div>
