<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ComboPersonObsolete.ascx.cs" Inherits="Swarmops.Controls.Swarm.ComboPersonObsolete" %>

<telerik:RadComboBox runat="server" ID="ComboControl" Width="200px"  Skin="WebBlue"
   EnableLoadOnDemand="True" ShowMoreResultsBox="true"
                            EnableVirtualScrolling="true" AutoPostBack="true"
    OnItemsRequested="ComboControl_ItemsRequested" ZIndex="9001"
    OnSelectedIndexChanged="ComboControl_SelectedIndexChanged" />

