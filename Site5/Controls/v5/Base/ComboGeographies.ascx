<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ComboGeographies.ascx.cs" Inherits="Swarmops.Controls.Base.ComboGeographies" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=this.ClientID %>_DropGeographies').combotree({
                animate: true,
                height: 30,
                onLoadSuccess: function() {
                    $('#<%=this.ClientID %>_DropGeographies').combotree('setText', "<%=this.RootGeographyName%>");
                    <%
                           if (!String.IsNullOrEmpty(this.OnClientLoaded))
                           {
                                Response.Write(this.OnClientLoaded + "(); ");
                           }%>
                }<% 
                           if (!String.IsNullOrEmpty(this.OnClientSelect))
                           {
                               Response.Write(", onSelect: function(account) { " + this.OnClientSelect + "(account.id); }");
                           }
                           
                           %>
            });

        $('#<%=this.ClientID %>_SpanGeographies span.combo input.combo-text').click(function () {
            $('#<%=this.ClientID %>_SpanGeographies span.combo span span.combo-arrow').click();
        });

        $('#<%=this.ClientID %>_SpanGeographies span.combo input.combo-text').keydown(function (e) {
            switch (e.which) {
                case 40: // down
                    $('#<%=this.ClientID %>_SpanGeographies span.combo span span.combo-arrow').click();
                    break;

                default: return; // exit this handler for other keys
            }
            e.preventDefault(); // prevent the default action (scroll / move caret)
        });
    });
 </script>
 
 <span id="<%=this.ClientID %>_SpanGeographies"><select class="easyui-combotree" url="/Automation/Json-GeographiesTree.aspx?RootGeographyId=<%=this.RootGeographyId %>" name="DropGeographies" id="<%=this.ClientID %>_DropGeographies" animate="true" style="width:300px"></select></span>