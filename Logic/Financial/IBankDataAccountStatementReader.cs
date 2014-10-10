using System.IO;

namespace Swarmops.Logic.Financial
{
    interface IBankDataAccountStatementReader
    {
        ImportedBankData ReadData(StreamReader data, FinancialAccount account, ExternalBankDataProfile profile);
    }
}
