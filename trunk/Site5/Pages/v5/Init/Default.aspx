<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Pages_v5_Init_Default" %>
<%@ Register Src="~/Controls/v5/Base/LanguageSelector.ascx" TagName="LanguageSelector" TagPrefix="act5" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <!-- jQuery and plugins -->
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.leanModal.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.smartWizard-2.0.min.js"></script>

    <!-- fonts -->
    <link href='https://fonts.googleapis.com/css?family=Permanent+Marker' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Ubuntu:light,regular,500,bold' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Arimo:regular,italic,bold,bolditalic' rel='stylesheet' type='text/css' />

    <!-- page title -->
	<title>Activizr - Initialize Installation</title>

	<!-- telerik style sheet manager, followed by style sheet -->
	<telerik:RadStyleSheetManager id="RadStyleSheetManager" runat="server" />
    <link href="/Style/style-v5.css" rel="stylesheet" type="text/css" />
    <link href="WizardStyle.css" rel="stylesheet" type="text/css" />

    <!-- initialize all modal popups -->
    <script language="javascript" type="text/javascript">
        $(function () {
            $("a[rel*=leanModal]").leanModal();
        });
    </script>
</head>
<body>
    <form id="form2" runat="server">
	<telerik:RadScriptManager ID="RadScriptManager1" runat="server" />
	<script type="text/javascript">


	    $(document).ready(function () {
	        // Initialize Smart Wizard	
	        $('#wizard').smartWizard({ transitionEffect: 'fade', onLeaveStep: leaveAStepCallback });

	        function leaveAStepCallback(obj) {
	            var step_num = obj.attr('rel');
	            return validateStep(step_num);
	        }

	        function validateStep(stepNumber) {
	            var isValid = false;

	            if (stepNumber == 1) {
	                // Validate Hostname, Host Address
	                var hostName = $('#<%=this.TextServerName.ClientID %>').val();
	                var hostAddress = $('#<%=this.TextServerAddress.ClientID %>').val();

	                if (hostName && hostName.length > 0 && hostAddress && hostAddress.length > 0) {
	                    $.ajax({
	                        type: "POST",
	                        url: "Default.aspx/VerifyHostNameAndAddress",
	                        data: "{'name': '" + hostName + "', 'address': '" + hostAddress + "'}",  // TODO: potential input vulnerability - strip single quotes from input
	                        contentType: "application/json; charset=utf-8",
	                        dataType: "json",
	                        async: false,  // blocks until function returns - race conditions otherwise
	                        success: function (msg) {
	                            if (msg.d == true) {
	                                isValid = true;
	                            }
	                        }
	                    });
	                }


	                if (!isValid) {
	                    $('#<%=this.TextServerName.ClientID %>').css('background-image', "url('/Images/Icons/iconshock-cross-12px.png')").css('background-position', 'right center').css('background-repeat', 'no-repeat');
	                    $('#<%=this.TextServerAddress.ClientID %>').css('background-image', "url('/Images/Icons/iconshock-cross-12px.png')").css('background-position', 'right center').css('background-repeat', 'no-repeat');
	                } else {
	                    $('#<%=this.TextServerName.ClientID %>').css('background-image', "none");
	                    $('#<%=this.TextServerAddress.ClientID %>').css('background-image', "none");
	                }
	            }
	            else if (stepNumber == 2) {
	                isValid = true; // assume true, make false as we go

	                var textBoxes = ["<%=this.TextCredentialsReadDatabase.ClientID %>", "<%=this.TextCredentialsReadServer.ClientID %>", "<%=this.TextCredentialsReadUser.ClientID %>", "<%=this.TextCredentialsReadPassword.ClientID %>",
	                    "<%=this.TextCredentialsWriteDatabase.ClientID %>", "<%=this.TextCredentialsWriteServer.ClientID %>", "<%=this.TextCredentialsWriteUser.ClientID %>", "<%=this.TextCredentialsWritePassword.ClientID %>",
	                    "<%=this.TextCredentialsAdminDatabase.ClientID %>", "<%=this.TextCredentialsAdminServer.ClientID %>", "<%=this.TextCredentialsAdminUser.ClientID %>", "<%=this.TextCredentialsAdminPassword.ClientID %>"];

	                for (var loop = 0; loop < textBoxes.length; loop++) {
	                    var fieldContents = $('#' + textBoxes[loop]).val();

	                    if (fieldContents && fieldContents.length > 0) {
	                        $('#' + textBoxes[loop]).css('background-image', "none");
	                    } else {
	                        $('#' + textBoxes[loop]).css('background-image', "url('/Images/Icons/iconshock-cross-12px.png')").css('background-position', 'right center').css('background-repeat', 'no-repeat');
	                        isValid = false;
	                    }
	                }


	                if (isValid) {
	                    $('#<%=this.ButtonInitDatabase.ClientID %>').click();
	                }
	                // TODO: If isValid, press invisible button that starts import

	            }
	            return isValid;
	        }
	    });

	</script>
	<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
	</telerik:RadAjaxManager>
 
	<telerik:RadSkinManager ID="RadSkinManager1" Runat="server" Skin="WebBlue">
	</telerik:RadSkinManager>
	
    <!-- Main menu, dynamically constructed -->

	<div class="center980px">
	    <div class="currentuserinfo"><div style="background-image:url('/Images/Icons/iconshock-user-16px.png');background-repeat:no-repeat;padding-left:16px;float:left"><asp:Label ID="LabelCurrentUserName" runat="server" /> | </div><div style="background-image:url('/Images/Icons/iconshock-workchair-16px.png');background-repeat:no-repeat;padding-left:17px;float:left"><asp:Label ID="LabelCurrentOrganizationName" runat="server" /> |&nbsp;</div><div style="background-image:url('/Images/Icons/iconshock-gamepad-16px.png');background-repeat:no-repeat;padding-left:20px;float:left"><asp:Label ID="LabelPreferences" runat="server" /> |&nbsp;</div><asp:Image ID="ImageCultureIndicator" runat="server" ImageUrl="~/Images/Flags/uk.png" /></div>
        <div class="logoimage"><a href="/"><img style="border:none" src="/Style/Images/activizr-v5-pirateedition-logo.png" alt="Activizr Logo" /></a></div>
        <div class="break"></div>
        <div class="topmenu">
            <div class="searchbox"><asp:TextBox ID="SearchBox" runat="server" /></div>
            <telerik:RadMenu ID="MainMenu" runat="server" BackColor="Transparent" Font-Bold="true">
                <Items>
                    <telerik:RadMenuItem runat="server" Text="XYZ People" UserLevel="1" Permission="All" GlobalResourceKey="Menu5_People">
                        <Items>
                            <telerik:RadMenuItem runat="server" Text="XYZ Log Activism" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_People_LogActivism" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Self-Signup Page" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_People_SelfSignup" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="2" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Add Member" Permission="Members:Write" UserLevel="2" GlobalResourceKey="Menu5_People_AddMember" />
                            <telerik:RadMenuItem runat="server" Text="XYZ List Members" Permission="Members:Read" UserLevel="2" GlobalResourceKey="Menu5_People_ListMembers" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Manage Volunteers" Permission="Responsibilities:Write" UserLevel="2" GlobalResourceKey="Menu5_People_ManageVolunteers" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="2" />
                            <telerik:RadMenuItem runat="server" Text="XYZ New conference" Permission="All" UserLevel="2" GlobalResourceKey="Menu5_People_NewConference" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Manage conferences" Permission="All" UserLevel="2" GlobalResourceKey="Menu5_People_ManageConferences" />
                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem runat="server" Text="XYZ Communications" Permission="All" UserLevel="2" GlobalResourceKey="Menu5_Communications">
                        <Items>
                            <telerik:RadMenuItem runat="server" Text="XYZ Mass Mailing" Permission="MemberMail:Write,Newsletter:Write" UserLevel="2" GlobalResourceKey="Menu5_Communications_MassMailing" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Edit Automail Text" Permission="MemberMail:Write" UserLevel="2" GlobalResourceKey="Menu5_Communications_EditAutomailText" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="2" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Received Paper Letter" Permission="All" UserLevel="2" GlobalResourceKey="Menu5_Communications_ReceivedPaperLetter" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Manage Paper Inbox" Permission="All" UserLevel="2" GlobalResourceKey="Menu5_Communications_PaperInbox" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Manage Mail Inbox" Permission="SupportMail:Read" UserLevel="2" GlobalResourceKey="Menu5_Communications_MailInbox" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Manage Text Inbox" Permission="SupportText:Read" UserLevel="2" GlobalResourceKey="Menu5_Communications_TextInbox" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="2" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Text Activists" Permission="ActivistText:Write" UserLevel="2" GlobalResourceKey="Menu5_Communications_TextActivists" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="2" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Manager Reporter List" Permission="ReporterList:Read" UserLevel="2" GlobalResourceKey="Menu5_Communications_ReporterList" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Send Press Release" Permission="PressReleases:Write" UserLevel="2" GlobalResourceKey="Menu5_Communications_SendPressRelease" />
                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem runat="server" Text="XYZ Financial" Permission="CanSeeSelf" UserLevel="1" GlobalResourceKey="Menu5_Financial">
                        <Items>
                            <telerik:RadMenuItem runat="server" Text="XYZ Claim Refund For Expense" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_Financial_ClaimExpense" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Request Cash Advance" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_Financial_RequestCashAdvance" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Create Purchase Order" Permission="All" UserLevel="2" GlobalResourceKey="Menu5_Financial_CreatePurchaseOrder" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="1" />
                            <telerik:RadMenuItem runat="server" Text="XYZ View Budget, Actuals" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_Financial_ViewBudgetActuals" />
                            <telerik:RadMenuItem runat="server" Text="XYZ View Liquidity Forecast" Permission="All" UserLevel="2" GlobalResourceKey="Menu5_Financial_ViewLiquidityForecast" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="2" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Manage Budget" Permission="All" UserLevel="2" GlobalResourceKey="Menu5_Financial_ManageBudget" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Attest Costs" Permission="All" UserLevel="2" GlobalResourceKey="Menu5_Financial_AttestCosts" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="2" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Send Invoice" Permission="Economy:Write" UserLevel="2" GlobalResourceKey="Menu5_Financial_SendInvoice" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Received Invoice" Permission="Economy:Write" UserLevel="2" GlobalResourceKey="Menu5_Financial_ReceivedInvoice" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Gigs Assistant" Permission="All" UserLevel="3" GlobalResourceKey="Menu5_Financial_GigsAssistant" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="3" />
                            <telerik:RadMenuItem runat="server" Text="XYZ List" Permission="Economy:Read" UserLevel="3" GlobalResourceKey="Menu5_Financial_List">
                                <Items>
                                    <telerik:RadMenuItem runat="server" Text="XYZ Expense Claims & Cash Requests" Permission="Economy:Read" UserLevel="3" GlobalResourceKey="Menu5_Financial_List_ExpenseClaims" />
                                    <telerik:RadMenuItem runat="server" Text="XYZ Inbound Invoices" Permission="Economy:Read" UserLevel="3" GlobalResourceKey="Menu5_Financial_List_InboundInvoices" />
                                    <telerik:RadMenuItem runat="server" Text="XYZ Salaries" Permission="Payroll:Read" UserLevel="3" GlobalResourceKey="Menu5_Financial_List_Salaries" />
                                    <telerik:RadMenuItem runat="server" Text="XYZ Outbound Invoices" Permission="Economy:Read" UserLevel="3" GlobalResourceKey="Menu5_Financial_List_OutboundInvoices" />
                                    <telerik:RadMenuItem runat="server" Text="XYZ Outstanding Cash Advances" Permission="Economy:Read" UserLevel="3" GlobalResourceKey="Menu5_Financial_List_OutstandingAdvances" />
                                </Items>
                            </telerik:RadMenuItem>
                            <telerik:RadMenuItem runat="server" Text="XYZ Pay Out Money" Permission="Economy:Write" UserLevel="3" GlobalResourceKey="Menu5_Financial_PayOutMoney" />
                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem runat="server" Text="XYZ Ledgers" Permission="All" UserLevel="3" GlobalResourceKey="Menu5_Ledgers">
                        <Items>
                            <telerik:RadMenuItem runat="server" Text="XYZ Balance Sheet" Permission="All" UserLevel="3" GlobalResourceKey="Menu5_Ledgers_BalanceSheet" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Result Sheet" Permission="All" UserLevel="3" GlobalResourceKey="Menu5_Ledgers_ResultSheet" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="3" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Audit Ledgers" Permission="EconomyDetail:Read" UserLevel="3" GlobalResourceKey="Menu5_Ledgers_AuditLedgers" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Account Plan" Permission="Economy:Read" UserLevel="3" GlobalResourceKey="Menu5_Ledgers_AccountPlan" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Set Budget" NavigateUrl="~/Pages/v5/Ledgers/SetRootBudgets.aspx" ImageUrl="~/Images/PageIcons/iconshock-moneybag-16px.png" Permission="Financial:Write" UserLevel="3" GlobalResourceKey="Menu5_Ledgers_SetBudgets" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="3" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Upload Bank Files" NavigateUrl="~/Pages/v5/Ledgers/UploadBankFiles.aspx" ImageUrl="~/Images/PageIcons/iconshock-bank-16px.png" Permission="Economy:Write" UserLevel="3" GlobalResourceKey="Menu5_Ledgers_UploadBankFiles" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="3" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Debug Ledgers" NavigateUrl="~/Pages/v5/Ledgers/DebugLedgers.aspx" ImageUrl="~/Images/PageIcons/iconshock-tester-16px.png" Permission="EconomyDetail:Read" UserLevel="3" GlobalResourceKey="Menu5_Ledgers_DebugLedgers" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Close Ledgers" NavigateUrl="~/Pages/v5/Ledgers/CloseLedgers.aspx" Template="CloseLedgers" Dynamic="true" ImageUrl="~/Images/PageIcons/iconshock-calculator-lock-16px.png" Permission="EconomyDetail:Write" UserLevel="3" GlobalResourceKey="Menu5_Ledgers_CloseLedgers" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="3" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Payroll" Permission="Payroll:Read" UserLevel="3" GlobalResourceKey="Menu5_Ledgers_Payroll" />
                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem runat="server" Text="XYZ Governance" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_Governance">
                        <Items>
                            <telerik:RadMenuItem runat="server" Text="XYZ Vote" NavigateUrl="~/Pages/v5/Governance/Vote.aspx" ImageUrl="~/Images/PageIcons/iconshock-vote-16px.png" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_Governance_Vote" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Motions" NavigateUrl="~/Pages/v5/Governance/ListMotions.aspx" ImageUrl="~/Images/PageIcons/iconshock-motions-16px.png" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_Governance_Motions" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Candidates" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_Governance_Candidates" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Create Voting List" Permission="All" UserLevel="2" GlobalResourceKey="Menu5_Governance_CreateVotingList" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="3" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Meeting Admin" Permission="Meetings:Read" UserLevel="3" GlobalResourceKey="Menu5_Governance_MeetingAdmin" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Meeting Admin for Board" Permission="MeetingsBoard:Read" UserLevel="3" GlobalResourceKey="Menu5_Governance_MeetingBoardAdmin" />
                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem runat="server" Text="XYZ Organization" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_Organization">
                        <Items>
                            <telerik:RadMenuItem runat="server" Text="XYZ Organization Chart" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_Organization_OrganizationChart" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Available Responsibilities" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_Organization_AvailableResponsibilities" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Member Statistics" Permission="All" UserLevel="1" GlobalResourceKey="Menu5_Organization_MemberStatistics" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="3" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Organization Settings" Permission="Organzation:Read" UserLevel="3" GlobalResourceKey="Menu5_Organization_Settings" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Organization Responsibilities" Permission=":Read" UserLevel="3" GlobalResourceKey="Menu5_Organization_Responsibilities" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Suborganization Tree" Permission=":Read" UserLevel="3" GlobalResourceKey="Menu5_Organization_SuborganizationTree" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="3" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Communication Templates" Permission="Communications:Read" UserLevel="3" GlobalResourceKey="Menu5_Organization_CommunicationTemplates" >
                                <Items>
                                    <telerik:RadMenuItem runat="server" Text="XYZ Press Release" Permission="Communications:Read" UserLevel="3" GlobalResourceKey="Menu5_Organization_CommunicationTemplates_PressRelease" />
                                    <telerik:RadMenuItem runat="server" Text="XYZ Mass Mailing" Permission="Communications:Read" UserLevel="3" GlobalResourceKey="Menu5_Organization_CommunicationTemplates_MassMailing" />
                                    <telerik:RadMenuItem runat="server" Text="XYZ Newsletter" Permission="Communications:Read" UserLevel="3" GlobalResourceKey="Menu5_Organization_CommunicationTemplates_Newsletter" />
                                    <telerik:RadMenuItem runat="server" Text="XYZ Invoice" Permission="Communications:Read" UserLevel="3" GlobalResourceKey="Menu5_Organization_CommunicationTemplates_Invoice" />
                                </Items>
                            </telerik:RadMenuItem>
                            <telerik:RadMenuItem runat="server" Text="XYZ Communication Feeds" Permission="Communications:Read" UserLevel="3" GlobalResourceKey="Menu5_Organization_Feeds" >
                                <Items>
                                    <telerik:RadMenuItem runat="server" Text="XYZ Newsletters" Permission="Newsletters:Write" UserLevel="3" GlobalResourceKey="Menu5_Organization_Feeds_Newsletter" />
                                    <telerik:RadMenuItem runat="server" Text="XYZ Press Releases" Permission="PressReleases:Write" UserLevel="3" GlobalResourceKey="Menu5_Organization_Feeds_PressReleases" />
                                </Items>
                            </telerik:RadMenuItem>
                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem runat="server" Text="XYZ Admin" Permission="All" UserLevel="4" GlobalResourceKey="Menu5_Administration">
                        <Items>
                            <telerik:RadMenuItem runat="server" Text="XYZ Organization Clusters" Permission="All" UserLevel="4" GlobalResourceKey="Menu5_Administration_OrganizationClusters" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Manage Geography" Permission="All" UserLevel="4" GlobalResourceKey="Menu5_Administration_GeographyTree" />
                            <telerik:RadMenuItem runat="server" Text="XYZ Access Control Lists" Permission="All" UserLevel="4" GlobalResourceKey="Menu5_Administration_AccessControlLists" />
                            <telerik:RadMenuItem runat="server" IsSeparator="true" UserLevel="4" />
                            <telerik:RadMenuItem runat="server" Dynamic="true" Template="Build#" Permission="All" UserLevel="4" NotaBene="This is a special case -- the build number is dynamically inserted for text" /> 
                        </Items>
                    </telerik:RadMenuItem>
                </Items>
            </telerik:RadMenu>
        </div>
        
        <div class="mainbar">
            <div id="page-icon-encaps"><asp:Image ID="IconPage" runat="server" ImageUrl="~/Images/PageIcons/iconshock-ignitionkey-40px.png" /></div><h1><asp:Label ID="LabelPageTitle" Text="Installation" runat="server" /></h1>
        
        <div class="box">
            <div class="content">
                
                <div id="wizard" class="swMain">
  			        <ul>
  				        <li><a href="#step-1">
                        <label class="stepNumber">1</label>
                        <span class="stepDesc">
                           Security Check<br />
                           <small>Are you the admin of this server?</small>
                        </span>                   
                    </a></li>
  				        <li><a href="#step-2">
                        <label class="stepNumber">2</label>
                        <span class="stepDesc">
                           Database<br />
                           <small>Supply database credentials and connect</small>
                        </span>
                    </a></li>
  				        <li><a href="#step-3">
                        <label class="stepNumber">3</label>
                        <span class="stepDesc">
                           User Data<br />
                           <small>Let's create the first user</small>
                        </span>                   
                     </a></li>
  				        <li><a href="#step-4">
                        <label class="stepNumber">4</label>
                        <span class="stepDesc">
                           Complete<br />
                           <small>All done. Let's login!</small>
                        </span>                   
                    </a></li>
  			        </ul>
  			        <div id="step-1">	
                        <h2>Welcome to Activizr</h2>
                        <p>Congratulations! Since you're reading this, you have successfully installed the Activizr packages and set up an Apache virtual server using mod_mono with Mono&nbsp;2.</p>

  			            <p>However, before we proceed, we need to make sure that you are indeed the sysadmin of this server, and not a remote bot who just discovered an unfinished Activizr installation. To do that, answer these simple questions:</p> <asp:Label runat="server" ID="LabelTest" />
                        
                        <div class="entrylabels" style="width:250px">
                            What is this server's /etc/hostname?<br />
  			                What is this server's internal IP?<br />
                            What is your favorite color?
                        </div>
                        <div class="entryfields">
                            <asp:TextBox CssClass="textinput" ID="TextServerName" runat="server" />&nbsp;<br />
                            <asp:TextBox CssClass="textinput"  ID="TextServerAddress" runat="server" />&nbsp;<br />
                            <asp:DropDownList ID="DropFavoriteColor" runat="server" />&nbsp;<br />
                        </div>
                    </div>
  			        <div id="step-2">
                        <h2>Connect to database</h2>	
                        <p>Before you fill this in, you will need to have created a database on a MySQL server that this web server can access, and set up user accounts that can access it. We <strong>very strongly</strong> recommend having three separate accounts - one for reading (needs SELECT only), one for writing (needs EXECUTE only), and one for admin. All three accounts also need SELECT permissions on the mysql database.</p>

                        <div class="entrylabels" style="width:120px">
                            &nbsp;<br/>
                            Database<br />
  			                Server<br />
                            User<br/>
                            Password
                        </div>
                        <div class="entryfields" style="width:80px">
                            <strong>Read access</strong><br/>
                            <asp:TextBox CssClass="textinput" ID="TextCredentialsReadDatabase" runat="server" />&nbsp;<br />
                            <asp:TextBox CssClass="textinput"  ID="TextCredentialsReadServer" runat="server" />&nbsp;<br />
                            <asp:TextBox CssClass="textinput"  ID="TextCredentialsReadUser" runat="server" />&nbsp;<br />
                            <asp:TextBox CssClass="textinput"  ID="TextCredentialsReadPassword" runat="server" />&nbsp;<br />
                        </div>
                        <div class="entryfields" style="width:80px;margin-left:10px">
                            <strong>Write access</strong><br/>
                            <asp:TextBox CssClass="textinput" ID="TextCredentialsWriteDatabase" runat="server" />&nbsp;<br />
                            <asp:TextBox CssClass="textinput"  ID="TextCredentialsWriteServer" runat="server" />&nbsp;<br />
                            <asp:TextBox CssClass="textinput"  ID="TextCredentialsWriteUser" runat="server" />&nbsp;<br />
                            <asp:TextBox CssClass="textinput"  ID="TextCredentialsWritePassword" runat="server" />&nbsp;<br />
                        </div>
                        <div class="entryfields" style="width:80px;margin-left:10px">
                            <strong>Admin access</strong><br/>
                            <asp:TextBox CssClass="textinput" ID="TextCredentialsAdminDatabase" runat="server" />&nbsp;<br />
                            <asp:TextBox CssClass="textinput"  ID="TextCredentialsAdminServer" runat="server" />&nbsp;<br />
                            <asp:TextBox CssClass="textinput"  ID="TextCredentialsAdminUser" runat="server" />&nbsp;<br />
                            <asp:TextBox CssClass="textinput"  ID="TextCredentialsAdminPassword" runat="server" />&nbsp;<br />
                        </div>
                        <div style="display:none"><asp:Button runat="server" ID="ButtonInitDatabase" Text="You should not see this button" OnClick="ButtonInitDatabase_Click"/></div>
                    </div>                      
  			        <div id="step-3">
                        <h2 class="StepTitle">Step 3 Content</h2>	
                        <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, 
                        sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
                        quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
                        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. 
                        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                        </p>
                        <p>
                        Lorem ipsum dolor sit amet, consectetur adipisicing elit, 
                        sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
                        quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
                        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. 
                        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                        </p>               				          
                    </div>
  			        <div id="step-4">
                        <h2 class="StepTitle">Step 4 Content</h2>	
                        <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, 
                        sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
                        quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
                        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. 
                        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                        </p>
                        <p>
                        Lorem ipsum dolor sit amet, consectetur adipisicing elit, 
                        sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
                        quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
                        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. 
                        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                        </p>
                        <p>
                        Lorem ipsum dolor sit amet, consectetur adipisicing elit, 
                        sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
                        quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
                        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. 
                        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                        </p>                			
                    </div>
  			        <div id="step-5">
                        <h2 class="StepTitle">Step 4 Content</h2>	
                        <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, 
                        sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
                        quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
                        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. 
                        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                        </p>
                        <p>
                        Lorem ipsum dolor sit amet, consectetur adipisicing elit, 
                        sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
                        quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
                        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. 
                        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                        </p>
                        <p>
                        Lorem ipsum dolor sit amet, consectetur adipisicing elit, 
                        sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, 
                        quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
                        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. 
                        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                        </p>                			
                    </div>
      		    </div>

                <asp:Label ID="LabelDashboardTemporaryContent" runat="server" /> <a href="/Pages/v5/Governance/Vote.aspx"><asp:Label ID="LabelGoThere2" runat="server" /></a>
            </div>
        </div>
        
        </div>
        <div class="sidebar">
    <h2 class="blue"><asp:Label ID="LabelSidebarInfoHeader" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
        <asp:Label ID="LabelSidebarInfoContent" runat="server" />
        </div>
    </div>
    
    <h2 class="blue"><asp:Label ID="LabelSidebarActionsHeader" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelSidebarActionsContent" runat="server" />
        </div>
    </div>
    
    <h2 class="orange"><asp:Label ID="LabelSidebarTodoHeader" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <div class="link-row-encaps" onclick="return false;" >
                <div class="link-row-icon" style="background-image:url('/Images/Icons/iconshock-databaseconnect-16px.png')"></div>
                <asp:Label ID="LabelSidebarTodoConnectDatabase" runat="server" />
            </div>
        </div>
    </div>
        </div>
        
	</div>

    <!-- language selector popup -->
    
    <telerik:RadToolTip ID="ToolTip" runat="server"  AnimationDuration="150" AutoCloseDelay="200000" ShowDelay="0"
            EnableShadow="true" HideDelay="1" Width="200px" Height="96px" HideEvent="ManualClose" OffsetX="-4" OffsetY="0"
            RelativeTo="Element" Animation="Slide" Position="BottomLeft" ShowCallout="true" TargetControlID="ImageCultureIndicator" RenderInPageRoot="true" ShowEvent="OnClick"
            Skin="Telerik" >
        <act5:LanguageSelector ID="LanguageSelector" runat="server" />
    </telerik:RadToolTip>
	
	</form>
</body>
</html>





        


