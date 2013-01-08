using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;

namespace Swarmops.Utility.Financial
{
    public class PaymentGroupMapper
    {
        public static void Run()
        {
            Organizations economyEnabledOrgs = Organizations.EconomyEnabled;

            foreach (Organization organization in economyEnabledOrgs)
            {
                FinancialTransactions unbalancedTransactions = FinancialTransactions.GetUnbalanced(organization);
                PaymentGroups groups = PaymentGroups.ForOrganization(organization);
                FinancialAccount assetsOutboundInvoices = organization.FinancialAccounts.AssetsOutboundInvoices;

                // This is an N^2 search. Don't care. It runs background.

                foreach (PaymentGroup group in groups)
                {
                    foreach (FinancialTransaction tx in unbalancedTransactions)
                    {
                        if (group.Open && tx.Description.EndsWith(group.Tag.Substring(4)) && tx.DateTime.Date == group.DateTime.Date && tx.Rows.AmountCentsTotal == group.AmountCents)
                        {
                            // Match!

                            tx.Dependency = group;
                            tx.AddRow(assetsOutboundInvoices, -group.AmountCents, null);
                            group.Open = false;
                        }
                    }
                }
            }
        }
    }
}
