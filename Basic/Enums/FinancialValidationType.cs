namespace Activizr.Basic.Enums
{
    public enum FinancialValidationType
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// An attestation of an expenditure from a budget.
        /// </summary>
        Attestation,
        /// <summary>
        /// Removal of attested status.
        /// </summary>
        Deattestation,
        /// <summary>
        /// Validation of expenditure documents.
        /// </summary>
        Validation,
        /// <summary>
        /// Devalidation - removal of validated status.
        /// </summary>
        Devalidation,
        /// <summary>
        /// Kill: close this financial doc as not valid.
        /// </summary>
        Kill
    }
}