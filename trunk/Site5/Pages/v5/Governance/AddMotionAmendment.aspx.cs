using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Governance;

namespace Swarmops.Pages.Governance
{
    public partial class AddMotionAmendment : PageV5Base
    {
        private Motion _motion;

        protected void Page_Load(object sender, EventArgs e)
        {
            string motionIdString = Request.QueryString["MotionId"];
            int motionId = Int32.Parse(motionIdString);
            _motion = Motion.FromIdentity(motionId);

            // Stylize

            this.PageIcon = "iconshock-motion-amendment-add";
            this.TextTitle.Style[HtmlTextWriterStyle.Width] = "300px";
            this.PersonSubmitter.Width = 304;  // !?
            this.TextAmendmentText.Style[HtmlTextWriterStyle.Width] = "300px";
            this.TextDecisionPoint.Style[HtmlTextWriterStyle.Width] = "300px";
            this.TextAmendmentText.Style[HtmlTextWriterStyle.Height] = "200px";

            // Localize

            this.PageTitle = Resources.Pages.Governance.AddMotionAmendment_PageTitle;

            this.LabelMotionName.Text = _motion.Title;
            this.LabelSubmittingAmendmentInfo.Text = Resources.Pages.Governance.AddMotionAmendment_SubmittingAmendmentInfo;
            this.LabelTitle.Text = Resources.Pages.Governance.AddMotionAmendment_Title;
            this.LabelSubmitter.Text = Resources.Pages.Governance.AddMotionAmendment_Submitter;
            this.LabelText.Text = Resources.Pages.Governance.AddMotionAmendment_Text;
            this.LabelDecision.Text = Resources.Pages.Governance.AddMotionAmendment_Decision;

            this.LabelSidebarInfo.Text = Resources.Global.Sidebar_Information;
            this.LabelSidebarActions.Text = Resources.Global.Sidebar_Actions;
            this.LabelSidebarTodo.Text = Resources.Global.Sidebar_Todo;
            this.LabelActionItemsHere.Text = Resources.Global.Sidebar_Todo_Placeholder;

            this.ButtonAddMotionAmendment.Text = Resources.Pages.Governance.AddMotionAmendment_SubmitAmendment;

            this.LabelActionListMotions.Text = Resources.Pages.Governance.ListMotions_PageTitle;

            if (!Page.IsPostBack)
            {
                this.PersonSubmitter.SelectedPerson = CurrentUser;

                // HACK: Hardcode the presidency of this meeting

                bool isPresidency = false;

                switch (CurrentUser.Identity)
                {
                    case 1681: // Nils Agnesson
                    case 15719: // Andreas Bjärnemalm
                    case 1185: // Jonatan Kindh
                    case 7838: // Jörgen Lindell
                    case 16761: // Mikael Holm
                    case 14843: // Staffan Jonsson
                    case 71888: // Henrik Brändén
                        {
                            isPresidency = true;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                this.PersonSubmitter.Enabled = isPresidency;

                this.TextTitle.Focus();
            }
        }

        protected void ValidatorSubmitterRequired_Validate(object source, ServerValidateEventArgs args)
        {
            if (this.PersonSubmitter.SelectedPerson == null)
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void ButtonAddAmendment_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                this.LabelAmendmentSaved.Text = string.Empty;
                return; // Do nothing if validators fail the page
            }

            MotionAmendment newAmendment = MotionAmendment.Create(_motion, this.TextTitle.Text, this.TextAmendmentText.Text,
                                                                  this.TextDecisionPoint.Text,
                                                                  this.PersonSubmitter.SelectedPerson, this.CurrentUser);

            this.LabelAmendmentSaved.Text = String.Format(Resources.Pages.Governance.AddMotionAmendment_AmendmentSubmittedAs,
                                                       _motion.SequenceNumber, (char) ((int)('A') + newAmendment.SequenceNumber - 1));

            this.TextDecisionPoint.Text = string.Empty;
            this.TextAmendmentText.Text = string.Empty;
            this.TextTitle.Text = string.Empty;

            if (this.PersonSubmitter.Enabled)
            {
                this.PersonSubmitter.SelectedPerson = null;
            }

            this.TextTitle.Focus();
        }
    }
}