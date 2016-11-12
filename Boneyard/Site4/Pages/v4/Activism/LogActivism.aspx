<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="LogActivism.aspx.cs" Inherits="Pages_v4_Activism_LogActivism" Title="Claim an Expense - PirateWeb" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTreeDropDown.ascx" TagName="FinancialAccountTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="smiley-grin--plus.png" Title="Log Activism" Description="Log an external activity with photo documentation"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Log activism</span><br />
            <div class="DivGroupBoxContents">
                
                <b>Step 1:</b> What <b>organization</b> is logging the activism?<br />
                <div style="float:left;width:100px">Organization:</div><asp:DropDownList ID="DropOrganizations" runat="server"><asp:ListItem Selected="True" Text="Piratpartiet SE" Value="1" /></asp:DropDownList>&nbsp;<br />
                <b>Step 2:</b> <b>Where</b> did the activism take place?<br />
                <div style="float:left;width:100px">Geography:</div>
                <pw4:GeographyTreeDropDown ID="DropGeographies" runat="server" />&nbsp;<asp:CustomValidator 
                                    ID="Validator_Geography_Required" runat="server" EnableClientScript="false" 
                                    ErrorMessage="Please select a geography." />
                                
                                <br />
                <b>Step 3:</b> What <b>kind</b> of activism took place, <b>when</b> did it happen, and what was it <b>about</b>? Describe in a few words.<br />
                <div style="float:left;width:100px">Type:<br />Date:<br />Description:</div>
                <asp:DropDownList ID="DropActivityType" runat="server" ><asp:ListItem Selected="True" Text="Freeform flyer/folder handouts" Value="Flyers" /><asp:ListItem Text="Street station (table, cabin) with colors" Value="Street" /><asp:ListItem Text="School display" Value="School" /><asp:ListItem Text="Rally/Demo" Value="Rally" /><asp:ListItem Text="Fair presence" Value="Fair" /><asp:ListItem Text="Seminar participation" Value="Seminar" /><asp:ListItem Text="Coverage in local press" Value="PressCoverage" /></asp:DropDownList>&nbsp;
                                <br /><telerik:RadDatePicker ID="DatePicker" runat="server" />&nbsp;<asp:CustomValidator 
                                    ID="Validator_DatePicker_Custom" runat="server" 
                                    ErrorMessage="Please select a valid non-future date." 
                                    onservervalidate="Validator_DatePicker_Custom_ServerValidate" />
                                <br /><asp:TextBox ID="TextDescription" runat="server" />&nbsp;<asp:RequiredFieldValidator EnableClientScript="false" 
                                    ID="Validator_TextDescription_Required" runat="server" 
                                    ErrorMessage="Please describe the activism." 
                    ControlToValidate="TextDescription" />
                                <br />
                <b>Step 4:</b> Upload a cool <b>photo</b> from the activism (mobile phone photos do well, as do press quality images).<br />
                <div style="float:left;width:100px">Photo:<br />Upload:<br />&nbsp;</div><asp:Label Visible="false" ID="TemporaryDocumentIdentity" Text="0" runat="server" />
                <pw4:DocumentList ID="DocumentList" runat="server" />
                                <asp:CustomValidator ID="Validator_DocumentList_Custom" runat="server" 
                                    ErrorMessage="Please upload one or more photos from the activism." 
                                    onservervalidate="Validator_DocumentList_Custom_ServerValidate"></asp:CustomValidator>
                                <br />
                <telerik:RadUpload ID="Upload" Runat="server" ControlObjectsVisibility="None" 
                    MaxFileInputsCount="1" EnableFileInputSkinning="false"
                    TargetPhysicalFolder="C:\Data\Uploads\PirateWeb" /><asp:Button ID="ButtonUpload" 
                                    runat="server" Text="Upload file" 
                    onclick="ButtonUpload_Click" CausesValidation="False" />&nbsp;<br clear="all"/>
                <b>Step 5:</b> All done? Log the activism!<br />
                <div style="float:left;width:100px">All done?</div>
                <asp:Button ID="ButtonLogActivism" runat="server" Text="All Done!" 
                    onclick="ButtonLogActivity_Click" />&nbsp;<br />
                
                
            </div>
        </div>
    </div>


</asp:Content>

