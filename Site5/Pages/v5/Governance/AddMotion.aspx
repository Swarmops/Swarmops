<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AddMotion.aspx.cs" Inherits="Swarmops.Pages.Governance.AddMotion" %>

<%@ Register src="~/Controls/v5/Swarm/ComboPersonObsolete.ascx" tagname="ComboPerson" tagprefix="act5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
<script type="text/javascript">
    $(document).ready(function() {
        $('#MotionSubmittedMessage').fadeOut(7000);
    });
</script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
<div class="entryLabelsAdmin" style="width:150px">
    <asp:Label ID="LabelTitle" runat="server" /><br />
    <asp:Label ID="LabelSubmitter" runat="server" /><br />
    <div style="height:214px"><asp:Label ID="LabelText" runat="server" /></div>
    <asp:Label ID="LabelDecisions" runat="server" />
</div>
<div class="entryFieldsAdmin">
    <div style="height:30px; margin-top:4px"><asp:TextBox CssClass="textinput" ID="TextTitle" runat="server" /></div>
    <div style="height:30px"><act5:ComboPerson ID="PersonSubmitter" runat="server" /></div>
    <asp:TextBox CssClass="textinput" TextMode="MultiLine" Rows="10" ID="TextMotionText" runat="server" />&nbsp;<br />
    <asp:TextBox CssClass="textinput" TextMode="MultiLine" Rows="5" ID="TextDecisionPoints" runat="server" />&nbsp;<br />
    <div class="button-orange-encaps" style="float:right; margin-top:5px; margin-right:2px;margin-bottom:8px;">
        <asp:Button ID="ButtonAddMotion" runat="server" 
            Text="Submit Motion" CssClass="button button-orange" OnClick="ButtonAddMotion_Click" />
    </div>
    <div id="MotionSubmittedMessage"><asp:Label ID="LabelMotionSaved" runat="server" Text="" /></div>
</div>
<div class="entryValidationAdmin">
    <asp:RequiredFieldValidator ID="ValidatorMotionTitleRequired" runat="server" Display="Dynamic" Text="Motion title required." ControlToValidate="TextTitle" EnableClientScript="false" />&nbsp;<br />
    <asp:CustomValidator ID="ValidatorSubmitterRequired" runat="server" Display="Dynamic" Text="Submitter required." OnServerValidate="ValidatorSubmitterRequired_Validate" />&nbsp;<br />
    <div style="height:214px"><asp:RequiredFieldValidator ID="ValidatorMotionTextRequired" runat="server" Display="Dynamic" Text="Motion text required." ControlToValidate="TextMotionText" EnableClientScript="false" /></div>
    <asp:RequiredFieldValidator ID="RequiredMotionDecisionPoints" runat="server" Display="Dynamic" Text="Decision points required." ControlToValidate="TextDecisionPoints" EnableClientScript="false" />
</div>
<div class="break"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue"><asp:Label ID="LabelSidebarInfo" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelSubmittingMotionInfo" runat="server" /> <b><asp:Label ID="LabelMeetingName" runat="server" /></b>. <a href="#"><asp:Label ID="LabelWrongMeeting" runat="server" /></a>
        </div>
    </div>   
    
    <h2 class="blue"><asp:Label ID="LabelSidebarActions" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <div class="link-row-encaps" onclick="document.location='/Pages/v5/Governance/ListMotions.aspx';" >
                <div class="link-row-icon" style="background-image:url('/Images/PageIcons/iconshock-motions-16px.png')"></div>
                <asp:Label ID="LabelActionListMotions" runat="server" />
            </div>
        </div>
    </div>
    
    <h2 class="orange"><asp:Label ID="LabelSidebarTodo" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelActionItemsHere" runat="server" />
        </div>
    </div>  

</asp:Content>

