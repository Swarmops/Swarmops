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
        }

        private void Localize()
        {
            this.LiteralHeaderAction.Text = Resources.Global.Global_Action;
            this.LiteralHeaderName.Text = Resources.Global.Swarm_AssignedPerson;
            this.LiteralHeaderPosition.Text = Resources.Global.Swarm_Position;
            this.LiteralHeaderExpires.Text = Resources.Global.Swarm_AssignmentExpires;
            this.LiteralHeaderMinMax.Text = Resources.Global.Global_MinMax;
        }

        public RoleLevel Level { get; set; }
        public int OrganizationId { get; set; }
        public int GeographyId { get; set; }


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