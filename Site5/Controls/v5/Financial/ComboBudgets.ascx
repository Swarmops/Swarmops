<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ComboBudgets.ascx.cs" Inherits="Swarmops.Controls.Financial.ComboBudgets" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=this.ClientID %>_DropBudgets').combotree({
                animate: true,
                height: 30 <%
                           if (!String.IsNullOrEmpty(this.OnClientLoaded))
                           {
                                Response.Write(", onLoadSuccess: function() { " + this.OnClientLoaded + "(); }");
                           }
                           if (!String.IsNullOrEmpty(this.OnClientSelect))
                           {
                               Response.Write(", onSelect: function(account) { " + this.OnClientSelect + "(account.id); }");
                           }
                           
                           %>
            });

        $('#<%=this.ClientID %>_SpanBudgets span.combo input.combo-text').click(function () {
            $('#<%=this.ClientID %>_SpanBudgets span.combo span span.combo-arrow').click();
        });

        $('#<%=this.ClientID %>_SpanBudgets span.combo input.combo-text').keydown(function (e) {
            switch (e.which) {
                case 40: // down
                    $('#<%=this.ClientID %>_SpanBudgets span.combo span span.combo-arrow').click();
                    break;

                default: return; // exit this handler for other keys
            }
            e.preventDefault(); // prevent the default action (scroll / move caret)
        });
    });
 </script>
 
 <span id="<%=this.ClientID %>_SpanBudgets"><select class="easyui-combotree" url="<%=this.DataUrl %>" name="DropBudgets" id="<%=this.ClientID %>_DropBudgets" animate="true" style="width:300px"></select></span>