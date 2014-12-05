using System;

namespace Swarmops.Logic.Structure
{
    public class GeographyBar : IComparable
    {
        public object DrillDownData;
        public Geography Geography;
        public double Value;

        #region IComparable Members

        public int CompareTo (object obj)
        {
            GeographyBar otherBar = (GeographyBar) obj;

            return this.Value.CompareTo (otherBar.Value);
        }

        #endregion
    }
}