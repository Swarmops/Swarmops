using System;

namespace Swarmops.Controls.Financial
{
    public partial class ComboBudgets : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string OnClientLoaded { get; set; }
        public string OnClientSelect { get; set; }
    }
}