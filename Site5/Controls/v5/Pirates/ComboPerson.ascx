<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ComboPerson.ascx.cs" Inherits="Activizr.Controls.Pirates.ComboPerson" %>

<telerik:RadComboBox runat="server" ID="ComboControl" Width="200px"  Skin="WebBlue"
   EnableLoadOnDemand="True" ShowMoreResultsBox="true"
                            EnableVirtualScrolling="true" AutoPostBack="true"
    OnItemsRequested="ComboControl_ItemsRequested" ZIndex="9001"
    OnSelectedIndexChanged="ComboControl_SelectedIndexChanged" />

