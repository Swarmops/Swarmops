using System;
using Swarmops.Logic.Swarm;

namespace Swarmops.Controls.Financial
{
    public partial class ComboPeople : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public Person Selected { set; protected get; }

        public string OnClientSelect { set; protected get; }

        public bool NobodySelected { set; protected get; }
    }
}