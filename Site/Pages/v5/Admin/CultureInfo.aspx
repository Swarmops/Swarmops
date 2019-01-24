<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeBehind="CultureInfo.aspx.cs" CodeFile="CultureInfo.aspx.cs" Inherits="Swarmops.Frontend.Admin.CultureInfo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" runat="server">
    
        <script type="text/javascript">
        function preload(arrayOfImages) {
            $(arrayOfImages).each(function () {
                (new Image()).src = this;
            });
        }

        preload([
            '/Images/Abstract/ajaxloader-medium.gif',
            '/Images/Abstract/ajaxloader-48x36px.gif',
            '/Images/Icons/iconshock-balloon-yes-128x96px-hot.png',
            '/Images/Icons/iconshock-balloon-yes-128x96px-disabled.png',
            '/Images/Icons/iconshock-balloon-yes-128x96px-hot-disabled.png',
            '/Images/Icons/iconshock-balloon-yes-128x96px-gold.png',
            '/Images/Icons/iconshock-balloon-yes-128x96px-hot-gold.png',
            '/Images/Icons/iconshock-balloon-no-128x96px-hot.png',
            '/Images/Icons/iconshock-green-tick-128x96px.png',
            '/Images/Icons/iconshock-red-cross-128x96px.png',
            '/Images/Icons/iconshock-red-cross-circled-128x96px.png',
            '/Images/Icons/iconshock-balloon-undo-128x96px.png',
            '/Images/Icons/iconshock-balloon-undo-128x96px-hot.png'
        ]);

        /* -- commented out -- do we need attestation logic for this page?
        loadUninitializedBudgets(); // no need to wait for doc.ready to load operating params

        SwarmopsJS.ajaxCall("/Pages/v5/Financial/AttestCosts.aspx/GetRemainingBudgets", {}, function(data) {
            data.forEach(function(accountData, dummy1, dummy2) {
                budgetRemainingLookup[accountData.AccountId] = accountData.Remaining;
            });

            if (budgetRemainingLookup.rowsLoaded == true) {
                setAttestability();
            }

            budgetRemainingLookup.budgetsLoaded = true;
        });*/

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
                <th field="name" width="178">Culture Native</th>  
                <th field="nameInternational" width="160">International</th>  
                <th field="language" width="120">Language</th>
                <th field="country" width="120">Country</th>
                <th field="flag" width="40" align="center">Flag</th>
                <th field="supported" width="40" align="center">Sup</th>
            </tr>  
        </thead>  
    </table>     
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" runat="server">
</asp:Content>
