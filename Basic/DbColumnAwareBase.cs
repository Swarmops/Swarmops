using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

/* 2018-Dec-22: This is a started refactoring of the Database namespace into separate derived classes
 *              from a generic that contains most of the gruntwork. It is not nearly complete and
 *              the code is still experimental, not taken into production yet.
 */

namespace Swarmops.Basic
{
    public class DbColumnAwareBase
    {
        static DbColumnAwareBase()
        {
            if (_classColumnOrdinalLookup == null)
            {
                _classColumnOrdinalLookup = new Dictionary<string, Dictionary<string, int>>();
                _classColumnSequenceLookup = new Dictionary<string, string>();
            }
        }

        public DbColumnAwareBase()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            VerifyLookupInitialized();
        }

        private static Dictionary<string, Dictionary<string, int>> _classColumnOrdinalLookup;
        private static Dictionary<string, string> _classColumnSequenceLookup;


        public virtual void VerifyLookupInitialized()
        {
            string className = this.GetType().Name;
            if (_classColumnOrdinalLookup.ContainsKey(className))
            {
                return;
            }

            // The key of the instantiated class is not in the dictionary, so initialize dictionary

            // This is done with reflection, so is very expensive, which is why we only do it once for
            // every class that derives from DbColumnAwareBase
            ;
        }

        /*
         * 
         * sample SELECT sequence:
         *         private const string financialAccountDocumentFieldSequence =
            " FinancialAccountDocumentId,FinancialAccountId,FinancialAccountDocumentTypes.Name,UploadedDateTime,UploadedByPersonId, " + // 0-4
            " ConcernsPeriodStart,ConcernsPeriodEnd,RawDocumentText " + // 5-7
            " FROM FinancialAccountDocuments JOIN FinancialAccountDocumentTypes USING (FinancialAccountDocumentTypeId) ";

        */
    }
}
