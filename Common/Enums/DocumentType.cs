namespace Swarmops.Common.Enums
{
    public enum DocumentType
    {
        Unknown = 0,

        /// <summary>
        ///     A photo that can be used to generate on-the-fly stuff, or for press images.
        /// </summary>
        PersonPhoto,

        /// <summary>
        ///     A receipt for an expense. Filed by the expenser.
        /// </summary>
        ExpenseClaim,

        /// <summary>
        ///     A document supporting a transaction where assets decrease (invoice, etc).
        /// </summary>
        FinancialTransaction,

        /// <summary>
        ///     A temporary identity, used while the basis of the document is still being created.
        /// </summary>
        Temporary,

        /// <summary>
        ///     An inbound invoice, going into accounts payable.
        /// </summary>
        InboundInvoice,

        /// <summary>
        ///     Something physical received in the mail
        /// </summary>
        PaperLetter,

        /// <summary>
        ///     A document (contract or else) that creates or modifies the payroll
        /// </summary>
        PayrollItem,

        /// <summary>
        ///     Photo documenting an external activity
        /// </summary>
        ExternalActivityPhoto
    }
}