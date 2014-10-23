using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class InspectLedgers : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Read);
            this.DbVersionRequired = 0; // base schema is fine
            this.PageTitle = "Inspect Ledgers"; // TODO localize
            this.PageIcon = "iconshock-ledger-inspect";

            Localize();
        }

        private void Localize()
        {
            // TODO
        }
    }
}