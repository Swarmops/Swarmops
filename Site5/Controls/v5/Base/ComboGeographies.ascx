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
 </script>
 
 <span id="<%=this.ClientID %>_SpanGeographies"><select class="easyui-combotree" name="DropGeographies" id="<%=this.ClientID %>_DropGeographies" animate="true" style="width:300px"></select></span>