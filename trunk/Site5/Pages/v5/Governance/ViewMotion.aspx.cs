using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Governance;

namespace Activizr.Pages.Governance
{
    public partial class ViewMotion : PageV5Base
    {
        private Motion _motion;
        private Meeting _meeting;

        protected void Page_Load(object sender, EventArgs e)
        {
            string motionIdString = Request.QueryString["MotionId"];
            int motionId = Int32.Parse(motionIdString);
            _motion = Motion.FromIdentity(motionId);
            _meeting = _motion.Meeting;

            // TODO: Authorize

            // TODO: Get # of posts in discussion thread

            int discussionPostCount = 0;

            // Localize

            this.PageTitle = Resources.Pages.Governance.ViewMotion_PageTitle;
            this.PageIcon = "iconshock-motion";

            this.LabelActionListMotions.Text = Resources.Pages.Governance.ListMotions_PageTitle;

            this.LabelDiscussLabel.Text = Resources.Pages.Governance.ViewMotion_Discuss;
            this.LabelMotionDecisionsLabel.Text = Resources.Pages.Governance.ViewMotion_DecisionPoints;
            this.LabelMotionDesignation.Text = String.Format(Resources.Pages.Governance.ViewMotion_MotionDesignation, _motion.SequenceNumber);
            this.LabelMotionTextLabel.Text = Resources.Pages.Governance.ViewMotion_Text;
            this.LabelViewingMotionInfo.Text = Resources.Pages.Governance.ViewMotion_ViewingMotionInfo;

            this.LabelSidebarInfo.Text = Resources.Pages.Global.Sidebar_Information;
            this.LabelSidebarActions.Text = Resources.Pages.Global.Sidebar_Actions;
            this.LabelSidebarTodo.Text = Resources.Pages.Global.Sidebar_Todo;
            this.LabelActionItemsHere.Text = Resources.Pages.Global.Sidebar_Todo_Placeholder;


            // Fill in fields

            this.LabelMeetingName.Text = _meeting.Name;
            this.LabelMotionText.Text = _motion.Text;
            this.LabelMotionTitle.Text = _motion.Title;
            this.LinkDiscussion.Text = String.Format(Resources.Pages.Governance.ViewMotion_DiscussLink, discussionPostCount);

            // construct motion text field (a literal, we make it an OL field, need to take care to HTML encode text)

            string[] decisionPoints = _motion.DecisionPoints.Split('\r', '\n');

            string literal = "<ol class=\"motion-proposed-decisions\">";
            foreach (string decisionPoint in decisionPoints)
            {
                string proposedDecision = decisionPoint.Trim();

                if (proposedDecision.Length > 0)
                {
                    literal += "<li>" + HttpUtility.HtmlEncode(proposedDecision) + "</li>";
                }
            }
            literal += "</ol>";

            this.LiteralMotionDecisions.Text = literal;

            MotionAmendments amendments = _motion.Amendments;

            this.RepeaterAmendments.DataSource = amendments;
            this.RepeaterAmendments.DataBind();

            if (amendments.Count > 1)
            {
                this.LabelAmendmentCount.Text = String.Format(Resources.Pages.Governance.ViewMotion_AmendmentCountTwoPlus,
                                                              amendments.Count);
            }
            else if (amendments.Count == 1)
            {
                this.LabelAmendmentCount.Text = Resources.Pages.Governance.ViewMotion_AmendmentCountOne;
            }
            else  // zero
            {
                this.LabelAmendmentCount.Text = Resources.Pages.Governance.ViewMotion_AmendmentCountZero;
            }

            this.LinkAddAmendment.NavigateUrl = "AddMotionAmendment.aspx?MotionId=" + _motion.Identity.ToString();
            this.LinkAddAmendment.Text = Resources.Pages.Governance.ViewMotion_AddAmendment;
        }
    }
}