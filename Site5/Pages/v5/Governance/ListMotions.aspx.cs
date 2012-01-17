using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Governance;
using Telerik.Web.UI;

namespace Activizr.Pages.Governance
{
    public partial class ListMotions : PageV5Base
    {
        private Meeting _meeting = Meeting.FromIdentity(1);
        private Dictionary<int, MotionAmendments> _amendmentLookup;

        public ListMotions()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageTitle = Resources.Pages.Governance.ListMotions_PageTitle;
            this.PageIcon = "iconshock-motions";

            this.LabelActionAddMotion.Text = Resources.Pages.Governance.AddMotion_PageTitle;
            this.LabelActionVote.Text = Resources.Pages.Governance.Vote_PageTitle;

            Localize();

            if (!this.IsPostBack)
            {
                PopulateGrid();
                this.GridMotions.DataBind();
            }

            ((Activizr.MasterV5) Master).LanguageChanged += new EventHandler(Master_LanguageChanged);

        }

        private void Master_LanguageChanged(object sender, EventArgs e)
        {
            // Grid must be re-localized

            LocalizeGrid();
            PopulateGrid();
            this.GridMotions.DataBind();
        }


        private void Localize()
        {
            this.LabelSidebarInfo.Text = Resources.Global.Sidebar_Information;
            this.LabelSidebarActions.Text = Resources.Global.Sidebar_Actions;
            this.LabelSidebarTodo.Text = Resources.Global.Sidebar_Todo;
            this.LabelActionItemsHere.Text = Resources.Global.Sidebar_Todo_Placeholder;

            this.LabelSidebarLookingAt.Text = Resources.Pages.Governance.ListMotions_Sidebar_Info;
            this.LabelMeetingName.Text = _meeting.Name;
            this.LabelChangeMeeting.Text = Resources.Global.Global_Change;

            LocalizeGrid();
        }


        private void LocalizeGrid()
        {
            TreeListColumnsCollection columns = this.GridMotions.Columns;

            foreach (TreeListColumn column in columns)
            {
                column.HeaderText = GetGlobalResourceObject("Pages.Governance", column.UniqueName).ToString();
            }
        }

        private void PopulateGrid()
        {
            Motions motions = Motions.ForMeeting(_meeting);
            MotionListRows rows = new MotionListRows();

            MotionAmendments amendments = MotionAmendments.ForMeeting(_meeting);

            _amendmentLookup = new Dictionary<int, MotionAmendments>();

            foreach (Motion motion in motions)
            {
                MotionListRow newRow = new MotionListRow(motion.Identity, 0, motion.SequenceNumber.ToString(), motion.Title,
                                                         0, false, false);
                rows.Add(newRow);
            }

            foreach (MotionAmendment amendment in amendments)
            {
                if (!_amendmentLookup.ContainsKey(amendment.MotionId))
                {
                    _amendmentLookup[amendment.MotionId] = new MotionAmendments();
                }

                _amendmentLookup[amendment.MotionId].Add(amendment);

                MotionListRow newRow = new MotionListRow(-amendment.Identity, amendment.MotionId, amendment.Designation, amendment.Title, 0, false, amendment.Carried);

                rows.Add(newRow);
            }

            // TODO: Sort rows by identity

            this.GridMotions.DataSource = rows;

            // TODO: Store in session var to enable paging
        }


        protected void GridMotions_ItemCreated(object sender, TreeListItemCreatedEventArgs e)
        {
            MotionListRow row = null;

            if (!(e.Item is TreeListDataItem))
            {
                return;
            }

            row = ((TreeListDataItem) e.Item).DataItem as MotionListRow;

            if (row == null)
            {
                return;
            }

            Literal literalAmendments = (Literal)e.Item.FindControl("LiteralAmendments");
            Literal literalTitle = (Literal)e.Item.FindControl("LiteralTitle");

            if (row.Identity > 0)
            {
                int amendmentCount = 0;

                if (_amendmentLookup.ContainsKey(row.Identity))
                {
                    amendmentCount = _amendmentLookup[row.Identity].Count;
                }

                literalAmendments.Text = String.Format("{0} (<a href=\"AddMotionAmendment.aspx?MotionId={1}\">{2}</a>)",
                                             amendmentCount, row.Identity,
                                             Resources.Pages.Governance.ListMotions_SubmitAmendment);

                literalTitle.Text = String.Format("<a href=\"ViewMotion.aspx?MotionId={0}\">{1}</a>", row.Identity,
                                                  HttpUtility.HtmlEncode(row.Title));
            }
            else
            {
                literalAmendments.Text = string.Empty;
                literalTitle.Text = HttpUtility.HtmlEncode(row.Title);
            }

            // 0 (<a href="AddMotionAmendment.aspx?MotionId=<%# Eval ("Identity") %>">Submit...</a>)
            //                     <a href="ViewMotion.aspx?MotionId=<%# Eval("Identity") %>"><%# Eval("Title") %></a>

        }


        public class MotionListRow
        {
            public MotionListRow (int identity, int parentIdentity, string designation, string title, int amendmentCount, bool amended, bool carried)
            {
                this.identity = identity;
                this.parentIdentity = parentIdentity;
                this.designation = designation;
                this.title = title;
                this.amendmentCount = amendmentCount;
                this.amended = amended;
                this.carried = carried;
            }

            private int identity;
            private int parentIdentity;
            private string designation;
            private string title;
            private int amendmentCount;
            private bool amended;
            private bool carried;

            public int ParentIdentity
            {
                get { return this.parentIdentity; }
            }

            public int Identity
            {
                get { return this.identity; }
            }

            public string Designation
            {
                get { return this.designation; }
            }

            public string Title
            {
                get { return this.title; }
            }
        }

        public class MotionListRows: List<MotionListRow>
        {
            // empty class; we just want the name
        }

        protected void GridMotions_NeedDataSource(object sender, TreeListNeedDataSourceEventArgs e)
        {
            LocalizeGrid();
            PopulateGrid(); // read this from session var instead, later
        }
    }
}