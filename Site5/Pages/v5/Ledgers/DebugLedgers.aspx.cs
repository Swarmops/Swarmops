using System;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

public partial class Pages_v5_Ledgers_DebugLedgers : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Write);

        this.PageTitle = "Debug Ledgers";
        this.PageIcon = "iconshock-tester";

        // Initialize by mapping all

        // TODO: If this O(n^2) matching becomes teh suckage, optimize using hashtables over amounts

        Payouts.AutomatchAgainstUnbalancedTransactions(this.CurrentOrganization);

        // Iterate over all open payment groups and try to map them to unbalanced transactions

        FinancialTransactions unbalancedTransactions = FinancialTransactions.GetUnbalanced(this.CurrentOrganization); // TODO: this fn should move to Organization
        PaymentGroups openGroups = PaymentGroups.ForOrganization(this.CurrentOrganization, false);

        foreach (PaymentGroup openGroup in openGroups)
        {
            openGroup.MapTransaction(unbalancedTransactions);
        }
    }
}