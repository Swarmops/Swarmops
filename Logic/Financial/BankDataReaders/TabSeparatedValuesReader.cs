using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Swarmops.Logic.Financial.BankDataReaders
{
    public class TabSeparatedValuesReader: IBankDataAccountStatementReader
    {
        public ImportedBankData ReadData(StreamReader data, FinancialAccount account, ExternalBankDataProfile profile)
        {
            throw new NotImplementedException();
        }
    }
}
