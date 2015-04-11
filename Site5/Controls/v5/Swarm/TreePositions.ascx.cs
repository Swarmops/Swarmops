using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Controls.v5.Swarm
{
    public partial class TreePositions : ControlV5Base
    {
        protected void Page_Init (object sender, EventArgs e)
        {
            // Request control framework in Init - the Load is too late for child controls

            ((PageV5Base) this.Page).RegisterControl (EasyUIControl.Tree | EasyUIControl.DataGrid);
        }

        protected void Page_Load (object sender, EventArgs e)
        {
            Localize();
            this.DropPerson.Placeholder = Resources.Global.Swarm_TypeName;
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
            this.LiteralButtonAssign.Text = Resources.Controls.Swarm.Positions_ButtonAssign; // Wrongly flagged red-error by Resharper; this assignment is legit

            this.DropDuration.Items.Add (new ListItem(Resources.Global.Timespan_Selection_OneMonth, "1"));
            this.DropDuration.Items.Add(new ListItem(Resources.Global.Timespan_Selection_TwoMonths, "2"));
            this.DropDuration.Items.Add(new ListItem(Resources.Global.Timespan_Selection_ThreeMonths, "3"));
            this.DropDuration.Items.Add(new ListItem(Resources.Global.Timespan_Selection_SixMonths, "6"));
            this.DropDuration.Items.Add(new ListItem(Resources.Global.Timespan_Selection_OneYear, "12"));
            this.DropDuration.Items.Add(new ListItem(Resources.Global.Timespan_Selection_TwoYears, "24"));
            this.DropDuration.Items.Add(new ListItem(Resources.Global.Timespan_Selection_UntilTermination, "-1"));

            this.DropDuration.SelectedValue = "12";

        }

        public RoleLevel Level { get; set; }
        public int OrganizationId { get; set; }
        public int GeographyId { get; set; }
        public string Cookie { get; set; }


        public enum RoleLevel
        {
            Unknown = 0,
            Systemwide,
            Organizationwide,
            SuborganizationwideDefault,
            OrganizationTop,
            SuborganizationTopDefault,
            GeographicDefault,
            Geographic
        }
    }
}