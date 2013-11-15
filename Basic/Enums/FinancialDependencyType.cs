namespace Swarmops.Basic.Enums
{
    public enum FinancialDependencyType
    {
        /// <summary>
        /// Unknown dependency
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// An expense claim, filed by an activist or officer
        /// </summary>
        ExpenseClaim,
        /// <summary>
        /// An invoice, received in the mail
        /// </summary>
        InboundInvoice,
        /// <summary>
        /// Money leaving the organization, documented after processes
        /// </summary>
        Payout,
        /// <summary>
        /// Salary paid to somebody employed or otherwise monthly compensated
        /// </summary>
        Salary,
        /// <summary>
        /// An outbound invoice, requesting money from another organization
        /// </summary>
        OutboundInvoice,
        /// <summary>
        /// A group of inbound payments
        /// </summary>
        PaymentGroup,
        /// <summary>
        /// A dependency on (another) financial transaction - used for chaining across fiscal years
        /// </summary>
        FinancialTransaction,
        /// <summary>
        /// A conference or similar event (called 'parley' in lingo)
        /// </summary>
        Parley,
        /// <summary>
        /// A cash advance (prior payment, rather than doing an expense claim post purchase)
        /// </summary>
        CashAdvance,
        /// <summary>
        /// A payback for a previous cash advance
        /// </summary>
        CashAdvancePayback
    }
}