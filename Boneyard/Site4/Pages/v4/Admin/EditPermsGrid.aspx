<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditPermsGrid.aspx.cs" Inherits="Admin_EditPermsGrid" MasterPageFile="~/PirateWeb-v4.master"  %>
 
 <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
   <style type="text/css">
        .style3
        {
        }
        td
        {
            border: 1px solid black;
            font-size:0.9em;
        }
        .rolecell
        {
            text-align:center;
        }
        .cb
        {
            border: 1px solid white;
            color: white;
        }
        .heading
        {
            font-size: large;
        }
        .newStyle1
        {
        }
    </style>

    <script type="text/javascript">
    function cbPerClick(cbElem,role,perm)
    {
        var newSetting= cbElem.checked ? true : false ;
    
        var request = new CSaveAccessClick(cbElem)
        request.SaveAccess(role,perm,newSetting);

    }

    function CSaveAccessClick(cbElem) {

        this.SaveAccessOK = function(result) {
            if (result.indexOf("Error:") == -1) {
                cbElem.parentNode.style.backgroundColor = "";
            }
            else {
                cbElem.parentNode.style.backgroundColor = "red";
                alert(result);
            }
        };

        this.SaveAccessFailed = function(error, userContext, methodName) {
            if (error) {
                cbElem.parentNode.style.backgroundColor = "red";
                alert("Callback failed:" + error.get_message());
            }
        };
        
        this.SaveAccess = function(role,perm,access) {
            cbElem.parentNode.style.backgroundColor = "silver";
            PageMethods.SaveAccessClick(role,perm,access, this.SaveAccessOK, this.SaveAccessFailed);
        };
    }


    function onMouseEnterResult(role, perm) {
        var rCell = $get("ctl00_BodyContent_R" + role);
        var pCell = $get("ctl00_BodyContent_P" + perm);
        pCell.style.backgroundColor = "#EEFF8C";
        rCell.style.backgroundColor = "#EEFF8C";

    }
    function onMouseLeaveResult(role, perm) {
        var rCell = $get("ctl00_BodyContent_R" + role);
        var pCell = $get("ctl00_BodyContent_P" + perm);
        pCell.style.backgroundColor = "";
        rCell.style.backgroundColor = "";

    }
    //function btnAddPerm_onclick() {
//     location.href='addPermission.aspx';
//}

//function btnAddRole_onclick() {
//     location.href='addRole.aspx';
//}

//function editRole(roleID) {
//     location.href='addRole.aspx?ID='+roleID;
//}

//function editPermission(permID) {
//     location.href='addPermission.aspx?ID='+permID;
//}

    </script>
</asp:Content>
 <asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <div class="bodyContent">
        <table id="MainTab" runat="server" cellspacing=0 border=1 style="border-collapse:collapse;">
            <tr>
                <td class="style3"  valign="bottom"  >
                    <span class="heading">Permissions&nbsp;&nbsp;</span>
                </td>            
            </tr>
            <tr>
                <td>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>