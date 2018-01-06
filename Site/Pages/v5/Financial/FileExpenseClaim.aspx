<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Financial.FileExpenseClaim" CodeFile="FileExpenseClaim.aspx.cs" Codebehind="FileExpenseClaim.aspx.cs" %>
<%@ Register src="~/Controls/v5/Base/FileUpload.ascx" tagname="FileUpload" tagprefix="Swarmops5" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="Currency" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

    <script type="text/javascript">

        $(document).ready(function () {

            $('#divTabs').tabs();
 
            if (vatEnable) {
                $('.enableVatField').show();
            }

        });



        function validateFields() {
            var isValid = true;
            
            isValid = validateTextField('#<%=this.TextAccount.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_BankAccount %>')) && isValid;
            isValid = validateTextField('#<%=this.TextClearing.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_BankClearing %>')) && isValid;
            isValid = validateTextField('#<%=this.TextBank.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_BankName %>')) && isValid;

            if ($('#<%=this.ComboBudgets.ClientID %>_DropBudgets').combotree('tree').tree('getSelected') == null) {
                isValid = false;
                $('#<%=this.ComboBudgets.ClientID %>_SpanBudgets').addClass("entryError");
                alertify.error(SwarmopsJS.unescape('<%= this.Localized_ValidationError_Budget %>'));
            }

            isValid = validateTextField('#<%=this.TextPurpose.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_Purpose %>')) && isValid;

            var jsonData = {};
            jsonData.amount = <%=this.CurrencyAmount.ClientID %>_val();

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
                        $('#<%=this.CurrencyAmount.ClientID %>_TextInput').addClass("entryError");
                        alertify.error(SwarmopsJS.unescape('<%= this.Localized_ValidationError_Amount %>'));
                        <%=this.CurrencyAmount.ClientID %>_focus();
                    }
                }
            });

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
                        $('#TextAmount').addClass("entryError");
                        alertify.error(SwarmopsJS.unescape('<%= this.Localized_ValidationError_Documents %>'));
                    }
                }
            });

            return isValid;
        }
        
        function validateTextField (fieldId, message) {
            if ($(fieldId).val().length == 0) {
                alertify.error(message);
                $(fieldId).addClass("entryError");
                $(fieldId).focus();
                return false;
            }

            return true;
        }

        var vatEnable = <%= this.CurrentOrganization.VatEnabled? "true" : "false" %>;

    </script>
    
    <style type="text/css">
        .enableVatField { display: none; }
    </style>
</asp:Content>


<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="easyui-tabs" id="divTabs" data-options="tabWidth:70,tabHeight:70">
        <div title="<img src='/Images/Icons/iconshock-invoice-256px.png' width='56' height='56' style='padding-top:4px'>">
            <h2><asp:Label runat="server" ID="BoxTitle" /></h2>
            <asp:HiddenField ID="HiddenTagSetIdentifiers" runat="server"/>
            <div class="entryFields">
                <Swarmops5:Currency runat="server" ID="CurrencyAmount" />
                <span class="enableVatField"><Swarmops5:Currency runat="server" ID="CurrencyVat" /></span>
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextPurpose" /></div>
                <Swarmops5:ComboBudgets ID="ComboBudgets" runat="server" />
                <asp:Repeater ID="RepeaterTagDrop" runat="server"><ItemTemplate><span id="SpanDropTags<%# Eval("TagSetId") %>"><select class="easyui-combotree" url="/Automation/Json-TransactionTagsTree.aspx?TagSetId=<%# Eval("TagSetId") %>" name="DropTags<%# Eval("TagSetId") %>" id="DropTags<%# Eval("TagSetId") %>" animate="true" style="width:300px"></select></span>&nbsp;<br/></ItemTemplate></asp:Repeater>

                <div class="stacked-input-control"></div><!-- placeholder for label-side H2 -->
        
                <!-- file upload begins here -->
        
                <Swarmops5:FileUpload ID="FileUpload" runat="server" Filter="ImagesOnly" />

                <!-- file upload ends -->

                <div class="stacked-input-control"></div><!-- placeholder for label-side H2 -->
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextBank" />&#8203;<br/></div>
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextClearing" />&#8203;<br/></div>
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextAccount" />&nbsp;<br/></div>
                <asp:Button ID="ButtonRequest" runat="server" CssClass="buttonAccentColor NoInputFocus" OnClientClick="return validateFields();" OnClick="ButtonRequest_Click" Text="Request"/>
            </div>
            <div class="entryLabels">
                <asp:Label runat="server" ID="LabelAmount" /><br/>
                <span class="enableVatField"><asp:Label runat="server" ID="LabelVat"/><br/></span>
                <asp:Label runat="server" ID="LabelPurpose" /><br/>
                <asp:Label runat="server" ID="LabelBudget" /><br/>
                <asp:Repeater ID="RepeaterTagLabels" runat="server"><ItemTemplate><%# Eval("TagSetLocalizedName") %><br/></ItemTemplate></asp:Repeater>
                <h2><asp:Label runat="server" ID="LabelHeaderImageFiles" /></h2>
                <asp:Label runat="server" ID="LabelImageFiles" /><br/>
                <h2><asp:Label runat="server" ID="LabelHeaderBankDetails" /></h2>
                <asp:Label runat="server" ID="LabelBankName" /><br/>
                <asp:Label runat="server" ID="LabelBankClearing" /><br/>
                <asp:Label runat="server" ID="LabelBankAccount" />
            </div>
            <div style="clear:both"></div>
        </div>
        <div title="<img src='/Images/Icons/expensify-icon-official.png' width='40' height='40' style='padding-top:12px'>">
            <h2><asp:Label runat="server" ID="LabelExpensifyUploadHeader" /></h2>
            
            <div id="divExpensifyInstructions">
                <p><asp:Label runat="server" ID="LabelExpensifyInstructions1"/></p>
                <p><asp:Label runat="server" ID="LabelExpensifyInstructions2"/></p>
            </div>

            <div id="DivUploadExpensify">
                <div id="divExpensifyUploadAnotherHeader"><h2><asp:Label runat="server" ID="LabelUploadAnotherFileHeader" /></h2></div>
                <div id="DivPrepData">
        
                    <div class="entryFields">
                        <Swarmops5:FileUpload runat="server" ID="UploadExpensify" Filter="NoFilter" DisplayCount="8" HideTrigger="true" ClientUploadCompleteCallback="uploadCompletedCallback" />
                        <asp:Button ID="ButtonExpensifyUpload" runat="server" CssClass="buttonAccentColor NoInputFocus" OnClientClick="alert('click'); return false;" Text="Request"/>
                    </div>
                    <div class="entryLabels">
                        <asp:Label runat="server" ID="LabelExpensifyCsv" />
                    </div>
                </div>
    
                <br clear="all"/>
            </div>
    
            <div id="DivProcessing" style="display:none">
                <h2><asp:Label runat="server" ID="LabelProcessing" /></h2>
                <div id="DivProgressProcessing" style="width:100%"></div>
            </div>

        </div>
    </div>
</asp:Content>



<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

