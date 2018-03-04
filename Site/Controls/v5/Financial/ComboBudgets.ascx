<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComboBudgets.ascx.cs" Inherits="Swarmops.Controls.Financial.ComboBudgets" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="Swarmops.Common.Enums" %>
<%@ Import Namespace="Swarmops.Frontend" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%= this.ClientID %>_DropBudgets').combotree({
            animate: true,
            editable: false,
            height: 32,
            panelWidth: 300,
            panelAlign: '<%= Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft? "right": "left" %>',
            onLoadSuccess: function () {
                    <% 
                    if (!SuppressPrompt)
                    {
                        Response.Write ("$('#" + this.ClientID + "_SpanBudgets span.combo input.textbox-text').val(comboBudgetsDropInit);");
                    }
                    if (!String.IsNullOrEmpty(this.OnClientLoaded)) 
                    {
                        Response.Write(this.OnClientLoaded + "();");
                    }%>
            },
            onSelect: function(account) {
                <%
                           if (!String.IsNullOrEmpty(this.OnClientSelect))
                           {
                               Response.Write(this.OnClientSelect + "(account.id);");
                           }
                           
                           %>
            }
        });
        
        $('#<%=this.ClientID %>_SpanBudgets span.combo input.textbox-text').click(function () {
            $('#<%=this.ClientID %>_SpanBudgets span.combo span span.combo-arrow').click();
        });

        $('#<%=this.ClientID %>_SpanBudgets span.combo input.textbox-text').keydown(function (e) {
            switch (e.which) {
                case 40: // down
                    var panel = $('#<%=this.ClientID %>_DropBudgets').combotree('panel');
                    if (!panel.is(':visible')) {
                        $('#<%=this.ClientID %>_SpanBudgets span.combo span span.combo-arrow').click();
                        panel.focus();
                    }
                    break;

                default: return; // exit this handler for other keys
            }
            e.preventDefault(); // prevent the default action (scroll / move caret)
        });
    });

    var comboBudgetsDropInit = comboBudgetsDropInit || SwarmopsJS.unescape('<%=this.Localized_DropInit%>');  // declare or redeclare

    function <%=this.ClientID %>_val(newValue) {
        if (newValue === undefined)
        {
            // getter
            return $('#<%=this.ClientID %>_DropBudgets').combotree('getValue');
        } else {
            // setter

            if (newValue > 0) 
            {
                $('#<%=this.ClientID %>_DropBudgets').combotree('setValue', newValue);
            } else {
                $('#<%=this.ClientID %>_DropBudgets').combotree('setValue', 0);
                $('#<%=this.ClientID %>_SpanBudgets span.combo input.textbox-text').val(comboBudgetsDropInit);
            }
        }

    }

    




 </script>
 
 <% if (this.Layout == LayoutDirection.Vertical) { %><div class="stacked-input-control"><% } %>
     <span id="<%=this.ClientID %>_SpanBudgets"><select class="easyui-combotree" url="<%=this.DataUrl %>" name="DropBudgets" id="<%=this.ClientID %>_DropBudgets" animate="true" style="width:324px"></select></span>
 <% if (this.Layout == LayoutDirection.Vertical) { %></div><% } %>