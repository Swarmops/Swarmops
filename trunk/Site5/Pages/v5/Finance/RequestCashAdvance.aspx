<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="RequestCashAdvance.aspx.cs" Inherits="Pages_v5_Finance_RequestCashAdvance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script src="https://hostedscripts.falkvinge.net/easyui/jquery.easyui.min.js" type="text/javascript"></script>
	<link rel="stylesheet" type="text/css" href="https://hostedscripts.falkvinge.net/easyui/themes/icon.css">    <link rel="stylesheet" type="text/css" href="https://hostedscripts.falkvinge.net/easyui/themes/default/tree.css"/>	<link rel="stylesheet" type="text/css" href="/Style/v5-easyui-elements.css">    <script type="text/javascript">
        $(document).ready(function () {
            $('#DropBudgets').combotree({
                animate: true
            });  // Is this init call even necessary?

            $('input.combo-text').click(function () {
                $('span.combo-arrow').click();
            });
        });


        function validateFields() {
            var isValid = true;
            
            isValid = validateTextField('#<%=this.TextAccount.ClientID %>', "Please state your bank account number.") && isValid;
            isValid = validateTextField('#<%=this.TextClearing.ClientID %>', "Please enter your bank's clearing number.") && isValid;
            isValid = validateTextField('#<%=this.TextBank.ClientID %>', "Please enter your bank's name.") && isValid;

            if ($('#DropBudgets').combotree('tree').tree('getSelected') == null) {
                isValid = false;
                alertify.error("Please select a budget.");
            }

            isValid = validateTextField('#<%=this.TextPurpose.ClientID %>', "Please state the purpose of the cash advance.") && isValid;

            $.ajax({
                type: "POST",
                url: "/Automation/FieldValidation.asmx/IsAmountValid",
                data: "{'amount': '" + escape($('#<%=this.TextAmount.ClientID %>').val()) + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,  // blocks until function returns - race conditions otherwise
                success: function (msg) {
                    if (msg.d != true) {
                        isValid = false;
                        alertify.error("Please enter the amount of money (in SEK) that you'd like to advance.");
                        $('#<%=this.TextAmount.ClientID %>').focus();
                    }
                }
            });

            return isValid;
        }
        
        function validateTextField (fieldId, message) {
            if ($(fieldId).val().length == 0) {
                alertify.error(message);
                $(fieldId).focus();
                return false;
            }

            return true;
        }

    </script></asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="entryFields">
        <asp:TextBox runat="server" ID="TextAmount" CssClass="alignRight" />&nbsp;<br/>
        <asp:TextBox runat="server" ID="TextPurpose" />&nbsp;<br/>
        <select class="easyui-combotree" url="Json-ExpensableBudgetsTree.aspx" name="DropBudgets" id="DropBudgets" animate="true" style="width:300px"></select>&nbsp;<br/>
        &nbsp;<br/><!-- placeholder for label-side H2 -->
        <asp:TextBox runat="server" ID="TextBank" />&nbsp;<br/>
        <asp:TextBox runat="server" ID="TextClearing" />&nbsp;<br/>
        <asp:TextBox runat="server" ID="TextAccount" />&nbsp;<br/>
        <asp:Button ID="ButtonRequest" runat="server" CssClass="buttonAccentColor" OnClientClick="return validateFields();" Text="Request"/>
    </div>
    <div class="entryLabels">
        Amount (SEK)<br/>
        Purpose<br/>
        Budget<br/>
        <h2>Your bank details</h2>
        Bank<br/>
        Clearing#<br/>
        Account#
    </div>
    <div style="clear:both"></div>
</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

