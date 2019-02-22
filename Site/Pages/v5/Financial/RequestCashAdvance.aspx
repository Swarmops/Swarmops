<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Financial.RequestCashAdvance" Codebehind="RequestCashAdvance.aspx.cs" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="CurrencyTextBox" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

    <script type="text/javascript">

        function validateFields() {
            var isValid = true;
            
            isValid = validateTextField('#<%=this.TextAccount.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_BankAccount %>')) && isValid;
            isValid = validateTextField('#<%=this.TextClearing.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_BankClearing %>')) && isValid;
            isValid = validateTextField('#<%=this.TextBank.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_BankName %>')) && isValid;

            if ($('#<%=this.ComboBudgets.ClientID %>_DropBudgets').combotree('tree').tree('getSelected') == null) {
                isValid = false;
                $('#<%=this.ComboBudgets.ClientID %>_SpanBudgets').addClass("data-entry-error");
                alertify.error(SwarmopsJS.unescape('<%= this.Localized_ValidationError_Budget %>'));
            }

            isValid = validateTextField('#<%=this.TextPurpose.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_Purpose %>')) && isValid;

            var jsonData = {};
            jsonData.amount = <%=this.TextAmount.ClientID %>_val();

            $.ajax({
                type: "POST",
                url: "/Automation/FieldValidation.aspx/IsAmountValid",
                data: $.toJSON(jsonData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,  // blocks until function returns - race conditions otherwise
                success: function (msg) {
                    if (msg.d != true) {
                        isValid = false;
                        alertify.error(SwarmopsJS.unescape('<%= this.Localized_ValidationError_Amount %>'));
                        $('#<%=this.TextAmount.ClientID %>_TextInput').addClass("data-entry-error").focus();
                    }
                }
            });

            return isValid;
        }
        
        function validateTextField (fieldId, message) {
            if ($(fieldId).val().length == 0) {
                alertify.error(message);
                $(fieldId).addClass("data-entry-error");
                $(fieldId).focus();
                return false;
            }

            return true;
        }

    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label ID="BoxTitle" runat="server" /></h2>
    <div class="data-entry-fields">
        <Swarmops5:CurrencyTextBox runat="server" ID="TextAmount" CssClass="align-for-numbers" />
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextPurpose" /></div>
        <Swarmops5:ComboBudgets ID="ComboBudgets" runat="server" />
        <div class="stacked-input-control">&nbsp;</div><!-- placeholder for label-side H2 -->
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextBank" /></div>
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextClearing" /></div>
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextAccount" /></div>
        <asp:Button ID="ButtonRequest" runat="server" CssClass="button-accent-color" OnClientClick="return validateFields();" OnClick="ButtonRequest_Click" Text="Request"/>
    </div>
    <div class="data-entry-labels">
        <asp:Label runat="server" ID="LabelAmount" /><br/>
        <asp:Label runat="server" ID="LabelPurpose" /><br/>
        <asp:Label runat="server" ID="LabelBudget" /><br/>
        <h2><asp:Label runat="server" ID="LabelHeaderBankDetails" /></h2>
        <asp:Label runat="server" ID="LabelBankName" /><br/>
        <asp:Label runat="server" ID="LabelBankClearing" /><br/>
        <asp:Label runat="server" ID="LabelBankAccount" />
    </div>
    <div style="clear:both"></div>
</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

