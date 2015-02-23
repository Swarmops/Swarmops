namespace Swarmops.Common.Enums
{
    public enum GeographyLevel
    {
        /// <summary>
        ///     Unknown level
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     Municipality or city level
        /// </summary>
        Municipality = 1,

        /// <summary>
        ///     Electoral circuit - usually 5-15 cities
        /// </summary>
        ElectoralCircuit = 2,

        /// <summary>
        ///     County - usually same as electoral circuit
        /// </summary>
        County = 3,

        /// <summary>
        ///     District - 4-7 districts per state
        /// </summary>
        District = 4,

        /// <summary>
        ///     State - not necessary for small countries
        /// </summary>
        State = 5
    }
}