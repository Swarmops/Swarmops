using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Transmission;
using Swarmops.Logic.Support.LogEntries;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class InboundInvoice: BasicInboundInvoice, IAttestable
    {
        public static InboundInvoice Create (Organization organization, DateTime dueDate, Int64 amountCents,
            FinancialAccount budget, string supplier, string description, string payToAccount, string ocr, 
            string invoiceReference, Person creatingPerson)
        {
            InboundInvoice newInvoice = FromIdentity(SwarmDb.GetDatabaseForWriting().
                CreateInboundInvoice(organization.Identity, dueDate, budget.Identity,
                    supplier, payToAccount, ocr,
                    invoiceReference, amountCents, creatingPerson.Identity));

            newInvoice.Description = description;  // Not in original schema; not cause for schema update

            // Create a corresponding financial transaction with rows

            FinancialTransaction transaction =
                FinancialTransaction.Create(organization.Identity, DateTime.Now,
                "Invoice #" + newInvoice.Identity + " from " + supplier);

            transaction.AddRow(organization.FinancialAccounts.DebtsInboundInvoices, -amountCents, creatingPerson);
            transaction.AddRow(budget, amountCents, creatingPerson);

            // Make the transaction dependent on the inbound invoice

            transaction.Dependency = newInvoice;

            // Create notification (slightly misplaced logic, but this is failsafest place)

            OutboundComm.CreateNotificationAttestationNeeded(budget, creatingPerson, supplier, (double)amountCents / 100.0, description, NotificationResource.InboundInvoice_Created); // Slightly misplaced logic, but failsafer here
            SwarmopsLogEntry.Create(creatingPerson,
                                    new InboundInvoiceCreatedLogEntry(creatingPerson, supplier, description, (double)amountCents / 100.0, budget), newInvoice);

            return newInvoice;
        }

        public static InboundInvoice FromBasic (BasicInboundInvoice basic)
        {
            return new InboundInvoice(basic);
        }

        private InboundInvoice(BasicInboundInvoice basicInstance):
            base (basicInstance)
        {
            // constructor from basic type
        }

        public static InboundInvoice FromIdentity (int inboundInvoiceId)
        {
            return new InboundInvoice(SwarmDb.GetDatabaseForReading().GetInboundInvoice(inboundInvoiceId));
        }

        public FinancialAccount Budget
        {
            get
            {
                if (base.BudgetId == 0)
                {
                    return null;
                }

                return FinancialAccount.FromIdentity(base.BudgetId);
            }
        }

        public Documents Documents
        {
            get
            {
                return Support.Documents.ForObject(this);
            }
        }

        public new bool Open
        {
            get { return base.Open; }
            set
            {
                if (base.Open != value)
                {
                    SwarmDb.GetDatabaseForWriting().SetInboundInvoiceOpen(this.Identity, value);
                    base.Open = value;
                }
            }
        }

        public Organization Organization
        {
            get { return Organization.FromIdentity(this.OrganizationId); }
        }


        public decimal Amount
        {
            get { return base.AmountCents / 100.0m; }
        }


        public new DateTime DueDate
        {
            get
            {
                return base.DueDate;
            }
            set
            {
                if (value != base.DueDate)
                {
                    SwarmDb.GetDatabaseForWriting().SetInboundInvoiceDueDate(this.Identity, value);
                    base.DueDate = value;
                }
            }
        }

        public FinancialTransaction FinancialTransaction
        {
            get
            {
                FinancialTransactions transactions = FinancialTransactions.ForDependentObject(this);

                if (transactions.Count == 0)
                {
                    return null; // not possible from July 26 onwards, but some grandfather work
                }

                else if (transactions.Count == 1)
                {
                    return transactions[0];
                }

                throw new InvalidOperationException("It appears inbound invoice #" + this.Identity +
                   " has multiple dependent financial transactions. This is an invalid state.");
            }
        }


        public void SetBudget (FinancialAccount budget, Person settingPerson)
        {
            base.BudgetId = budget.Identity;
            SwarmDb.GetDatabaseForWriting().SetInboundInvoiceBudget(this.Identity, budget.Identity);
            UpdateTransaction(settingPerson);
        }


        public void SetAmountCents (Int64 amountCents, Person settingPerson)
        {
            base.AmountCents = amountCents;
            SwarmDb.GetDatabaseForWriting().SetInboundInvoiceAmount(this.Identity, amountCents);
            UpdateTransaction(settingPerson);
        }


        private void UpdateTransaction(Person updatingPerson)
        {
            Dictionary<int, Int64> nominalTransaction = new Dictionary<int, Int64>();

            // Create an image of what the transaction SHOULD look like with changes.

            if (this.Attested || this.Open)
            {
                // ...only holds values if not closed as invalid...

                nominalTransaction[this.Organization.FinancialAccounts.DebtsInboundInvoices.Identity] = -AmountCents;
                nominalTransaction[this.BudgetId] = AmountCents;
            }

            FinancialTransaction.RecalculateTransaction(nominalTransaction, updatingPerson);
        }


        public FinancialValidations Validations
        {
            get { return FinancialValidations.ForObject(this); }
        }


        public string Description
        {
            get
            {
                return
                    ObjectOptionalData.ForObject(this).GetOptionalDataString(
                        ObjectOptionalDataType.InboundInvoiceDescription);
            }

            set
            {
                ObjectOptionalData.ForObject(this).SetOptionalDataString(ObjectOptionalDataType.InboundInvoiceDescription, value);
            }
        }


        #region IAttestable Members

        public void Attest(Person attester)
        {
            SwarmDb.GetDatabaseForWriting().SetInboundInvoiceAttested(this.Identity, true);
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Attestation,
                                                             FinancialDependencyType.InboundInvoice, this.Identity,
                                                             DateTime.Now, attester.Identity, (double) this.Amount);
            base.Attested = true;
        }

        public void Deattest(Person deattester)
        {
            SwarmDb.GetDatabaseForWriting().SetInboundInvoiceAttested(this.Identity, false);
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Deattestation,
                                                             FinancialDependencyType.InboundInvoice, this.Identity,
                                                             DateTime.Now, deattester.Identity, (double) this.Amount);
            base.Attested = false;
        }

        #endregion
    }
}
