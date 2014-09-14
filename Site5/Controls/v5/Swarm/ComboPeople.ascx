<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ComboPeople.ascx.cs" Inherits="Swarmops.Controls.Financial.ComboPeople" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=this.ClientID %>_DropPeople').combobox({
            animate: true,
            height: 30,
            loader: <%=this.ClientID %>_autoCompleteLoader,
            mode: 'remote',
            valueField: 'id',
            textField: 'name',
            formatter: <%=this.ClientID %>_formatItem,
            
            onSelect: function (person) {

                // update avatar when person selected

                $.ajax({
                    url: '/Automation/Json-GetPersonAvatar.aspx',
                    dataType: 'json',
                    data: {
                        personId: person.id
                    },
                    success: function(data){
                        $('span#<%= this.ClientID %>_SpanPeople span input.combo-text').css('background-image', "url('" + data.avatar24Url + "')");
                        
                    },
                    error: function(){
                        error.apply(this, arguments);
                    }
                });

                // call owner, if we have a callback

                <% if (!String.IsNullOrEmpty(this.OnClientSelect)) {
                    Response.Write (this.OnClientSelect + "(person.id);");
                }  %>

            }
        });

        <% if (this.Selected != null)
           { %>;
        $('span#<%= this.ClientID %>_SpanPeople span input.combo-text').css('background-image', "url('<%= this.Selected.GetSecureAvatarLink(24) %>')");
        $('span#<%= this.ClientID %>_SpanPeople span input.combo-text').text('<%= this.Selected.Canonical %>');
        
        <% }
           else if (this.NobodySelected)
           { %>;
        $('span#<%= this.ClientID %>_SpanPeople span input.combo-text').css('background-image', "url('/Images/Icons/iconshock-warning-24px.png')");
        $('span#<%= this.ClientID %>_SpanPeople span input.combo-text').text('<%= Resources.Global.Global_NoOwner %>');
        
        <% } else { %>;
        $('span#<%= this.ClientID %>_SpanPeople span input.combo-text').css('background', '');

        <% } %>;
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
        var s = '<span style="padding-left:20px;margin-left:4px;' + "background-image:url('" + row.avatar16Url + "');" + 'background-position:left center;background-repeat:no-repeat">' + row.name + '</span>';
        return s;
    }
</script>
 
 <span id="<%=this.ClientID %>_SpanPeople"><select class="easyui-combobox comboperson" url="" name="DropPeople" id="<%=this.ClientID %>_DropPeople" animate="true" style="width:300px"></select></span>