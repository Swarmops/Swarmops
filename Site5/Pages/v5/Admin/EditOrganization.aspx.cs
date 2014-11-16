using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Admin
{

    public partial class EditOrganization : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageIcon = "iconshock-box-cog";
            this.PageTitle = Resources.Pages.Admin.EditOrganization_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Admin.EditOrganization_Info;
            this.PageAccessRequired = new Access(CurrentOrganization, AccessAspect.Administration, AccessType.Write);

            if (!Page.IsPostBack)
            {
                Localize();
            }


            this.EasyUIControlsUsed = EasyUIControl.Tabs;
            this.IncludedControlsUsed = IncludedControl.FileUpload | IncludedControl.SwitchButton;
        }

        private void Localize()
        {
            string participants =
                Resources.Global.ResourceManager.GetString("Title_" + CurrentOrganization.RegularLabel + "_Plural");
            string participantship =
                Resources.Global.ResourceManager.GetString("Title_" + CurrentOrganization.RegularLabel + "_Ship");

            this.LabelParticipationEntry.Text =
                String.Format(Resources.Pages.Admin.EditOrganization_ParticipationBeginsWhen, participants);
            this.LabelParticipationOrg.Text =
                String.Format(Resources.Pages.Admin.EditOrganization_ParticipationBeginsOrg, participants);
            this.LabelParticipationDuration.Text =
                String.Format(Resources.Pages.Admin.EditOrganization_ParticipationDuration, participantship);
            this.LabelParticipationChurn.Text =
                String.Format(Resources.Pages.Admin.EditOrganization_ParticipationChurn, participantship);
            this.LabelRenewalCost.Text =
                String.Format(Resources.Pages.Admin.EditOrganization_RenewalsCost,
                    CurrentOrganization.Currency.DisplayCode);
            this.LabelParticipationCost.Text =
                String.Format(Resources.Pages.Admin.EditOrganization_ParticipationCost,
                    participantship, CurrentOrganization.Currency.DisplayCode);
            this.LabelRenewalsAffect.Text = Resources.Pages.Admin.EditOrganization_RenewalsAffect;
            this.LabelRenewalDateEffect.Text = Resources.Pages.Admin.EditOrganization_RenewalDateEffect;
            this.LabelRenewalReminder.Text = Resources.Pages.Admin.EditOrganization_RenewalReminders;

            this.DropMembersWhen.Items.Clear();
            this.DropMembersWhen.Items.Add(new ListItem("On application", "Application"));
            this.DropMembersWhen.Items.Add(new ListItem("On application + approval", "ApplicationApproval"));
            this.DropMembersWhen.Items.Add(new ListItem("On application + payment", "ApplicationPayment"));
            this.DropMembersWhen.Items.Add(new ListItem("Application, payment, approval", "ApplicationPaymentApproval"));
            this.DropMembersWhen.Items.Add(new ListItem("Invitation + acceptance", "InvitationAcceptance"));
            this.DropMembersWhen.Items.Add(new ListItem("Invitation + payment", "InvitationPayment"));
            this.DropMembersWhen.Items.Add(new ListItem("Manual add only", "Manual"));

            this.DropMembersWhere.Items.Clear();
            this.DropMembersWhere.Items.Add(new ListItem("Root organization only", "Root"));
            this.DropMembersWhere.Items.Add(new ListItem("Most local org only", "Local"));
            this.DropMembersWhere.Items.Add(new ListItem("Root and most local org", "RootLocal"));
            this.DropMembersWhere.Items.Add(new ListItem("All applicable organizations", "All"));

            this.DropMembershipDuration.Items.Clear();
            this.DropMembershipDuration.Items.Add(new ListItem("One month", "Month"));
            this.DropMembershipDuration.Items.Add(new ListItem("One year", "Year"));
            this.DropMembershipDuration.Items.Add(new ListItem("Two years", "TwoYears"));
            this.DropMembershipDuration.Items.Add(new ListItem("Five years", "FiveYears"));
            this.DropMembershipDuration.Items.Add(new ListItem("Forever", "Forever"));
            this.DropMembershipDuration.SelectedValue = "Year";

            this.DropMembersChurn.Items.Clear();
            this.DropMembersChurn.Items.Add(new ListItem("Expiry date reached", "Expiry"));
            this.DropMembersChurn.Items.Add(new ListItem("Not paid final reminder", "NotPaid"));
            this.DropMembersChurn.Items.Add(new ListItem("Never", "Never"));

            this.DropRenewalDateEffect.Items.Clear();
            this.DropRenewalDateEffect.Items.Add(new ListItem("Date of renewal", "RenewalDate"));
            this.DropRenewalDateEffect.Items.Add(new ListItem("Previous expiry", "FromExpiry"));

            this.DropRenewalsAffect.Items.Clear();
            this.DropRenewalsAffect.Items.Add(new ListItem("All related organizations", "All"));
            this.DropRenewalsAffect.Items.Add(new ListItem("One organization at a time", "One"));

            this.DropRenewalReminder.Items.Clear();
            this.DropRenewalReminder.Items.Add(new ListItem("30, 14, 7, 1 days before", "Standard"));
            this.DropRenewalReminder.Items.Add(new ListItem("Never", "Never"));
        }

        [WebMethod]
        public static CallResult FlickSwitch(string switchName, bool switchOn)
        {
            // TODO

            return new CallResult();
        }


        public class CallResult
        {
            public bool Success { get; set; }
            public string OpResult { get; set; }
            public string DisplayMessage { get; set; }
            public string RequiredOn { get; set; }
        }
    }

}