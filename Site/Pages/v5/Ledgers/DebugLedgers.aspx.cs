using System;
using Swarmops.Frontend;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

public partial class Pages_v5_Ledgers_DebugLedgers : PageV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Write);

        PageTitle = "Debug Ledgers";
        PageIcon = "iconshock-tester";

        // Initialize by mapping all

        // TODO: If this O(n^2) matching becomes teh suckage, optimize using hashtables over amounts

        Payouts.AutomatchAgainstUnbalancedTransactions (CurrentOrganization);

        // Iterate over all open payment groups and try to map them to unbalanced transactions

        FinancialTransactions unbalancedTransactions = FinancialTransactions.GetUnbalanced (CurrentOrganization);
        // TODO: this fn should move to Organization
        PaymentGroups openGroups = PaymentGroups.ForOrganization (CurrentOrganization, false);

        foreach (PaymentGroup openGroup in openGroups)
        {
            openGroup.MapTransaction (unbalancedTransactions);
        }
    }
}