<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Default" Codebehind="Default.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <!-- Odometer -->
    <script language="javascript" type="text/javascript" src="/Scripts/odometer.js" ></script>
    <link href="/Style/odometer-theme-car.css" rel="stylesheet" type="text/css" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="box" style="background-image: url(/Images/Other/member-background-istockphoto.jpg); background-size: 700px"><div class="content">
    <div class="odometer-wrapper"><div class="elementFloatFar odometer odometer-integer nocents" id="odoGlobalParticipation">123402500</div><div class="odometer-label">Current Participants (Testing new code)</div>
    </div>
        
    <div class="odometer-wrapper"><div class="elementFloatFar odometer odometer-integer nocents" id="odoActiveParticipation">125498</div><div class="odometer-label">Active Participants Last 30 Days (Testing new code)</div>
    </div>
        
        <hr/>

            <div class="odometer-wrapper"><div class="elementFloatFar odometer odometer-integer nocents" id="odoLocalParticipation">498.001</div><div class="odometer-label">Current Participants Sollentuna</div>
    </div>


    </div></div>

    <div class="box" style="background-image: url(/Images/Other/coins-background-istockphoto.jpg); background-size: 700px"><div class="content">
    <div class="odometer-wrapper"><div class="elementFloatFar odometer odometer-currency" id="odoProfitLossToDate">500.001</div><div class="odometer-label">Annual Profit To Date (Testing new code)</div>
    </div></div></div>
    
    <div class="box"><div class="content">
    <h2>Dashboard Placeholder</h2>
    <p>Dashboard is being constructed in preparation for beta release. Lots of code are in testing (visible on the Sandbox).</p>
        
        </div></div>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">

</asp:Content>
