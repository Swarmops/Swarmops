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
                },
                loader: function (param, success, error) {
                    if (!param.id) {
                        $.getJSON("/Automation/Json-GeographiesTree.aspx?ParentGeographyId=<%=this.ParentGeographyId%>", null, success);
                    } else {
                        $.getJSON("/Automation/Json-GeographiesTree.aspx?InitialExpand=false&ParentGeographyId=" + param.id, null, success);
                    }
                },
            onSelect: function (row) {
                    <% 
                           if (!String.IsNullOrEmpty(this.OnClientSelect))
                           {
                               Response.Write(this.OnClientSelect + "(row.id);");
                           }
                           
                           %>
            },
            formatter: function (row) {
                // If we're at a country node, add that country's flag ahead of the country name. Replaces the folder icon (that part is done in CSS).

                if (row.countryId != null) {
                    return "<img src='/Images/Flags/" + row.countryId + "-24px.png' style='width:16px;height:18px;vertical-align:bottom' /> " + row.text;
                }
                else return row.text;
            }
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
 
 <span id="<%=this.ClientID %>_SpanGeographies"><select class="easyui-combotree" name="DropGeographies" id="<%=this.ClientID %>_DropGeographies" animate="true" style="width:300px"></select></span>