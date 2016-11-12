<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="SearchFrame.aspx.cs" Inherits="Pages_v4_v3_Members_SearchFrame" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        .tabFrame
        {
            position: relative;
            padding: 2px 2px 2px 2px !important;
            margin: 0px 0px 0px 0px !important;
        }
        .tabFrame iframe
        {
            margin-top: 2px;
            position: relative;
            width: 100%;
            height: 100%;
        }
        #viewport
        {
            position: relative;
            left: 10px;
            overflow: hidden;
        }
        html, body
        {
            margin-bottom: 0px !important;
            margin-right: 0px !important;
        }
    </style>

    <script type="text/javascript">

        $(window).ready(function() {
            $("#tabs").tabs();
            doResize();
            setTimeout(doResize, 500);
            frames.listFrame.location = "List.aspx";
            frames.searchFrame.location = "Search.aspx";
        });
        $("#tabs").tabs({
            select: function(event, ui) {
                doResize();
            }
        });

        $(window).resize(doResize);

        function doResize() {
            var h = $(window).height();
            var w = $(window).width();
            var vpW = w - 20
            var vpH = h - 64
            var tabStripHeight = $('#tabs>ul').height();
            $('#viewport')
                .css("height", vpH)
                .css("width", vpW);
            $('#tabs')
                .css("height", vpH - 8)
                .css("width", vpW - 2);
            $('.tabFrame')
                .css("height", $('#tabs').height() - tabStripHeight - 6)
                .css("width", $('#tabs').width() - $('#tabs>ul').position().left - 4);
            $('iframe')
               .css("height", '100%')
                .css("width", '100%');
        }


    </script>

    <script type="text/javascript">
        function LoadMember(memberId) {
            if (typeof frames.searchResult.onUnload == "function")
                frames.searchResult.onUnload();

            frames.searchResult.location = "PersonFrame.aspx?id=" + memberId;
            $("#tabs").tabs("select", 2)
        }


    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div id="viewport">
        <div id="tabs" style="position: relative;">
            <ul>
                <li><a href="#tabs-1">
                    <asp:Literal ID="Literal1" runat="server" meta:resourcekey="Literal1Resource1" 
                        Text="Search"></asp:Literal>
                </a></li>
                <li><a href="#tabs-2">
                    <asp:Literal ID="Literal2" runat="server" meta:resourcekey="Literal2Resource1" 
                        Text="List"></asp:Literal>
                </a></li>
                <li><a href="#tabs-3">
                    <asp:Literal ID="Literal3" runat="server" meta:resourcekey="Literal3Resource1" 
                        Text="Selected person"></asp:Literal>
                </a></li>
            </ul>
            <div id="tabs-1" class="tabFrame">
                <iframe name="searchFrame" id="searchFrame" src="empty.htm" frameborder="0"></iframe>
            </div>
            <div id="tabs-2" class="tabFrame">
                <iframe name="listFrame" id="listFrame" src="empty.htm" frameborder="0"></iframe>
            </div>
            <div id="tabs-3" class="tabFrame">
                <iframe name="searchResult" id="searchResult" src="empty.htm" frameborder="0"></iframe>
            </div>
        </div>
    </div>
</asp:Content>
