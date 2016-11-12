<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="PersonFrame.aspx.cs" Inherits="Pages_v4_v3_Members_PersonFrame" meta:resourcekey="PageResource1" %>

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
            position: relative;
            width: 100%;
            height: 100%;
        }
        #viewport
        {
            position: relative;
            left: -100px;
            width: 1px;
            height: 1px;
            overflow: hidden;
        }
        html, body
        {
            margin-bottom: 0px !important;
            margin-right: 0px !important;
        }
    </style>

    <script src="/jQuery/js/jquery.cookie.js" type="text/javascript"></script>

    <script type="text/javascript">

        function onUnload() {
            $('#viewport')
                .css("left", -10010)
        }


        function doResize() {
            var h = $(window).height();
            var w = $(window).width();
            var vpW = w - 12
            var vpH = h - 2
            var tabStripHeight = $('#tabs ul').height();
            $('#viewport')
                .css("left", 10)
                .css("height", vpH)
                .css("width", vpW);
            $('#tabs')
                .css("height", vpH - 8)
                .css("width", vpW - 2);
            $('.tabFrame')
                .css("height", $('#tabs').height() - tabStripHeight - 20)
                .css("width", $('#tabs').width() - $('#tabs ul').position().left - 4);
            $('iframe')
               .css("height", '100%')
               .css("width", '100%');
        }

    </script>

    <script type="text/javascript">
       var urls = [<%= urlArray %>];
    </script>

    <script type="text/javascript">
        var framesCollection = [];

        $(function() {
            $("#tabs").tabs({
                selected: 1 * $.cookie('pwPersonFrame_selectedTab'),
                select: function(event, ui) {

                    $.cookie('pwPersonFrame_selectedTab', '' + ui.index);
                    var frame = document.getElementById("Iframe" + ui.index);
                    var frameD = frame.contentDocument;
                    var frameW = frame.contenWindow;
                    if (typeof frameW != "undefined") frameD = frameW;
                    if (typeof document.frames != "undefined") frameD = document.frames['frame' + ui.index];

                    if (frameD.location.href.indexOf("empty.htm") >= 0) {
                        frameD.location.href = urls[ui.index] + "?id=<%= currentId %>";
                    }
                    return true;
                }
            });

            doResize();
            setTimeout(doResize, 500);
        });

        $(window).resize(doResize);

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div id="viewport">
        <asp:Label ID="LabelSelectedMember" runat="server" Text=""></asp:Label>
        <div id="tabs" style="position: relative;" runat="server">
        </div>
    </div>
</asp:Content>
