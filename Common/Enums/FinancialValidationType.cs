namespace Swarmops.Common.Enums
{
    public enum FinancialValidationType
    {
        /// <summary>
        ///     Unknown type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     An approval of an expenditure from a budget.
        /// </summary>
        Approval,

        /// <summary>
        ///     Removal of approved status.
        /// </summary>
        UndoApproval,

        /// <summary>
        ///     Validation of expenditure documents.
        /// </summary>
        Validation,

        /// <summary>
        ///     Removal of validated status.
        /// </summary>
        UndoValidation,

        /// <summary>
        ///     Kill: close this financial doc as not valid.
        /// </summary>
        Kill
    }
}