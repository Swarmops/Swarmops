namespace Swarmops.Common.Enums
{
    public enum ReportRequirement
    {
        Unknown = 0,
        /// <summary>
        /// a report (of the discussed type) is required for this time period
        /// </summary>
        Required,
        /// <summary>
        /// a report (of the discussed type) is NOT required for this time period
        /// </summary>
        NotRequired,
        /// <summary>
        /// A report (of the discussed type) has already been completed for this time period
        /// </summary>
        Completed
    }
}
