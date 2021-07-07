﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Financial.CreateInboundInvoice" Codebehind="CreateInboundInvoice.aspx.cs" %>
<%@ Register src="~/Controls/v5/Base/FileUpload.ascx" tagname="FileUpload" tagprefix="Swarmops5" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="CurrencyAmount" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="DateTextBox" Src="~/Controls/v5/Base/DateTextBox.ascx" %>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
    <script src="/Scripts/jquery.fileupload/jquery.iframe-transport.js" type="text/javascript" language="javascript"></script>
    <!-- The basic File Upload plugin -->
    <script src="/Scripts/jquery.fileupload/jquery.fileupload.js" type="text/javascript" language="javascript"></script>

    <script type="text/javascript">

        $(document).ready(function () {

            if (vatEnabled) { // if VAT is enabled, show those controls
                $('.vatEnabled').show();
            }

            <asp:Repeater ID="RepeaterTagDropScript" runat="server"><ItemTemplate>
            $('#DropTags<%# Eval("TagSetId") %>').combotree({
                animate: true,
                height: 30
            });

            
            $('#SpanDropTags<%# Eval("TagSetId") %> span.combo input.combo-text').click(function () {
                $('#SpanDropTags<%# Eval("TagSetId") %> span.combo span span.combo-arrow').click();
            });
            </ItemTemplate></asp:Repeater>


            if (successMessage.length > 0) {
                alertify.success(successMessage);  // displayed on successful creation of invoice
            }
        });


        function validateFields() {
            var isValid = true;

            isValid = validateTextField('#<%=this.TextAccount.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_Account %>')) && isValid;

            if ($('#<%=this.ComboBudgets.ClientID %>_DropBudgets').combotree('tree').tree('getSelected') == null) {
                isValid = false;
                $('#<%=this.ComboBudgets.ClientID %>_SpanBudgets').addClass("data-entry-error");
                alertify.error(SwarmopsJS.unescape('<%= this.Localized_ValidationError_Budget %>'));
            }

            <asp:Repeater ID="RepeaterErrorCheckTags" runat="server"><ItemTemplate>
                if ($('#DropTags<%# Eval("TagSetId") %>').combotree('tree').tree('getSelected') == null) {
                    isValid = false;
                    $('#SpanDropTags<%# Eval("TagSetId") %>').addClass("data-entry-error");
                    alertify.error(SwarmopsJS.unescape('<%=this.Localized_ValidationError_MissingTag %>'));
                }
            </ItemTemplate></asp:Repeater>

            isValid = validateTextField('#<%=this.TextPurpose.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_Purpose %>')) && isValid;

            var jsonData = {};
            jsonData.amount = <%=this.CurrencyAmount.ClientID %>_val();

            $.ajax({
                type: "POST",
                url: "/Automation/FieldValidation.aspx/IsAmountValid",
                data: $.toJSON(jsonData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false, // blocks until function returns - race conditions otherwise
                success: function(msg) {
                    if (msg.d != true) {
                        isValid = false;
                        $('#<%=CurrencyAmount.ClientID %>_TextInput').addClass("data-entry-error");
                        alertify.error(SwarmopsJS.unescape('<%= this.Localized_ValidationError_Amount %>'));
                        <%=CurrencyAmount.ClientID %>_focus();
                    }
                }
            });

            if (vatEnabled) {
                $.ajax({
                    type: "POST",
                    url: "/Automation/FieldValidation.aspx/IsAmountValid",
                    data: $.toJSON(jsonData),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false, // blocks until function returns - race conditions otherwise
                    success: function(msg) {
                        if (msg.d != true) {
                            isValid = false;
                            $('#<%=CurrencyAmount.ClientID %>_TextInput').addClass("data-entry-error");
                            alertify.error(SwarmopsJS.unescape('<%= this.Localized_ValidationError_Amount %>'));
                            <%=CurrencyAmount.ClientID %>_focus();
                        }
                    }
                });
            }

            $.ajax({
                type: "POST",
                url: "/Automation/FieldValidation.aspx/AreDocumentsUploaded",
                data: "{'guidString': '<%=this.FileUpload.GuidString %>'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,  // blocks until function returns - race conditions otherwise
                success: function (msg) {
                    if (msg.d != true) {
                        isValid = false;
                        $('#TextAmount').addClass("data-entry-error");
                        alertify.error(SwarmopsJS.unescape('<%= this.Localized_ValidationError_Documents %>'));
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

        var vatEnabled = <%= this.CurrentOrganization.FinancialAccounts.AssetsVatInbound == null? "false": "true" %>;
        var successMessage = SwarmopsJS.unescape('<%= this.Localized_ForGreatJustice %>');

    </script>
    
    <style type="text/css">
        
        .vatEnabled
        { display: none;}

    </style>

</asp:Content>


<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label ID="BoxTitle" runat="server"/></h2>
    <asp:HiddenField ID="HiddenTagSetIdentifiers" runat="server"/>
    <div class="data-entry-fields">
        <Swarmops5:DateTextBox runat="server" ID="DateInvoiceIssued"/>
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextInvoiceDate"/></div>
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextSupplier" /></div>
        <Swarmops5:CurrencyAmount runat="server" ID="CurrencyAmount" />
        <span class="vatEnabled"><Swarmops5:CurrencyAmount runat="server" ID="CurrencyVat" /></span>
        <Swarmops5:DateTextBox runat="server" ID="DateInvoiceDue"/>
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextPurpose" /></div>
        <Swarmops5:ComboBudgets ID="ComboBudgets" runat="server" ListType="InvoiceableIn" />
        <asp:Repeater ID="RepeaterTagDrop" runat="server"><ItemTemplate><span id="SpanDropTags<%# Eval("TagSetId") %>"><select class="easyui-combotree" url="/Automation/Json-TransactionTagsTree.aspx?TagSetId=<%# Eval("TagSetId") %>" name="DropTags<%# Eval("TagSetId") %>" id="DropTags<%# Eval("TagSetId") %>" animate="true" style="width:300px"></select></span>&nbsp;<br/></ItemTemplate></asp:Repeater>

       <div class="stacked-input-control"></div> <!-- placeholder for label-side H2 -->
        
        <!-- file upload begins here -->
        
        <Swarmops5:FileUpload ID="FileUpload" runat="server" Filter="ImagesOrPdf"/>

        <!-- file upload ends -->

        <div class="stacked-input-control"></div> <!-- placeholder for label-side H2 -->
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextAccount" /></div>
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextReference" /></div>
        <asp:Button ID="ButtonCreate" runat="server" CssClass="button-accent-color suppress-input-focus" OnClientClick="return validateFields();" OnClick="ButtonCreate_Click" Text="Create"/>
    </div>
    <div class="data-entry-labels">
        <asp:Label runat="server" ID="LabelInvoiceDate"/><br/>
        <asp:Label runat="server" ID="LabelSupplier" /><br/>
        <asp:Label runat="server" ID="LabelAmount" /><br/>
        <span class="vatEnabled"><asp:Label runat="server" ID="LabelVat" /><br/></span>
        <asp:Label runat="server" ID="LabelDueDate" /><br/>
        <asp:Label runat="server" ID="LabelPurpose" /><br/>
        <asp:Label runat="server" ID="LabelBudget" /><br/>
        <asp:Repeater ID="RepeaterTagLabels" runat="server"><ItemTemplate><%# Eval("TagSetLocalizedName") %><br/></ItemTemplate></asp:Repeater>
        <h2><asp:Label runat="server" ID="LabelHeaderImageFiles" /></h2>
        <asp:Label runat="server" ID="LabelImageFiles" /><br/>
        <h2><asp:Label runat="server" ID="LabelHeaderBankDetails" /></h2>
        <asp:Label runat="server" ID="LabelAccount" /><br/>
        <asp:Label runat="server" ID="LabelReference" /><br/>
    </div>
    <div style="clear:both"></div>
</asp:Content>



<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

