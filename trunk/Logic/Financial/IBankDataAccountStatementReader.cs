using System.IO;
using Swarmops.Logic.Financial.BankDataReaders;

namespace Swarmops.Logic.Financial
{
    interface IBankDataAccountStatementReader
    {
        ImportedBankData ReadData(StreamReader data, FinancialAccount account, ExternalBankDataProfile profile);
    }
}
