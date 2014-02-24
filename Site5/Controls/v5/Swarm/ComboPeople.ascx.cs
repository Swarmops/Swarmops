using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Swarm;

namespace Swarmops.Controls.Financial
{
    public partial class ComboPeople : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public Person Selected { set; protected get; }

        public bool NobodySelected { set; protected get; }
    }
}