using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Activizr.Basic.Enums;
using Activizr.Logic.Communications;

public partial class Pages_v4_AlertActivists : PageV4Base
{


    protected void Page_Load (object sender, EventArgs e)
    {
        this.pagePermissionDefault = new PermissionSet(Permission.CanSendLocalMail);

        PanelTestSent.Visible = false;

        if (!Page.IsPostBack)
        {
            Geographies geoList = _authority.GetGeographiesForOrganization(Organization.PPSE);  // Hack. Should be org agnostic.
            geoList = geoList.RemoveRedundant();
            ViewState["actingOrg"] = Organization.PPSEid;

            if (geoList.Count == 0)
            {
                // Quick Hack for PPFI
                geoList = _authority.GetGeographiesForOrganization(Organization.FromIdentity(Organization.PPFIid));
                geoList = geoList.RemoveRedundant();
                if (geoList.Count != 0)
                {
                    ViewState["actingOrg"] = Organization.PPFIid;
                }
            }


            if (geoList.Count == 0)
            {
                ViewState["actingOrg"] = 0;

                this.LabelGeographies.Text = "You don't have access to any geographic area.";
            }
            else
            {
                this.GeographyTree.Roots = geoList;
            }

            UpdateActivistCount();
            UpdateSmsCosts();

            if (PhoneMessageTransmitter.CheckServiceStatus() == false)
            {
                CheckSms.Enabled = false;
                CheckSms.Text = "SMS-service not available currently. No reply from server.";
            }

        }
        if (Organization.PPSEid != (int)ViewState["actingOrg"])
        {
            SmsPanel.Visible = false;
        }
        TextSms.Attributes["onkeydown"] = "updateSmsLength(this);";

        //Add the necessary AJAX setting programmatically -- commented out
        // RadAjaxManager1.AjaxSettings.AddAjaxSetting(GeographyTree.FindControl("DropGeographies"), this.PanelRestOfPage);
        // RadAjaxManager1.AjaxSettings.AddAjaxSetting(GeographyTree.FindControl("DropGeographies"), this.LabelGeographies);
        // Controls_v4_WSGeographyTreeDropDown.EmitScripts(this);
    }


    protected void CheckSms_CheckedChanged (object sender, EventArgs e)
    {
        UpdateSmsCosts();
    }

    private void UpdateSmsCosts ()
    {
        this.PanelSmsText.Visible = this.CheckSms.Checked;

        if (this.CheckSms.Checked)
        {
            Geography selectedGeo = this.GeographyTree.SelectedGeography;
            int activistCount = Activists.GetCountForGeography(selectedGeo);

            ResetSmsCosts(selectedGeo, activistCount);
        }
        else
        {
            this.LabelSmsCost.Text = "0.00";
            this.LabelBudget.Text = "None";
        }
    }

    protected void CheckMail_CheckedChanged (object sender, EventArgs e)
    {
        this.PanelMailText.Visible = this.CheckMail.Checked;
    }

    protected void ResetSmsCosts (Geography geo, int activistCount)
    {
        FinancialAccount budget = SelectBudget(geo);

        double cost = PhoneMessageTransmitter.SMSCost * (activistCount + 2);   // +2 because of status message before and after transmission
        this.LabelSmsCost.Text = cost.ToString("N2");
        this.LabelBudget.Text = budget.Name;
    }

    protected FinancialAccount SelectBudget (Geography geo)
    {
        //TODO: Greate a general funtion to select local budget based on geography
        int financialAccountId = 0;

        if (geo.Identity == 32 || geo.Inherits(32))
        {
            financialAccountId = 18;
        }
        else if (geo.Identity == 33 || geo.Inherits(33))
        {
            financialAccountId = 15;
        }
        else if (geo.Identity == 34 || geo.Inherits(34))
        {
            financialAccountId = 17;
        }
        else if (geo.Identity == 35 || geo.Inherits(35))
        {
            financialAccountId = 16;
        }
        else if (geo.Identity == 39 || geo.Inherits(39))
        {
            financialAccountId = 19;
        }
        else
        {
            financialAccountId = 27;
        }

        return FinancialAccount.FromIdentity(financialAccountId);
    }

    protected Geography GetTopGeography ()
    {

        int actingOrgId = (int)ViewState["actingOrg"];

        if (actingOrgId == 0)
        {
            return null;
        }

        Organization actingOrg = Organization.FromIdentity(actingOrgId);

        Geographies geoList = _authority.GetGeographiesForOrganization(actingOrg);
        geoList = geoList.RemoveRedundant();

        if (geoList.Count > 1)
        {
            // Problem. Alert Rick.
        }
        if (geoList.Count < 1)
        {
            return null;
        }

        return geoList[0];
    }

    protected void GeographyTreeDropDown_SelectedNodeChanged (object sender, EventArgs e)
    {
        UpdateActivistCount();

    }

    private void UpdateActivistCount ()
    {
        Geography selectedGeo = this.GeographyTree.SelectedGeography;
        int activistCount = Activists.GetCountForGeography(selectedGeo);

        string result = " No activist in " + selectedGeo.Name + ". Not even one! Nobody will receive your alert.";

        if (activistCount == 1)
        {
            result = " One activist in " + selectedGeo.Name + ". Just one. Hardly a mobilization, eh?";
        }
        else if (activistCount > 1)
        {
            result = " " + activistCount.ToString() + " activists in " + selectedGeo.Name + ".";
        }

        this.LabelGeographies.Text = result;
        UpdateSmsCosts();
        this.ButtonSend.Text = "Send (to " + activistCount.ToString() + ")";
    }

    protected void ButtonTest_Click (object sender, EventArgs e)
    {
        int actingOrgId = (int)ViewState["actingOrg"];

        if (actingOrgId == 0)
        {
            ErrorMsg.Text = "No organisation!";
            return;
        }
        Geography geography = this.GeographyTree.SelectedGeography;
        Organization actingOrg = Organization.FromIdentity(actingOrgId);

        if (this.CheckSms.Checked)
        {
            string smsText = this.TextSms.Text;
            if (!string.IsNullOrEmpty(smsText) && smsText.Trim().Length > 3) // 3 characters is too small message, should always be more
            {
                _currentUser.SendPhoneMessage("PP: " + smsText.Trim());

                ChargeBudget(SelectBudget(geography), PhoneMessageTransmitter.SMSCost, "Test SMS");
            }
        }

        if (this.CheckMail.Checked)
        {

            ActivistMail activistMail = new ActivistMail();

            activistMail.pSubject = this.TextMailSubject.Text;
            activistMail.pBodyContent = this.TextMailBody.Text;
            activistMail.pOrgName = actingOrg.MailPrefixInherited;
            activistMail.pGeographyName = (geography.Identity == Geography.RootIdentity ? "" : geography.Name);

            OutboundMail mail = activistMail.CreateFunctionalOutboundMail(MailAuthorType.ActivistService, OutboundMail.PriorityNormal, actingOrg, geography);

            mail.AddRecipient(_currentUser, false);
            mail.SetRecipientCount(1);
            mail.SetResolved();
            mail.SetReadyForPickup();

        }
        PanelTestSent.Visible = true;
    }

    protected void ButtonSend_Click (object sender, EventArgs e)
    {
        int actingOrgId = (int)ViewState["actingOrg"];

        if (actingOrgId == 0)
        {
            ErrorMsg.Text = "No organisation!";
            return;
        }



        Button theBtn = (sender as Button);

        ErrorMsg.Text = TimedDisableButton(theBtn, "AlertActivistLastSend", 60); // Should it even be possible to push the button again without reloading page?
        if (ErrorMsg.Text != "")
            return;

        Activists activists = Activists.FromGeography(this.GeographyTree.SelectedGeography);

        if (this.CheckSms.Checked && actingOrgId == Organization.PPSEid)
        {
            string smsText = this.TextSms.Text;
            if (!string.IsNullOrEmpty(smsText) && smsText.Trim().Length > 3) // 3 characters is too small message, should always be more
            {
                // Defer sending SMS messages to bot

                Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.PhoneMessagesCreated,
                                                           _currentUser.Identity,
                                                           actingOrgId, this.GeographyTree.SelectedGeography.Identity, 0,
                                                           SelectBudget(this.GeographyTree.SelectedGeography).Identity, "PP: " + smsText.Trim());
            }
        }

        if (this.CheckMail.Checked)
        {
            string mailSubject = this.TextMailSubject.Text;
            string mailBody = this.TextMailBody.Text;

            if (!string.IsNullOrEmpty(mailSubject) && !string.IsNullOrEmpty(mailBody))
            {
                Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.ActivistMailsCreated,
                                                             _currentUser.Identity,
                                                             actingOrgId,
                                                             this.GeographyTree.SelectedGeography.Identity, 0,
                                                             0,
                                                             mailSubject.Replace("|", "") + "|" + mailBody);
            }
        }

        this.PanelFinal.Visible = true;
        PanelTestSent.Visible = false;
    }

    private string TimedDisableButton (Button theBtn, string sessVarName, int disableTime)
    {
        string errortext = "";
        DateTime lastSend;
        if (Session[sessVarName] != null)
        {
            try
            {
                lastSend = (DateTime)Session[sessVarName];
                //Just ignore click if within 5 seconds;
                if (DateTime.Now.Subtract(lastSend).TotalSeconds < disableTime)
                {
                    errortext = "Ignored click, please wait a sec...";
                }
            }
            catch (Exception)
            { }
        }
        Session[sessVarName] = DateTime.Now;

        ScriptManager.RegisterStartupScript(
            this, this.GetType(), "reenableButton" + theBtn.ClientID,
            "document.getElementById('" + theBtn.ClientID + "').disabled=true;setTimeout(\"document.getElementById('" + theBtn.ClientID + "').disabled=false\"," + (disableTime * 1000).ToString() + ")", true);
        return errortext;
    }


    protected void ChargeBudget (FinancialAccount budget, double amount, string comment)
    {

        FinancialTransaction newTransaction = FinancialTransaction.Create(1, DateTime.Now, comment);
        newTransaction.AddRow(budget, amount, _currentUser);
        newTransaction.AddRow(Organization.FromIdentity(Organization.PPSEid).FinancialAccounts.CostsInfrastructure, -amount, _currentUser);
    }
}