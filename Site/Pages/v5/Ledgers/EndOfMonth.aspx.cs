using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.Ledgers
{
    public partial class EndOfMonth : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.BookkeepingDetails);
        }


        public string JavascriptDocReady
        {
            get
            {
                return string.Empty;
                
            }
        }
    }
}