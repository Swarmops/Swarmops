using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Governance;
using Activizr.Logic.Pirates;

namespace Activizr.Pages.Governance
{

    public partial class AddMotion : PageV5Base
    {
        private Meeting _meeting = Meeting.FromIdentity(1);

        protected void Page_Load(object sender, EventArgs e)
        {
            // Stylize

            this.PageIcon = "iconshock-motion-add";
            this.TextTitle.Style[HtmlTextWriterStyle.Width] = "300px";
            this.PersonSubmitter.Width = 304;  // !?
            this.TextMotionText.Style[HtmlTextWriterStyle.Width] = "300px";
            this.TextDecisionPoints.Style[HtmlTextWriterStyle.Width] = "300px";
            this.TextMotionText.Style[HtmlTextWriterStyle.Height] = "200px";

            // Localize

            this.PageTitle = Resources.Pages.Governance.AddMotion_PageTitle;

            this.LabelMeetingName.Text = _meeting.Name;
            this.LabelWrongMeeting.Text = Resources.Pages.Governance.AddMotion_WrongMeeting;
            this.LabelSubmittingMotionInfo.Text = Resources.Pages.Governance.AddMotion_SubmittingMotionInfo;
            this.LabelTitle.Text = Resources.Pages.Governance.AddMotion_Title;
            this.LabelSubmitter.Text = Resources.Pages.Governance.AddMotion_Submitter;
            this.LabelText.Text = Resources.Pages.Governance.AddMotion_Text;
            this.LabelDecisions.Text = Resources.Pages.Governance.AddMotion_Decisions;

            this.LabelSidebarInfo.Text = Resources.Pages.Global.Sidebar_Information;
            this.LabelSidebarActions.Text = Resources.Pages.Global.Sidebar_Actions;
            this.LabelSidebarTodo.Text = Resources.Pages.Global.Sidebar_Todo;
            this.LabelActionItemsHere.Text = Resources.Pages.Global.Sidebar_Actions_Placeholder;

            this.ButtonAddMotion.Text = Resources.Pages.Governance.AddMotion_SubmitMotion;

            this.LabelActionListMotions.Text = Resources.Pages.Governance.ListMotions_PageTitle;

            if (!Page.IsPostBack)
            {
                this.PersonSubmitter.SelectedPerson = _currentUser;

                // HACK: Hardcode the presidency of this meeting

                bool isPresidency = false;

                switch (_currentUser.Identity)
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

        protected void ButtonAddMotion_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                this.LabelMotionSaved.Text = string.Empty;
                return; // Do nothing if validators fail the page
            }

            Motion newMotion = Motion.Create(_meeting, this.PersonSubmitter.SelectedPerson, _currentUser,
                                             this.TextTitle.Text, this.TextMotionText.Text, this.TextDecisionPoints.Text);

            this.LabelMotionSaved.Text = String.Format(Resources.Pages.Governance.AddMotion_MotionSubmittedAs,
                                                       newMotion.SequenceNumber);

            this.TextDecisionPoints.Text = string.Empty;
            this.TextMotionText.Text = string.Empty;
            this.TextTitle.Text = string.Empty;

            if (this.PersonSubmitter.Enabled)
            {
                this.PersonSubmitter.SelectedPerson = null;
            }

            this.TextTitle.Focus();
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
    }
}