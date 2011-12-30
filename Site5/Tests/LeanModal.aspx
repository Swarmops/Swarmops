<%@ Page Title="" Language="C#" MasterPageFile="LeanModal-Master.master" AutoEventWireup="true" CodeFile="LeanModal.aspx.cs" Inherits="Tests_LeanModal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

<style type="text/css">
    
    
#lean_overlay {
    position: fixed;
    z-index:100;
    top: 0px;
    left: 0px;
    height:100%;
    width:100%;
    background: #000;
    display: none;
}

    
    </style>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">

    <a id="go" rel="leanModal" href="#overlay">Test!</a>

    <div id="overlay" style="opacity:0;display:none">Samuel Jackson Lorem Ipsum</div>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

