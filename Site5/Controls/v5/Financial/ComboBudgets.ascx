<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ComboBudgets.ascx.cs" Inherits="Swarmops.Controls.Financial.ComboBudgets" %>
<%@ Import Namespace="System.Threading" %>

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
                        Response.Write ("$('#" + this.ClientID + "_SpanBudgets span.combo input.textbox-text').val(\"" + Resources.Global.Global_DropInits_SelectFinancialAccount + "\");");
                    }
                    if (!String.IsNullOrEmpty(this.OnClientLoaded)) 
                    {
                        Response.Write(this.OnClientLoaded + "();");
                    }%>
                }
                <%
                           if (!String.IsNullOrEmpty(this.OnClientSelect))
                           {
                               Response.Write(", onSelect: function(account) { " + this.OnClientSelect + "(account.id); }");
                           }
                           
                           %>
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

    // below code from http://doc.javake.cn/jeasyui/www.jeasyui.com/forum/index.php-topic=1972.0.htm, understood to be in public domain: enables keyboard navigation

    (function () {
        $.extend($.fn.combotree.methods, {
            nav: function (jq, dir) {
                return jq.each(function () {
                    var opts = $(this).combotree('options');
                    var t = $(this).combotree('tree');
                    var nodes = t.tree('getChildren');
                    if (!nodes.length) { return }
                    var node = t.tree('getSelected');
                    if (!node) {
                        t.tree('select', dir > 0 ? nodes[0].target : nodes[nodes.length - 1].target);
                    } else {
                        var index = 0;
                        for (var i = 0; i < nodes.length; i++) {
                            if (nodes[i].target == node.target) {
                                index = i;
                                break;
                            }
                        }
                        if (dir > 0) {
                            while (index < nodes.length - 1) {
                                index++;
                                if ($(nodes[index].target).is(':visible')) { break }
                            }
                        } else {
                            while (index > 0) {
                                index--;
                                if ($(nodes[index].target).is(':visible')) { break }
                            }
                        }
                        t.tree('select', nodes[index].target);
                    }
                    if (opts.selectOnNavigation) {
                        var node = t.tree('getSelected');
                        $(node.target).trigger('click');
                        $(this).combotree('showPanel');
                    }
                });
            }
        });
        $.extend($.fn.combotree.defaults.keyHandler, {
            up: function () {
                $(this).combotree('nav', -1);
            },
            down: function () {
                $(this).combotree('nav', 1);
            },
            enter: function () {
                var t = $(this).combotree('tree');
                var node = t.tree('getSelected');
                if (node) {
                    $(node.target).trigger('click');
                }
                $(this).combotree('hidePanel');
            }
        });
    })(jQuery);

 </script>
 
 <span id="<%=this.ClientID %>_SpanBudgets"><select class="easyui-combotree" url="<%=this.DataUrl %>" name="DropBudgets" id="<%=this.ClientID %>_DropBudgets" animate="true" style="width:324px"></select></span>