using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class VatReportItems: PluralBase<VatReportItems, VatReportItem, BasicVatReportItem>
    {
        #region Creation and Construction

        public static VatReportItems ForReport(VatReport report)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetVatReportItems(report));
        }

        #endregion
    }
}
