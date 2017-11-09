using System;
using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class OutboundInvoices : PluralBase<OutboundInvoices, OutboundInvoice, BasicOutboundInvoice>
    {
        public static OutboundInvoices ForOrganization (Organization organization)
        {
            return ForOrganization (organization, false);
        }

        public static OutboundInvoices ForOrganization (Organization organization, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray (SwarmDb.GetDatabaseForReading().GetOutboundInvoices (organization));
            }
            return
                FromArray (SwarmDb.GetDatabaseForReading()
                    .GetOutboundInvoices (organization, DatabaseCondition.OpenTrue));
        }


        public static void AutomatchAgainstUnbalancedTransactions(Organization organization, Person person)
        {
            // Matches unbalanced financial transactions against unclosed outbound invoices

            // Should this be in bot?

            OutboundInvoices invoices = ForOrganization(organization); // gets all open

            // build a hash of all invoice reference numbers, ours and theirs (and let's hope for no collision...)

            // TODO: Collision detection

            Dictionary<string, OutboundInvoice> invoiceLookup = new Dictionary<string, OutboundInvoice>();
            foreach (OutboundInvoice invoice in invoices)
            {
                invoiceLookup[RemoveNoise(invoice.TheirReference)] = invoice;
                invoiceLookup[RemoveNoise(invoice.Reference)] = invoice;
            }

            FinancialTransactions transactions = FinancialTransactions.GetUnbalanced(organization);

            foreach (FinancialTransaction transaction in transactions)
            {
                string[] words = transaction.Description.Split(' ');
                bool collision = false;
                int invoiceId = 0;
                OutboundInvoice identifiedInvoice = null;

                foreach (string word in words)
                {
                    string cleanWord = RemoveNoise(word);
                    if (invoiceLookup.ContainsKey(cleanWord))
                    {
                        OutboundInvoice invoice = invoiceLookup[cleanWord];
                        if (transaction.Rows.AmountCentsTotal == invoice.AmountCents)
                        {
                            // Matching description and amount

                            if (invoiceId != 0 && invoiceId != invoice.Identity)
                            {
                                collision = true;
                            }
                            else if (invoice.Open) // double check it wasn't closed previously in loop
                            {
                                invoiceId = invoice.Identity;
                                identifiedInvoice = invoice;
                            }
                        }
                    }
                }

                // If invoiceId != 0 and collision == false, we've found exactly one match

                if (invoiceId != 0 && !collision)
                {
                    Payment.CreateSingle(organization, transaction.DateTime, identifiedInvoice.Currency,
                        identifiedInvoice.AmountCents, identifiedInvoice, person);

                    // Balance transaction against outbound invoices

                    transaction.AddRow(organization.FinancialAccounts.AssetsOutboundInvoices,
                        -identifiedInvoice.AmountCents, person);
                }
            }
        }

        private static string RemoveNoise(string input)
        {
            return input.Replace("-", "").Replace(".", "").Replace("/", "");
        }
    }
}