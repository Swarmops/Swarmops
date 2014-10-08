namespace Swarmops.Basic.Enums
{
    public enum OutboundCommPriority
    {
        Unknown = 0,
        /// <summary>
        /// Priority 1 of 7 (highest).
        /// </summary>
        Highest = 32,
        /// <summary>
        /// Priority 2 of 7 (second highest).
        /// </summary>
        High = 64,
        /// <summary>
        /// Priority 3 of 7 (just above normal priority).
        /// </summary>
        Elevated = 96,
        /// <summary>
        /// Priority 4 of 7 (normal and default).
        /// </summary>
        Normal = 128,
        /// <summary>
        /// Priority 5 of 7 (just below normal priority).
        /// </summary>
        Lowered = 160,
        /// <summary>
        /// Priority 6 of 7 (second lowest).
        /// </summary>
        Low = 192,
        /// <summary>
        /// Priority 7 of 7 (lowest).
        /// </summary>
        Lowest = 224
    }
}
