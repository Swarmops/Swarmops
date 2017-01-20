<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Admin.CommenceImpersonation" CodeFile="CommenceImpersonation.aspx.cs" %>
<%@ Register src="~/Controls/v5/Base/FileUpload.ascx" tagname="FileUpload" tagprefix="Swarmops5" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboPeople" Src="~/Controls/v5/Swarm/ComboPeople.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="CurrencyAmount" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

    <script type="text/javascript">

        $(document).ready(function () {
 
        });
        
        function dialogBeginImpersonation() {
            var selectedPersonId = <%=this.ComboPeople.ClientID%>_selectedPersonId();
            if (selectedPersonId < 1) {
                alertify.log("You need to select a person."); // TODO: LOC
                return false;
            }
	                    
            alertify.set({
	            labels: {
	                ok: SwarmopsJS.unescape('<%=this.Localized_ConfirmDialog_Proceed%>'),
	                cancel: SwarmopsJS.unescape('<%=this.Localized_ConfirmDialog_Cancel%>')
	            },
	            buttonFocus: 'cancel'
	        });

	        alertify.confirm(SwarmopsJS.unescape('<%=this.Localized_ConfirmDialog_Text%>'),
	            $.proxy(function (response) {
	                if (response) {
 	                    selectedPersonId = <%=this.ComboPeople.ClientID%>_selectedPersonId();
	                    SwarmopsJS.ajaxCall("/Pages/v5/Admin/CommenceImpersonation.aspx/Commence",
	                        { personId: selectedPersonId },
	                        function(result) {
	                            // if Success is true, we have a new auth cookie and should redir to Dashboard,
	                            // otherwise, we should dialog the returned DisplayMessage
	                            if (result.Success) {
	                                document.location = "/"; // redirect
	                            } else {
	                                alert.dialog(result.DisplayMessage);
	                            }


	                        });
	                } else {
	                    alertify.log("Canceled.");
	                    // Do nothing on cancel
	                }
	            }, this));

            return false; // prevent further processing of button
        }

    </script>
</asp:Content>




<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="LabelImpersonationHeader" /></h2>
    <asp:Literal runat="server" ID="LiteralImpersonationWarning"/>
    <div class="entryFields">
        <Swarmops5:ComboPeople ID="ComboPeople" runat="server" />
        <asp:Button ID="ButtonImpersonate" runat="server" CssClass="buttonAccentColor NoInputFocus" OnClientClick="dialogBeginImpersonation();" Text="#Impersonate#"/>
    </div>
    <div class="entryLabels">
        <asp:Label runat="server" ID="LabelPerson" />
    </div>
    <div style="clear:both"></div>
</asp:Content>



<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

