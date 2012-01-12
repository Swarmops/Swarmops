using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Financial;
using Activizr.Logic.Security;

public partial class Pages_v5_Ledgers_DebugLedgers : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageAccessRequired = new Access(_currentOrganization, AccessAspect.Bookkeeping, AccessType.Write);

        this.PageTitle = "Debug Ledgers";
        this.PageIcon = "iconshock-tester";

        // Initialize by mapping all

        // TODO: If this O(n^2) matching becomes teh suckage, optimize using hashtables over amounts

        // Iterate over all open payment groups and try to map them to unbalanced transactions

        FinancialTransactions unbalancedTransactions = FinancialTransactions.GetUnbalanced(_currentOrganization); // TODO: this fn should move to Organization
        PaymentGroups openGroups = PaymentGroups.ForOrganization(_currentOrganization, false);

        foreach (PaymentGroup openGroup in openGroups)
        {
            openGroup.MapTransaction(unbalancedTransactions);
        }
    }
}