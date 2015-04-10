<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ComboPeople.ascx.cs" Inherits="Swarmops.Controls.Financial.ComboPeople" %>
<%@ Import Namespace="System.Threading" %>

<% string _nearEdge = Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft ? "right" : "left"; %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=this.ClientID %>_DropPeople').combobox({
            animate: true,
            height: 32,
            panelWidth: 300,
            panelAlign: '<%= _nearEdge %>',
            loader: <%=this.ClientID %>_autoCompleteLoader,
            mode: 'remote',
            valueField: 'id',
            textField: 'name',
            formatter: <%=this.ClientID %>_formatItem,
            
            onSelect: function (person) {

                // update avatar when person selected

                SwarmopsJS.ajaxCall(
                    "/Automation/SwarmFunctions.aspx/GetPersonAvatar",
                    { personId: person.id },
                    function(data) {
                        $('span#<%= this.ClientID %>_SpanPeople span input.textbox-text').css('background-image', "url('" + data.Avatar24Url + "')");
                    });

                <%=this.ClientID%>_selectedPersonIdPrivate = person.id;

                // call owner, if we have a callback

                <% if (!String.IsNullOrEmpty(this.OnClientSelect)) {
                    Response.Write (this.OnClientSelect + "(person.id);");
                }  %>

            }
        });

        <%=this.ClientID%>_placeholder(decodeURIComponent("<%=JavascriptEscape (this.Placeholder)%>"));

        <% if (this.Selected != null)
           { %>;
        $('span#<%= this.ClientID %>_SpanPeople span input.textbox-text').css('background-image', "url('<%= this.Selected.GetSecureAvatarLink(24) %>')");
        $('span#<%= this.ClientID %>_SpanPeople span input.textbox-text').text('<%= this.Selected.Canonical %>');
        
        <% }
           else if (this.NobodySelected)
           { %>;
        $('span#<%= this.ClientID %>_SpanPeople span input.textbox-text').css('background-image', "url('/Images/Icons/iconshock-warning-24px.png')");
        $('span#<%= this.ClientID %>_SpanPeople span input.textbox-text').text('<%= Resources.Global.Global_NoOwner %>');
        
        <% } else { %>;
        $('span#<%= this.ClientID %>_SpanPeople span input.textbox-text').css('background', '');

        <% } %>
    });

    var <%=this.ClientID%>_selectedPersonIdPrivate = 0;

    var <%=this.ClientID %>_autoCompleteLoader = function(param,success,error){
        var q = param.q || '';
        if (q.length < 3) {
            success (new Array());
            return false;
        }
        $.ajax({
            url: '/Automation/Json-SearchPeoplePattern.aspx',
            dataType: 'json',
            data: {
                maxRows: 20,
                namePattern: q
            },
            success: function(data){
                success(data);
            },
            error: function(){
                error.apply(this, arguments);
            }
        });
        return false;
    };
    
    function <%=this.ClientID %>_formatItem(row)
    {
        var s = '<span style="padding-<%= _nearEdge %>:20px;margin-<%= _nearEdge %>:4px;' + "background-image:url('" + row.avatar16Url + "');" + 'background-position:<%= _nearEdge %> center;background-repeat:no-repeat">' + row.name + '</span>';
        return s;
    }

    function <%=this.ClientID %>_placeholder(newValue) {
        $('span#<%= this.ClientID %>_SpanPeople span input.textbox-text').attr('placeholder', newValue);
    }

    function <%=this.ClientID %>_avatarUrl(avatarUrl) {
        $('span#<%= this.ClientID %>_SpanPeople span input.textbox-text').css('background-image', "url('" + avatarUrl + "')");
    }

    function <%=this.ClientID %>_selectedPersonId() {
        return <%=this.ClientID%>_selectedPersonIdPrivate;
    }

    function <%=this.ClientID %>_placeholder(newValue) {
        $('span#<%= this.ClientID %>_SpanPeople span input.textbox-text').attr('placeholder', newValue);
    }

    function <%=this.ClientID %>_val(newValue) {
        return $('span#<%= this.ClientID %>_SpanPeople span input.textbox-text').val(newValue);
    }

    function <%=this.ClientID %>_backgroundColor(newColor) {
        $('span#<%= this.ClientID %>_SpanPeople span input.textbox-text').css('background-color', newColor);
    }

    function <%=this.ClientID %>_backgroundToWhite() {
        $('span#<%= this.ClientID %>_SpanPeople span input.textbox-text').animate({ backgroundColor: "#FFFFFF" }, 250);
    }


</script>
 
 <span id="<%=this.ClientID %>_SpanPeople" class="fakePlaceholderText"><select class="easyui-combobox comboperson" url="" name="DropPeople" id="<%=this.ClientID %>_DropPeople" animate="true" style="width:324px"></select></span>