<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TreeDropdown.ascx.cs"
    Inherits="jQuery_ServerControls_TreeDropdown" %>
    <asp:HiddenField ID="HiddenValue" runat="server" />
    <asp:HiddenField ID="HiddenText" runat="server" />
<div style="position: relative; display: inline; z-index:1;" runat="server" id="TreeDropdown_controlFrame">
    <div id="valueFrame" runat="server" class="TreeDropdown_valueframe">
        <div id="value" runat="server" class="TreeDropdown_value" >
        </div>
        <input type="image" runat="server" id="ImageButton1" class="TreeDropdown_openbutton"
            style="border-width: 0px; display: inline;" onclick="this.focus();return false;"
            src="/jQuery/ServerControls/DropDownArrow-s.png" />
    </div>
    <div id="treeView" class="TreeDropdown_tree" runat="server" 
        enableviewstate="False">
    </div>
</div>
