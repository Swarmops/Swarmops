<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Financial.FileExpenseClaim" CodeFile="FileExpenseClaim.aspx.cs" Codebehind="FileExpenseClaim.aspx.cs" %>
<%@ Register src="~/Controls/v5/Base/FileUpload.ascx" tagname="FileUpload" tagprefix="Swarmops5" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="Currency" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ScriptFancyBox" Package="FancyBox" runat="server"/>
    
    <style type="text/css">

        .fancybox-arrow-previous,
        .fancybox-arrow-next {
          position: absolute;
          width: 44px;
          height: 44px;
          background: #000;
          text-align: center;
          line-height: 44px;
          color: #fff;
          text-decoration: none;
          border-radius: 50%;
          font-size: 16px;
          top: 50%;
          margin-top: -22px;
          line-height: 42px;
        }

        .fancybox-arrow-previous {
            left: -50px;
        }

        .fancybox-arrow-next {
            right: -50px;
        }        

    </style>

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
        var expensifyProcessingHalfway = false;

        function onExpensifyUpload() {
            SwarmopsJS.ajaxCall
                ("/Pages/v5/Financial/FileExpenseClaim.aspx/InitializeExpensifyProcessing",
                {
                    guidFiles: '<%= this.UploadExpensify.GuidString%>',
                    guidProgress: '<%= this.ProgressExpensify.Guid%>'
                },
                function(result) {
                    if (result.Success) {
                        <%=this.ProgressExpensify.ClientID%>_fadeIn();
                        <%=this.ProgressExpensify.ClientID%>_begin();  // starts listening / polling for progress
                    }
                });
            
        }

        function onExpensifyProgressHalfway() {
            $('#divUploadExpensify').slideUp().fadeOut(); // hide the upload panel
        }

        function onExpensifyProgressComplete() {
            // Get results

            $('#divExpensifyUploadHeader').hide(); // this should be hidden at this time regardless of result

            SwarmopsJS.ajaxCall('/Pages/v5/Financial/FileExpenseClaim.aspx/GetExpensifyUploadResult',
                { guid: '<%=this.UploadExpensify.GuidString%>' },
                function(result) {
                    if (result.Success) {

                        // Make a neat transition to success view

                        $('#divExpensifyResultsBad').hide();
                        $('#divExpensifyResultsGood').show();
                        <%=this.ProgressExpensify.ClientID%>_hide();
                        <%=this.ProgressExpensify.ClientID%>_reset();
                        <%=this.ProgressExpensifyFake.ClientID%>_show();
                        $('#divUploadExpensify').hide();
                        $('#divExpensifyUploadAnotherHeader').show();
                        $('#divExpensifyResults').slideDown();

                        // Display processed data

                        $('#expensifyDataGrid').datagrid({});
                        $('#expensifyDataGrid').datagrid('loadData', result.Data);

                        // Fill in documents (hidden)



                        // Enable document viewing

                        $('#divDocumentsHidden').html(result.Documents);

                        $("a.FancyBox_Gallery").fancybox({
                            toolbar: false,
                            smallBtn: true,
                            arrows: false,
                            infobar: false,

                            afterShow: function() {
                                $('.zoomContainer').remove();
                                $('.fancybox-image').elevateZoom({
                                    zoomType: "lens",
                                    cursor: "crosshair",
                                    zoomWindowFadeIn: 200,
                                    zoomWindowFadeOut: 200,
                                    lensShape: "round",
                                    lensSize: 200
                                });
                            },

                            afterLoad: function() {

                                /* TODO: MAKE A RIGHT-TO-LEFT VERSION OF THIS */

                                if ( instance.group.length > 1 && current.$content ) {
                                    current.$content.append('<a data-fancybox-next class="fancybox-arrow-next" href="javascript:;">→</a><a data-fancybox-previous class="fancybox-arrow-previous" href="javascript:;">←</a>');
                                }                            
                            },

                            afterClose: function() {
                                $('.zoomContainer').remove();
                            }
                        });

                        $(".LocalIconViewDoc").click(function () {
                            $("a.FancyBox_Gallery[data-fancybox='" + $(this).attr("firstDocId") + "']").first().click();
                        });


                    } else {

                        // Make a brutal transition to failure view

                        <%=this.ProgressExpensify.ClientID%>_fadeOut();
                        <%=this.ProgressExpensify.ClientID%>_hide(); // both fadeOut + hide necessary
                        <%=this.ProgressExpensify.ClientID%>_reset();
                        <%=this.ProgressExpensifyFake.ClientID%>_hide();

                        $('#divExpensifyResultsBad').show();
                        $('#divExpensifyResultsGood').hide();
                        $('#divExpensifyResults').show();
                        $('#divExpensifyInstructions').hide();
                        $('#divExpensifyUploadAnotherHeader').show();
                        $('#divExpensifyResultsBadText').html(result.DisplayMessage);

                        $('#divUploadExpensify').fadeIn().slideDown(); // aborts the slideUp probably in progress
                    }

                    // Regardless of whether result is good or bad, reset the upload control

                    <%=this.UploadExpensify.ClientID%>_clear();
                });


        }


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
            
            <Swarmops5:ProgressBarFake ID="ProgressExpensifyFake" runat="server"/>

            <div id="divExpensifyResults" style="display:none; margin-bottom:10px">
               
                <h2><asp:Label ID="LabelExpensifyProcessingComplete" runat="server" /></h2>

                <div id="divExpensifyResultsGood" style="display:none">
                    <table id="expensifyDataGrid" class="easyui-datagrid" style="width:680px"
                        data-options="rownumbers:false,singleSelect:false,nowrap:false,fitColumns:true,fit:false,showFooter:true,loading:false,selectOnCheck:true,checkOnSelect:true"
                        idField="expenseId">
                        <thead>  
                            <tr>
                                <th data-options="field:'Budget',width:200"><asp:Label ID="LabelExpensifyHeaderBudget" runat="server" Text="XYZ Budget" /></th>  
                                <th data-options="field:'CreatedDateTime',width:50"><asp:Label ID="LabelExpensifyHeaderDate" runat="server" Text="XYZ Created" /></th>
                                <th data-options="field:'Description',width:200"><asp:Label ID="LabelExpensifyHeaderDescription" runat="server" Text="XYZ Description" /></th>
                                <th data-options="field:'AmountVat',width:80,align:'right',hidden:<%=(!CurrentOrganization.VatEnabled).ToString().ToLowerInvariant() %>"><asp:Label ID="LabelExpensifyHeaderVat" runat="server" Text="XYZ Vat" /></th>
                                <th data-options="field:'Amount',width:80,align:'right'"><asp:Label ID="LabelExpensifyHeaderAmount" runat="server" Text="XYZ Amount" /></th>
                                <th data-options="field:'Actions',width:60,align:'center'"><asp:Label ID="LabelExpensifyHeaderDocs" runat="server" Text="XYZAct" /></th>
                            </tr>  
                        </thead>
                    </table>  
                    

                </div>

                <div id="divExpensifyResultsBad" style="display:none">
                    <div style="float:left;margin-right:10px"><img src="/Images/Icons/iconshock-cross-96px.png" /></div><div id="divExpensifyResultsBadText"></div>
                </div>

                <br clear="all"/>
            </div>

            
            <div id="divUploadExpensify">
                <div id="divExpensifyInstructions">
                    <h2><asp:Label runat="server" ID="LabelExpensifyUploadHeader" /></h2>

                    <p><asp:Label runat="server" ID="LabelExpensifyInstructions1"/></p>
                    <p><asp:Label runat="server" ID="LabelExpensifyInstructions2"/></p>
                </div>

                <div id="divExpensifyUploadAnotherHeader" style="display:none"><h2><asp:Label runat="server" ID="LabelExpensifyUploadAnotherHeader" Text="Upload Another XYZ" /></h2></div>
        
                <div class="entryFields">
                    <Swarmops5:FileUpload runat="server" ID="UploadExpensify" Filter="NoFilter" DisplayCount="8" ClientUploadCompleteCallback="onExpensifyUpload" />
                </div>
                <div class="entryLabels">
                    <div class="stacked-input-control"><asp:Label runat="server" ID="LabelExpensifyCsv" /></div>
                </div>
    
                <br clear="all"/>
            </div>

            <Swarmops5:ProgressBar ID="ProgressExpensify" runat="server" OnClientProgressHalfwayCallback="onExpensifyProgressHalfway" OnClientProgressCompleteCallback="onExpensifyProgressComplete"/>
            
            <div id="divDocumentsHidden"></div>

        </div>
    </div>
</asp:Content>



<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

