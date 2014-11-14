<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="ListFindPeople.aspx.cs" Inherits="Swarmops.Frontend.Pages.Swarm.ListFindPeople" %>
<%@ Register src="~/Controls/v5/Base/ComboGeographies.ascx" tagname="ComboGeographies" tagprefix="Swarmops5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
   
    <script type="text/javascript"> 
    
        function preload(arrayOfImages) {
            $(arrayOfImages).each(function () {
                (new Image()).src = this;
            });
        }

        preload([
            '/Images/Abstract/ajaxloader-medium.gif',
            '/Images/Icons/iconshock-balloon-yes-16px-hot.png',
            '/Images/Icons/iconshock-balloon-no-16px-hot.png',
            '/Images/Icons/iconshock-greentick-16px.png',
            '/Images/Icons/iconshock-redcross-16px.png',
            '/Images/Icons/undo-16px.png'
        ]);

        $(document).ready(function () {

            $('#<%=this.TextNamePattern.ClientID%>').on('fieldchange', function () {
                var newNamePattern = $('#<%=this.TextNamePattern.ClientID%>').val();
                if (newNamePattern != lastNamePattern && newNamePattern.length >= 3) {
                    lastNamePattern = newNamePattern;
                    $('#TableSearchResults').datagrid({ url: 'Json-ListFindPeople.aspx?Pattern=' + escape(newNamePattern) + '&GeographyId=' + selectedGeographyId });
                } else if (newNamePattern.length < 3 && newNamePattern != lastNamePattern && selectedGeographyId != 1) {
                    $('#TableSearchResults').datagrid({ url: 'Json-ListFindPeople.aspx?Pattern=&GeographyId=' + selectedGeographyId });
                    lastNamePattern = '';
                } else { // empty pattern and "world"
                    $('#TableSearchResults').datagrid('loadData', []);
                    lastNamePattern = '';
                }
            });

            $('#<%=this.TextNamePattern.ClientID%>').on('input', function () {
                clearTimeout(this.delayer);

                var context = this;
                this.delayer = setTimeout(function () {
                    $(context).trigger('fieldchange');
                }, 1000);
            });


            $('#TableSearchResults').datagrid(
                {
                    onLoadSuccess: function () {

                        var rowCount = $(this).datagrid('getRows').length;
                        $("#spanHitCount").text(rowCount);
                        if (rowCount == 1000) {
                            alertify.log("<%= Resources.Pages.People.ListFindPeople_TooManyHits %>");
                        }

                        // Leaving some remnant code from PayOutMoney in here for now, as similar code will be needed
                        // when actions are enabled on the search results

                        $(".LocalIconApproval").attr("src", "/Images/Icons/iconshock-balloon-yes-16px.png");
                        $(".LocalIconApproved").attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                        $(".LocalIconApproved").css("display", "none");
                        $(".LocalIconApproval, .LocalIconApproved").css("cursor", "pointer");

                        $(".LocalIconApproval").mouseover(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-16px-hot.png");
                            }
                        });

                        $(".LocalIconApproval").mouseout(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-16px.png");
                            }
                        });

                        $(".LocalIconApproved").mouseover(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/undo-16px.png");
                            }
                        });

                        $(".LocalIconApproved").mouseout(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                            }
                        });

                        $(".LocalIconApproval").click(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-medium.gif");
                                $("#IconDenial" + $(this).attr("baseid").replace(/\|/g,'')).fadeTo(1000, 0.01).css("cursor", "default");
                                var thisIcon = this;
                                $.ajax({
                                    type: "POST",
                                    url: "PayOutMoney.aspx/ConfirmPayout",
                                    data: "{'protoIdentity': '" + escape($(this).attr("baseid")) + "'}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (msg) {
                                        $(thisIcon).attr("databaseid", msg.d.AssignedId);
                                        $(thisIcon).css("display", "none");
                                        $(thisIcon).attr("src", "/Images/Icons/iconshock-balloon-yes-16px.png");
                                        $(thisIcon).attr("rel", "active");
                                        $("#IconApproved" + $(thisIcon).attr("baseid").replace(/\|/g, '')).fadeIn(100);
                                        alertify.success(unescape(msg.d.DisplayMessage));
                                    }
                                });
                            }
                        });

                        $(".LocalIconApproved").click(function () {
                            if ($(this).attr("rel") != "loading") {
                                var thisIcon = this;
                                
                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-medium.gif");
                                $.ajax({
                                    type: "POST",
                                    url: "PayOutMoney.aspx/UndoPayout",
                                    data: "{'databaseId': '" + $("#IconApproval" + $(thisIcon).attr("baseid").replace(/\|/g, '')).attr("databaseid") + "'}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (msg) {
                                        if (msg.d.Success) {
                                            $(thisIcon).css("display", "none");
                                            $(thisIcon).attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                                            $(thisIcon).attr("rel", "");
                                            $("#IconApproval" + $(thisIcon).attr("baseid").replace(/\|/g, '')).fadeIn(100);
                                            $("#" + $(thisIcon).attr("rel"), "");
                                            alertify.log(unescape(msg.d.DisplayMessage));
                                        } else {
                                            $(thisIcon).attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                                            alertify.error(unescape(msg.d.DisplayMessage));
                                            // TODO: Add alert box?
                                        }
                                    }
                                });

                            }

                            // end of remnant code from PayOutMoney
                        });

                    }
                }
            );

            $('#<%=this.TextNamePattern.ClientID%>').focus();
        });

        function onGeographyChange(newGeographyId) {
            if (newGeographyId != selectedGeographyId) {
                selectedGeographyId = newGeographyId;
                if (selectedGeographyId != 1 || lastNamePattern.length > 2) { // do not allow carte-blance listing for "world"
                    $('#TableSearchResults').datagrid({ url: 'Json-ListFindPeople.aspx?Pattern=' + escape(lastNamePattern) + '&GeographyId=' + selectedGeographyId });
                } else {
                    $('#TableSearchResults').datagrid('loadData', []);
                }
            }
        }

        var lastNamePattern = '';
        var selectedGeographyId = 1;  // TODO: SET ROOT GEOGRAPHY BY AUTHORITY/ACCESS

    </script>

    <style type="text/css">
        .datagrid-row-selected,.datagrid-row-over{
            background:transparent;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="entryFields" style="padding-top:4px">
        <Swarmops5:ComboGeographies ID="ComboGeographies" runat="server" OnClientSelect="onGeographyChange" />&thinsp;<br/>
        <asp:TextBox runat="server" ID="TextNamePattern" />
    </div>
    <div class="entryLabels" style="padding-top:10px">
        <asp:Label ID="LabelGeography" runat="server" /><br/>
        <asp:Label ID="LabelNamePattern" runat="server" /><br/>
    </div>
    <h2 style="padding-top:15px"><asp:Label ID="LabelMatchingPeopleInX" runat="server" /> (<span id="spanHitCount">0</span>)</h2>
    <table id="TableSearchResults" class="easyui-datagrid" style="width:680px;height:500px"
        data-options="rownumbers:false,singleSelect:false,nowrap:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true"
        idField="itemId">
        <thead>
            <tr>
                <th data-options="field:'name',width:210"><asp:Label ID="LabelGridHeaderName" runat="server" Text="XYZ Name"/></th>  
                <th data-options="field:'geographyName',width:150,sortable:true"><asp:Label ID="LabelGridHeaderGeography" runat="server" Text="XYZ Geography" /></th>
                <th data-options="field:'mail',width:105,sortable:true"><asp:Label ID="LabelGridHeaderMail" runat="server" Text="XYZ Mail" /></th>  
                <th data-options="field:'phone',width:100,sortable:true"><asp:Label ID="LabelGridHeaderPhone" runat="server" Text="XYZ Phone" /></th>
                <th data-options="field:'notes',width:50"><asp:Label ID="LabelGridHeaderNotes" runat="server" Text="XYZ Notes" /></th>
                <th data-options="field:'actions',width:43,align:'center'"><asp:Label ID="LabelGridHeaderAction" runat="server" Text="XYZ Actions" /></th>
            </tr>  
        </thead>
    </table>  
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

