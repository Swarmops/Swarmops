<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeBehind="SupportedCultures.aspx.cs" CodeFile="SupportedCultures.aspx.cs" Inherits="Swarmops.Frontend.Admin.SupportedCultures" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" runat="server">
    
        <script type="text/javascript">

        // Doc.ready:

        $(document).ready(function() {

            $('#tableSupportedCultures').datagrid(
                {
                    onLoadSuccess: function() {
                    }
                }
            );
        });

    </script>
    
     <style type="text/css">
          .datagrid-row-selected,.datagrid-row-over{
              background:transparent;
          }
     </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <h2>Supported and Installed Cultures</h2>
    <table id="tableCultures" title="" class="easyui-datagrid" style="width:680px"
        url="Json-CultureInfo.aspx"
        rownumbers="false"
        animate="true"
        fitColumns="true"
        idField="cultureId">
        <thead>
            <tr>  
                <th field="cultureId" width="80">ID</th>  
                <th field="name" width="218">Culture Native</th>  
                <th field="nameInternational" width="200">International</th>  
                <th field="country" width="160">Country</th>
                <th field="flag" width="40" align="center">Flag</th>
                <th field="supported" width="40" align="center">Sup</th>
            </tr>  
        </thead>  
    </table>     
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" runat="server">
</asp:Content>
