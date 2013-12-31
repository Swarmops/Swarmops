<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" EnableViewState="true" Inherits="Swarmops.Pages.Security.Login" %>
<%@ Import Namespace="Swarmops.Database" %>
<%@ Import Namespace="Telerik.Web.UI" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href='https://fonts.googleapis.com/css?family=Permanent+Marker' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Arimo:regular,italic,bold,bolditalic' rel='stylesheet' type='text/css' />
	<telerik:RadStyleSheetManager id="RadStyleSheetManager" runat="server" />
    <link href="../Style/style-v5.css" rel="stylesheet" type="text/css" />
    <title>Swarmops Alpha - Login</title>
</head>

<body class="loginpage">
    <div class="center640px">
        <div style="height:100px">&nbsp;</div>
        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />

	        <script type="text/javascript">
		        //Put your JavaScript code here.
            </script>
            
	        <telerik:RadAjaxManager ID="RadAjaxManager" runat="server" />
	        <telerik:RadSkinManager ID="RadSkinManager" Runat="server" Skin="WebBlue" />
        
            <div class="loginbox">
                <div class="content">
                    <div class="content">
                        <div style="width:64px;height:64px;float:left"><img src="/Security/Images/iconshock-fingerprint-scanner-unlock-64px.png" alt="unlock icon" /></div><h1>PIRATEWEB5<span class="loginalphabetasign">&beta;</span> LOGIN</h1>
                        <div class="box">
                            <div class="content">
                                <div class="entryLabelsAdmin">
                                    Login:<br />
                                    Password:<br />
                                </div>
                                <div class="entryFieldsAdmin">
                                    <asp:TextBox ID="TextLogin" runat="server" CssClass="textinput" />&nbsp;<br />
                                    <asp:TextBox ID="TextPassword" runat="server" CssClass="textinput" TextMode="Password" />&nbsp;<br />
                                    <div class="button-orange-encaps" style="float:right; margin-top:5px; margin-right:2px;margin-bottom:3px;">
                                        <asp:Button ID="ButtonLogin" runat="server" 
                                            Text="Login" onclick="ButtonLogin_Click" CssClass="button button-orange" /></div><br clear="all" />
                                </div>
                                <div class="entryValidationAdmin">
                                    <asp:RequiredFieldValidator ID="ValidatorRequired_Login" Text="Please enter a valid login." runat="server" Display="Dynamic" EnableClientScript="false" ControlToValidate="TextLogin" CssClass="" />&nbsp;<asp:Label ID="LabelLoginFailed" runat="server" Visible="false" Text="Login failed, please retry." /><br />
                                    <asp:RequiredFieldValidator ID="ValidatorRequired_Password" Text="Please enter your password." runat="server" Display="Dynamic" EnableClientScript="false" ControlToValidate="TextPassword" CssClass="" />&nbsp;<br />
                                </div>
                                <div class="break"></div>
                            </div>
                        </div>
                    </div>
                </div>        
            </div>
        </form>
    </div>
</body>
</html>
