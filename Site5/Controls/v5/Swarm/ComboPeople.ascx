<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ComboPeople.ascx.cs" Inherits="Swarmops.Controls.Financial.ComboPeople" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=this.ClientID %>_DropPeople').combobox({
            animate: true,
            height: 30
        });

        <% if (this.Selected != null)
           { %>

        $('span#<%= this.ClientID %>_SpanPeople span input.combo-text').css('background', "url('<%= this.Selected.GetSecureAvatarLink(24) %>')");
        $('span#<%= this.ClientID %>_SpanPeople span input.combo-text').text('<%= this.Selected.Canonical %>');
        
        <% }
           else if (this.NobodySelected)
           { %>

        $('span#<%= this.ClientID %>_SpanPeople span input.combo-text').css('background', "url('/Images/Icons/iconshock-warning-24px.png')");
        $('span#<%= this.ClientID %>_SpanPeople span input.combo-text').text('<%= Resources.Global.Global_NoOwner %>');
        
        <% } else { %>
        
        $('span#<%= this.ClientID %>_SpanPeople span input.combo-text').css('background', '');

        <% } %>

        $('#<%=this.ClientID %>_SpanPeople span.combo input.combo-text').keydown(function (e) {
            switch (e.which) {
                case 40: // down
                    $('#<%=this.ClientID %>_SpanPeople span.combo span span.combo-arrow').click();
                    break;

                default: return; // exit this handler for other keys
            }
            e.preventDefault(); // prevent the default action (scroll / move caret)
        });
    });
 </script>
 
 <span id="<%=this.ClientID %>_SpanPeople"><select class="easyui-combobox comboperson" url="/Automation/Json-ExpensableBudgetsTree.aspx" name="DropPeople" id="<%=this.ClientID %>_DropPeople" animate="true" style="width:300px"></select></span>