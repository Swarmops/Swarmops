using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Governance;

namespace Activizr.Pages.Governance
{

    public partial class Vote : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int motionCount = Motions.ForMeeting(Meeting.FromIdentity(1)).Count;
            int amendmentCount = MotionAmendments.ForMeeting(Meeting.FromIdentity(1)).Count;

            this.PageTitle = Resources.Pages.Governance.Vote_PageTitle;
            this.PageIcon = "iconshock-vote";

            this.LabelSidebarInfo.Text = Resources.Pages.Global.Sidebar_Information;
            this.LabelSidebarActions.Text = Resources.Pages.Global.Sidebar_Actions;
            this.LabelSidebarTodo.Text = Resources.Pages.Global.Sidebar_Todo;
            this.LabelActionItemsHere.Text = Resources.Pages.Global.Sidebar_Actions_Placeholder;

            this.LabelActionListMotions.Text = Resources.Pages.Governance.ListMotions_PageTitle;
            this.LabelVotingInfo.Text = String.Format(Resources.Pages.Governance.Vote_Info, _currentOrganization.Name);

            this.LabelPointsOfOrderHeader.Text = Resources.Pages.Governance.Vote_PointsOfOrder;
            this.LabelPointsOfOrderTemp.Text = Resources.Pages.Governance.Vote_PointsOfOrderTemp;

            this.LabelVoteAmendmentsLabel.Text = String.Format(Resources.Pages.Governance.Vote_VoteOnAmendments,
                                                               amendmentCount);
            this.LabelVoteMotionsLabel.Text = String.Format(Resources.Pages.Governance.Vote_VoteOnMotions, motionCount);

            this.LabelCreateVotingList.Text = Resources.Pages.Governance.Vote_CreateVotingList;
            this.LabelVoteHere.Text = Resources.Pages.Governance.Vote_VoteHere;

            this.LabelElectionsHeader.Text = Resources.Pages.Governance.Vote_MeetingElections;
            this.LabelMotionsHeader.Text = Resources.Pages.Governance.Vote_Motions;
        }
    }
}