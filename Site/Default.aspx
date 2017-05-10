<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Default" CodeFile="Default.aspx.cs" %>
<%@ Import Namespace="Swarmops.Logic.Support" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    
    <% if (Swarmops.Logic.Support.PilotInstallationIds.IsPilot(PilotInstallationIds.DevelopmentSandbox))
       { %>
    <div class="box" style="background-image: url(/Images/Other/member-background-istockphoto.jpg); background-size: 700px"><div class="content">
    <div class="odometer-wrapper">
        <div class="elementFloatFar odometer odometer-integer nocents" id="odoGlobalParticipation">12342500</div><div class="odometer-label">Current Participants</div>
    </div>
        
    <div class="odometer-wrapper">
        <div class="elementFloatFar odometer odometer-integer nocents" id="odoActiveParticipation">125498</div><div class="odometer-label">Active Participants Last 30 Days</div>
    </div>
        
        <hr/>

            <div class="odometer-wrapper"><div class="elementFloatFar odometer odometer-integer nocents" id="odoLocalParticipation">498.001</div><div class="odometer-label">Current Participants in Your City</div>
    </div>


    </div></div>
    
    <% } %>
    
    <div class="box"><div class="content">
      <h2><asp:Label ID="LabelHeaderLocal" runat="server" /></h2>
      <Swarmops5:TreePositions ID="TreePositions" Level="Geography" runat="server" />
        
     </div></div>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">

</asp:Content>
