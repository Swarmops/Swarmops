using System;
using System.Collections.Generic;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.DashboardTasks
{
    public class TaskGroup
    {
        private static readonly Dictionary<TaskGroupType, string> lookupDescriptionSingle;
        private static readonly Dictionary<TaskGroupType, string> lookupDescriptionMany;
        private static readonly Dictionary<TaskGroupType, string> lookupNavigateUrl;
        private static readonly Dictionary<TaskGroupType, string> lookupIconUrl;

        static TaskGroup()
        {
            lookupDescriptionSingle = new Dictionary<TaskGroupType, string>();
            lookupDescriptionMany = new Dictionary<TaskGroupType, string>();
            lookupNavigateUrl = new Dictionary<TaskGroupType, string>();
            lookupIconUrl = new Dictionary<TaskGroupType, string>();

            // TODO: Needs localization

            lookupNavigateUrl[TaskGroupType.AttestSalaries] = "/Pages/v4/Financial/AttestCosts.aspx";
            lookupIconUrl[TaskGroupType.AttestSalaries] = "/Images/Public/Fugue/icons-shadowless/money-coin.png";
            lookupDescriptionSingle[TaskGroupType.AttestSalaries] = "One salary awaiting attestation";
            lookupDescriptionMany[TaskGroupType.AttestSalaries] = "{0} salaries awaiting attestation";

            lookupNavigateUrl[TaskGroupType.AttestExpenseClaims] = "/Pages/v4/Financial/AttestCosts.aspx";
            lookupIconUrl[TaskGroupType.AttestExpenseClaims] = "/Images/Public/Fugue/icons-shadowless/money-coin.png";
            lookupDescriptionSingle[TaskGroupType.AttestExpenseClaims] = "One expense claim awaiting attestation";
            lookupDescriptionMany[TaskGroupType.AttestExpenseClaims] = "{0} expense claims awaiting attestation";

            lookupNavigateUrl[TaskGroupType.AttestInvoices] = "/Pages/v4/Financial/AttestCosts.aspx";
            lookupIconUrl[TaskGroupType.AttestInvoices] = "/Images/Public/Fugue/icons-shadowless/money-coin.png";
            lookupDescriptionSingle[TaskGroupType.AttestInvoices] = "One invoice awaiting attestation";
            lookupDescriptionMany[TaskGroupType.AttestInvoices] = "{0} invoices awaiting attestation";

            lookupNavigateUrl[TaskGroupType.Volunteers] = "/Pages/v4/Pirates/ListVolunteers.aspx";
            lookupIconUrl[TaskGroupType.Volunteers] = "/Images/Public/Silk/user_star.png";
            lookupDescriptionSingle[TaskGroupType.Volunteers] = "One waiting volunteer";
            lookupDescriptionMany[TaskGroupType.Volunteers] = "{0} waiting volunteers";

            lookupNavigateUrl[TaskGroupType.ValidateExpenseClaims] =
                "/Pages/v4/Financial/ValidateExpenseDocumentation.aspx";
            lookupIconUrl[TaskGroupType.ValidateExpenseClaims] =
                "/Images/Public/Fugue/icons-shadowless/receipt-invoice.png";
            lookupDescriptionSingle[TaskGroupType.ValidateExpenseClaims] = "One receipt to validate";
            lookupDescriptionMany[TaskGroupType.ValidateExpenseClaims] = "{0} receipts to validate";

            lookupNavigateUrl[TaskGroupType.DeclareAdvanceDebts] = "/Pages/v4/Financial/ClaimExpense.aspx";
            lookupIconUrl[TaskGroupType.DeclareAdvanceDebts] =
                "/Images/Public/Fugue/icons-shadowless/pwcustom/money-red.png";
            lookupDescriptionSingle[TaskGroupType.DeclareAdvanceDebts] =
                "Submit receipts for, or repay, your advance of SEK {0}";
            lookupDescriptionMany[TaskGroupType.DeclareAdvanceDebts] =
                "Submit receipts for, or repay, your advance of SEK {0}";

            lookupNavigateUrl[TaskGroupType.Payout] = "/Pages/v4/Financial/PreparePayouts.aspx";
            lookupIconUrl[TaskGroupType.Payout] = "/Images/Public/Silk/money.png";
            lookupDescriptionSingle[TaskGroupType.Payout] = "One payout waiting";
            lookupDescriptionMany[TaskGroupType.Payout] = "{0} payouts waiting";

            lookupNavigateUrl[TaskGroupType.PayoutUrgently] = "/Pages/v4/Financial/PreparePayouts.aspx";
            lookupIconUrl[TaskGroupType.PayoutUrgently] =
                "/Images/Public/Fugue/icons-shadowless/pwcustom/money-silk-red.png";
            lookupDescriptionSingle[TaskGroupType.PayoutUrgently] = "One URGENT payout waiting";
            lookupDescriptionMany[TaskGroupType.PayoutUrgently] = "{0} URGENT payouts waiting";

            lookupNavigateUrl[TaskGroupType.PayoutOverdue] = "/Pages/v4/Financial/PreparePayouts.aspx";
            lookupIconUrl[TaskGroupType.PayoutOverdue] =
                "/Images/Public/Fugue/icons-shadowless/pwcustom/money-silk-darkred.png";
            lookupDescriptionSingle[TaskGroupType.PayoutOverdue] = "One OVERDUE payout waiting";
            lookupDescriptionMany[TaskGroupType.PayoutOverdue] = "{0} OVERDUE payouts waiting";

            lookupNavigateUrl[TaskGroupType.AttestationWarning] = "/Pages/v4/Financial/ViewInboundInvoices.aspx";
            lookupIconUrl[TaskGroupType.AttestationWarning] =
                "/Images/Public/Fugue/icons-shadowless/receipt-invoice.png";
            lookupDescriptionSingle[TaskGroupType.AttestationWarning] = "Approval for invoice {0} is running late";
            lookupDescriptionMany[TaskGroupType.AttestationWarning] = "Attestations for invoices {0} are running late";
        }

        public TaskGroup (TaskGroupType type)
        {
            Type = type;
            Tasks = new List<TaskBase>();
        }

        public TaskGroupType Type { get; protected set; }
        public List<TaskBase> Tasks { get; private set; }

        public string NavigateUrl
        {
            get { return lookupNavigateUrl[Type]; }
        }

        public string IconUrl
        {
            get { return lookupIconUrl[Type]; }
        }

        public string Description
        {
            get
            {
                if (Type == TaskGroupType.DeclareAdvanceDebts)
                {
                    return String.Format (lookupDescriptionSingle[TaskGroupType.DeclareAdvanceDebts],
                        Tasks[0].Description);
                }
                if (Type == TaskGroupType.AttestationWarning)
                {
                    List<int> identities = new List<int>();

                    foreach (TaskBase item in Tasks)
                    {
                        identities.Add (item.Identity);
                    }

                    identities.Sort();

                    string baseString = identities.Count > 1
                        ? lookupDescriptionMany[TaskGroupType.AttestationWarning]
                        : lookupDescriptionSingle[TaskGroupType.AttestationWarning];

                    return String.Format (baseString, Formatting.GenerateRangeString (identities));
                }
                if (Tasks.Count > 1)
                {
                    return String.Format (lookupDescriptionMany[Type], Tasks.Count);
                }
                return lookupDescriptionSingle[Type];
            }
        }
    }


    public enum TaskGroupType
    {
        Unknown = 0,

        /// <summary>
        ///     Salaries to be attested
        /// </summary>
        AttestSalaries,

        /// <summary>
        ///     Invoices to be attested
        /// </summary>
        AttestInvoices,

        /// <summary>
        ///     Expense claims to be attested
        /// </summary>
        AttestExpenseClaims,

        /// <summary>
        ///     Volunteers that need to be taken care of
        /// </summary>
        Volunteers,

        /// <summary>
        ///     Expense claim receipts to be validated
        /// </summary>
        ValidateExpenseClaims,

        /// <summary>
        ///     Cash advances that need to be repaid or declared
        /// </summary>
        DeclareAdvanceDebts,

        /// <summary>
        ///     Payout waiting
        /// </summary>
        Payout,

        /// <summary>
        ///     Payout waiting and running late
        /// </summary>
        PayoutUrgently,

        /// <summary>
        ///     Payout is overdue
        /// </summary>
        PayoutOverdue,

        /// <summary>
        ///     Warning: invoice needs attestation urgently
        /// </summary>
        AttestationWarning
    }
}