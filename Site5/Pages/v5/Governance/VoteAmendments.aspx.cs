using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Governance;
using Activizr.Logic.Pirates;
using Telerik.Web.UI;

namespace Activizr.Pages.Governance
{
    public partial class VoteAmendments : PageV5Base
    {
        private Meeting _meeting;
        private string _selectedRecommendation;

        protected void Page_Init (object sender, EventArgs e)
        {
            _selectedRecommendation = Request.Form["ctl00$PlaceHolderMain$DropRecommendations"];
            if (String.IsNullOrEmpty(_selectedRecommendation))
            {
                _selectedRecommendation = "0";
            }

            /*
            PopulateGrid();
            this.GridAmendmentsVote.DataBind();*/
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string motionIdString = Request.QueryString["MeetingId"];
            int meetingId = Int32.Parse(motionIdString);
            _meeting = Meeting.FromIdentity(meetingId);

            // TODO: Authorize

            // Localize

            this.PageTitle = Resources.Pages.Governance.VoteAmendments_PageTitle;
            this.PageIcon = "iconshock-motion-amendments-vote";

            this.LabelActionListMotions.Text = Resources.Pages.Governance.ListMotions_PageTitle;

            // Fill in fields

            LocalizeGrid();
            Localize();

            if (!Page.IsPostBack)
            {
                PopulateGrid();
                PopulateRecommendations();
            }

            this.DropRecommendations.Style[HtmlTextWriterStyle.Position] = "relative";
            this.DropRecommendations.Style[HtmlTextWriterStyle.Top] = "3px";
        }

        private void LocalizeGrid()
        {
            TreeListColumnsCollection columns = this.GridAmendmentsVote.Columns;

            foreach (TreeListColumn column in columns)
            {
                column.HeaderText = GetGlobalResourceObject("Pages.Governance", column.UniqueName).ToString();
            }
        }

        private void Localize()
        {
            this.LabelActionListMotions.Text = Resources.Pages.Governance.ListMotions_PageTitle;
            this.LabelActionItemsHere.Text = Resources.Pages.Global.Sidebar_Todo_Placeholder;
            this.LabelAmendmentsForMeeting.Text = _meeting.Name;
            this.LabelSidebarInfo.Text = Resources.Pages.Global.Sidebar_Information;
            this.LabelSidebarActions.Text = Resources.Pages.Global.Sidebar_Actions;
            this.LabelSidebarTodo.Text = Resources.Pages.Global.Sidebar_Todo;
            this.ButtonSaveVote.Text = Resources.Pages.Governance.VoteAmendments_SaveVote_Temp;

            this.LabelInfo.Text = Resources.Pages.Governance.VoteAmendments_Info;
        }

        private void PopulateRecommendations()
        {
            this.DropRecommendations.Items.Clear();
            this.DropRecommendations.Items.Add(new ListItem(Resources.Pages.Governance.VoteAmendments_SelectRecommendation, "0"));
            this.DropRecommendations.Items.Add(new ListItem(Resources.Pages.Governance.VoteAmendments_BoardRecommendation, "-1"));
            this.DropRecommendations.Items.Add(new ListItem("DinLista - " + _currentUser.Canonical, "3"));
            this.DropRecommendations.Items.Add(new ListItem("TestList - Rick Falkvinge (#1)", "1"));
        }


        private void PopulateGrid()
        {
            // Use lazy method. This can be optimized plenty.

            List<GridRowItem> items = new List<GridRowItem>();
            Motions motions = Motions.ForMeeting(_meeting);

            foreach (Motion motion in motions)
            {
                items.Add(new GridRowItem(String.Format(Resources.Pages.Governance.ViewMotion_MotionDesignation, motion.SequenceNumber), motion.Title, motion.Submitter, -motion.Identity, 0));
                items.Add(new GridRowItem(Resources.Pages.Governance.VoteAmendments_Grid_Text, motion.Text, null, -motion.Identity + 210000, -motion.Identity));
                items.Add(new GridRowItem(Resources.Pages.Governance.VoteAmendments_Grid_Decision, motion.DecisionPoints, null, -motion.Identity + 220000, -motion.Identity));

                MotionAmendments amendments = MotionAmendments.ForMotion(motion);

                foreach (MotionAmendment amendment in amendments)
                {
                    items.Add(new GridRowItem("#" + amendment.Designation, amendment.Title, amendment.Submitter, amendment.Identity, 0));
                    items.Add(new GridRowItem(Resources.Pages.Governance.VoteAmendments_Grid_Rationale, amendment.Text, null,
                                              amendment.Identity + 100000, amendment.Identity));
                    items.Add(new GridRowItem(Resources.Pages.Governance.VoteAmendments_Grid_Change,
                                              amendment.DecisionPoint, null, amendment.Identity + 200000, amendment.Identity));
                }
            }

            this.GridAmendmentsVote.DataSource = items;
        }


        public class GridRowItem
        {
            public GridRowItem(string designation, string text, Person submitter, int identity, int parentIdentity)
            {
                this.TextField = text;
                this.DesignationField = designation;
                this.Submitter = submitter;
                this.IdentityField = identity;
                this.ParentIdentityField = parentIdentity;
            }

            public string Title;
            public string TextField;
            public string DesignationField;
            public Person Submitter;
            public int IdentityField;
            public int ParentIdentityField;

            public int Identity
            {
                get { return this.IdentityField; }
            }

            public int ParentIdentity
            {
                get { return this.ParentIdentityField; }
            }

            public string Designation
            {
                get { return this.DesignationField;}
            }

            public string Text
            {
                get { return this.TextField;  }
            }
        }

        protected void GridAmendments_NeedDataSource(object sender, TreeListNeedDataSourceEventArgs e)
        {
            PopulateGrid();
        }

        protected void GridAmendmentsVote_ItemCreated(object sender, TreeListItemCreatedEventArgs e)
        {
            GridRowItem row = null;

            if (!(e.Item is TreeListDataItem))
            {
                return;
            }

            row = ((TreeListDataItem)e.Item).DataItem as GridRowItem;

            if (row == null)
            {
                return;
            }

            RadioButton radioYes = (RadioButton)e.Item.FindControl("RadioYes");
            RadioButton radioNo = (RadioButton)e.Item.FindControl("RadioNo");
            RadioButton radioAbstain = (RadioButton)e.Item.FindControl("RadioAbstain");
            Label labelRecommendation = (Label)e.Item.FindControl("LabelRecommendation");

            labelRecommendation.Style[HtmlTextWriterStyle.FontWeight] = "700";
            labelRecommendation.Style[HtmlTextWriterStyle.Color] = "#880";

            bool showButtons = false;

            if (row.Identity > 0 && row.Identity < 100000)
            {
                showButtons = true;

                string recommendation = Resources.Pages.Governance.VoteAmendments_Recommend_Abstain;

                radioYes.GroupName = 
                    radioNo.GroupName = 
                    radioAbstain.GroupName = "Amendment" + row.Identity.ToString();
                
                if (_selectedRecommendation != "0")
                {
                    switch ((Convert.ToInt32(_selectedRecommendation) + row.Identity) % 3)
                    {
                        case 1:
                            recommendation = Resources.Pages.Governance.VoteAmendments_Recommend_Yes;
                            labelRecommendation.Style[HtmlTextWriterStyle.Color] = "#080";
                            break;
                        case 2:
                            recommendation = Resources.Pages.Governance.VoteAmendments_Recommend_No;
                            labelRecommendation.Style[HtmlTextWriterStyle.Color] = "#800";
                            break;
                        default:
                            break;
                    }
                }

                labelRecommendation.Text = recommendation;

                // Repopulate the state directly from the form

                string previousState = null;
                
                foreach (string key in Request.Form.AllKeys)
                {
                    if (key.EndsWith(radioYes.GroupName))
                    {
                        previousState = Request.Form[key];
                    }
                }

                if (!String.IsNullOrEmpty(previousState))
                {
                    if (previousState == "RadioYes")
                    {
                        radioYes.Checked = true;
                    }
                    else if (previousState == "RadioNo")
                    {
                        radioNo.Checked = true;
                    }
                    else if (previousState == "RadioAbstain")
                    {
                        radioAbstain.Checked = true;
                    }
                }
            }

            radioYes.Visible = showButtons;
            radioNo.Visible = showButtons;
            radioAbstain.Visible = showButtons;
            labelRecommendation.Visible = showButtons;
        }

        protected void DropRecommendations_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
            this.GridAmendmentsVote.DataBind();
        }
    }
}