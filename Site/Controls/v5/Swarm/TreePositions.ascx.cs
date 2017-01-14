using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Common.Enums;

namespace Swarmops.Frontend.Controls.Swarm
{
    public partial class TreePositions : ControlV5Base
    {
        protected void Page_Init (object sender, EventArgs e)
        {
            // Request control framework in Init - the Load is too late for child controls

            ((PageV5Base) this.Page).RegisterControl (EasyUIControl.Tree | EasyUIControl.DataGrid);
        }

        protected void Page_Load (object sender, EventArgs e) // PreRender to make sure teh DropDuration control exists
        {
            this.DropPerson.Placeholder = Resources.Global.Swarm_TypeName;
            Localize();
        }

        private void Localize()
        {
            this.LiteralHeaderAction.Text = Resources.Global.Global_Action;
            this.LiteralHeaderName.Text = Resources.Global.Swarm_AssignedPerson;
            this.LiteralHeaderPosition.Text = Resources.Global.Swarm_Position;
            this.LiteralHeaderExpires.Text = Resources.Controls.Swarm.Positions_AssignmentExpires;
            this.LiteralHeaderMinMax.Text = Resources.Global.Global_MinMax;

            this.LabelAssignPersonTo.Text = Resources.Controls.Swarm.Positions_AssignPersonToPosition;
            this.LabelAssignmentDuration.Text = Resources.Controls.Swarm.Positions_AssignmentDuration;
            this.LabelModalHeader.Text = String.Format (Resources.Controls.Swarm.Positions_ModalHeader, this.ClientID);
            this.LiteralButtonAssign.Text = Resources.Controls.Swarm.Positions_ButtonAssign;

            this.DropDuration.Items.Add (new ListItem(Resources.Global.Timespan_Selection_OneMonth, "1"));
            this.DropDuration.Items.Add(new ListItem(Resources.Global.Timespan_Selection_TwoMonths, "2"));
            this.DropDuration.Items.Add(new ListItem(Resources.Global.Timespan_Selection_ThreeMonths, "3"));
            this.DropDuration.Items.Add(new ListItem(Resources.Global.Timespan_Selection_SixMonths, "6"));
            this.DropDuration.Items.Add(new ListItem(Resources.Global.Timespan_Selection_OneYear, "12"));
            this.DropDuration.Items.Add(new ListItem(Resources.Global.Timespan_Selection_TwoYears, "24"));
            this.DropDuration.Items.Add(new ListItem(Resources.Global.Timespan_Selection_UntilTermination, "-1"));

            this.LiteralTerminateNo.Text = JavascriptEscape (Resources.Controls.Swarm.Positions_TerminateNo);
            this.LiteralTerminateYes.Text = JavascriptEscape (Resources.Controls.Swarm.Positions_TerminateYes);
            this.LiteralTerminateSelfNo.Text = JavascriptEscape (Resources.Controls.Swarm.Positions_TerminateSelfNo);
            this.LiteralTerminateSelfYes.Text = JavascriptEscape (Resources.Controls.Swarm.Positions_TerminateSelfYes);
            this.LiteralConfirmTermination.Text = JavascriptEscape (Resources.Controls.Swarm.Positions_ConfirmTerminate);
            this.LiteralConfirmSelfTermination.Text =
                JavascriptEscape (Resources.Controls.Swarm.Positions_ConfirmSelfTerminate);

            this.DropDuration.SelectedValue = "12";

        }

        public PositionLevel Level { get; set; }
        public int OrganizationId { get; set; }
        public int GeographyId { get; set; }
        public string Cookie { get; set; }

        public int Height { get; set; }

        protected string HeightField
        {
            get
            {
                if (Height == 0)
                {
                    return string.Empty;
                }
                return String.Format(";height={0}px", Height);
            }
        }
    }
}