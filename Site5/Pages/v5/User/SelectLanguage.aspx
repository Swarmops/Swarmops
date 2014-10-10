<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="SelectLanguage.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.User.SelectLanguage" %>
<%@ Register src="~/Controls/v5/UI/ExternalScripts.ascx" tagname="ExternalScripts" tagprefix="Swarmops5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ExternalScripts1" Package="easyui" Control="datagrid" runat="server" />
	<script type="text/javascript" src="/Scripts/fancybox/jquery.fancybox-1.3.4.js"></script>
    <script type="text/javascript" src="/Scripts/fancybox/jquery.mousewheel-3.0.4.pack.js"></script>
	<link rel="stylesheet" type="text/css" href="/Scripts/fancybox/jquery.fancybox-1.3.4.css" media="screen" />

    
    <script type="text/javascript">

        $(document).ready(function () {
        });

    </script>
    
    <style type="text/css">
        div.divLanguageEncapsulation {
            border: solid 1px transparent;
            padding-top: 1px;                    
        }
        div.divLanguageEncapsulation:hover {
            border-radius: 4px;
            -moz-border-radius:4px;
            border: solid 1px #FFBC37;
            background: #FFD580;
            background: -webkit-gradient(linear, left top, left bottom, from(#ffd98c), to(#f0c265));
            background: -moz-linear-gradient(top,  #ffd98c,  #f0c265);
            filter:  progid:DXImageTransform.Microsoft.gradient(startColorstr='#ffd98c', endColorstr='#f0c265');
            color: #7F5500;
            cursor: pointer;
            -webkit-box-shadow: 0 1px 3px #C78B15;
            -moz-box-shadow: 0 1px 3px #C78B15;
            box-shadow: 0 1px 3px #C78B15;
        }
         
        div.divLanguageFlag {
            float: left;
            background-position: left;
            padding-left: 2px;
            margin-left: 4px;
            background-repeat: no-repeat;
            width: 70px;
            height: 48px;
        }

        div.spanLanguageNativeName {
            padding-top: 10px;
            font-size: 24px;
            padding-bottom: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <asp:Repeater runat="server" ID="RepeaterLanguages">
        <ItemTemplate>
            <div class="divLanguageEncapsulation" onclick="document.location='/Pages/v5/User/SetCulture.aspx?CultureId=<%# Eval("CultureId") %>';">
                <div class="divLanguageFlag" style="background-image:url('<%# Eval("IconUrl")%>')" ></div>
                <div class="spanLanguageNativeName"><%# Eval("DisplayName") %></div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

