using System.IO;

namespace Swarmops.Logic.Financial
{
    internal interface IBankDataAccountStatementReader
    {
        ImportedBankData ReadData (StreamReader data, FinancialAccount account, ExternalBankDataProfile profile);
    }
}