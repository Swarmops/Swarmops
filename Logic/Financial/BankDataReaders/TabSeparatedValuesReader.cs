using System;
using System.IO;

namespace Swarmops.Logic.Financial.BankDataReaders
{
    public class TabSeparatedValuesReader : IBankDataAccountStatementReader
    {
        public ImportedBankData ReadData (StreamReader data, FinancialAccount account, ExternalBankDataProfile profile)
        {
            throw new NotImplementedException();
        }
    }
}