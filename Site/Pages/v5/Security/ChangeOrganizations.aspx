<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Security.ChangeOrganizations" Codebehind="ChangeOrganizations.aspx.cs" %>
<%@ Import Namespace="Swarmops.Logic.Support" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
	<script type="text/javascript" src="/Scripts/fancybox/jquery.fancybox-1.3.4.js"></script>
    <script type="text/javascript" src="/Scripts/fancybox/jquery.mousewheel-3.0.4.pack.js"></script>
	<link rel="stylesheet" type="text/css" href="/Scripts/fancybox/jquery.fancybox-1.3.4.css" media="screen" />

    
    <script type="text/javascript">

        $(document).ready(function() {
            if (organizationCount == 0) {
                alertify.alert("<asp:Literal ID="LabelNoOrganizations" runat="server" />", function() {
                    document.location = "/";
                });
            }
        });

        var organizationCount = <%=this.OrganizationCount %>;

      
    </script>
    
    <style type="text/css">
        div.divOrganizationEncapsulation {
            border: solid 1px transparent;
            padding-top: 1px;                    
        }
        div.divOrganizationEncapsulation:hover {
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
         
        div.divOrganizationLogo {
            float: left;
            background-position: left;
            padding-left: 2px;
            margin-left: 4px;
            margin-right: 8px;
            background-repeat: no-repeat;
            width: 70px;
            height: 48px;
        }

        div.spanOrganizationName {
            padding-top: 10px;
            font-size: 24px;
            padding-bottom: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <% if (!PilotInstallationIds.IsPilot(PilotInstallationIds.PiratePartySE))  // at the first pilot installation, the first live org was #1 and there was no sandbox
       { %>
    <div class="divOrganizationEncapsulation" onclick="document.location='/Pages/v5/Security/SetCurrentOrganization.aspx?OrganizationId=1';">
        <div class="divOrganizationLogo" style="background-image:url('/Images/Other/clipart-panda-free-sandbox-icon-64px.png')" ></div>
        <div class="spanOrganizationName">Sandbox</div>
    </div>
    <hr/>
    <% } %>
    <asp:Repeater runat="server" ID="RepeaterOrganizations">
        <ItemTemplate>
            <div class="divOrganizationEncapsulation" onclick="document.location='/Pages/v5/Security/SetCurrentOrganization.aspx?OrganizationId=<%# Eval("OrganizationId") %>';">
                <div class="divOrganizationLogo" style="background-image:url(data:image/png;base64,<%# Eval("LogoImage")%>)" ></div>
                <div class="spanOrganizationName"><%# Eval("OrganizationName") %></div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

