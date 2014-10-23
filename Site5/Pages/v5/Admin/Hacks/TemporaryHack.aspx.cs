using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// This page executes code on page load. Used for various tests of logic. Not supposed to ever be linked from something.
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

public partial class Pages_v5_Admin_Hacks_TemporaryHack : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        FinancialAccount bankAccount = FinancialAccount.FromIdentity(29);
        Organization euroPirates = Organization.FromIdentity(7);

        FinancialTransactions unbalancedTransactions = FinancialTransactions.GetUnbalanced(euroPirates);

        // assume all in bank account

        foreach (FinancialTransaction transaction in unbalancedTransactions)
        {
            if (transaction.Description[0] == 'H' && Char.IsDigit(transaction.Description[1]) && transaction.Rows.BalanceCentsDelta > 0)
            {
                // outbound intl invoice. Add an untracked intl invoice 10 days prior, map it to this

                OutboundInvoice newInvoice = OutboundInvoice.Create(euroPirates, Person.FromIdentity(1),
                    transaction.DateTime, euroPirates.FinancialAccounts.IncomeSales, "Untracked",
                    "untracked@example.com", string.Empty, euroPirates.Currency, false, string.Empty);
                newInvoice.AddItem("Untracked item", transaction.Rows.BalanceCentsDelta);

                // Doesn't close

                // Add transaction

                // Create the financial transaction with rows

                FinancialTransaction invoiceTransaction =
                    FinancialTransaction.Create(euroPirates.Identity, transaction.DateTime.AddDays(-10),
                    "Outbound Invoice #" + newInvoice.Identity);

                invoiceTransaction.AddRow(euroPirates.FinancialAccounts.AssetsOutboundInvoices, newInvoice.AmountCents, null);
                invoiceTransaction.AddRow(euroPirates.FinancialAccounts.IncomeSales, -newInvoice.AmountCents, null);
                invoiceTransaction.Dependency = newInvoice;

                transaction.AddRow(euroPirates.FinancialAccounts.AssetsOutboundInvoices, -newInvoice.AmountCents, null);

                Payment payment = Payment.CreateSingle(euroPirates, transaction.DateTime, euroPirates.Currency,
                                                               newInvoice.AmountCents, newInvoice, null);

                transaction.Dependency = payment.Group;
                payment.Group.Open = false;

            }

            if (transaction.Description == "Kostnad" && transaction.Rows.BalanceCentsDelta < 0 &&
                transaction.Rows.BalanceCentsDelta > -10000)
            {
                // Bank fee.

                transaction.AddRow(euroPirates.FinancialAccounts.CostsBankFees, -transaction.Rows.BalanceCentsDelta,
                    null);
            }

            if (transaction.Description.StartsWith("Paypal "))
            {
                // Bank fee.

                transaction.AddRow(euroPirates.FinancialAccounts.CostsBankFees, -transaction.Rows.BalanceCentsDelta,
                    null);
            }
        }

        Payouts.AutomatchAgainstUnbalancedTransactions(euroPirates);

    }
}