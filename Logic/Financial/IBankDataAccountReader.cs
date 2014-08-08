using System.IO;
using Swarmops.Logic.Financial.BankDataReaders;

namespace Swarmops.Logic.Financial
{
    interface IBankDataAccountReader
    {
        ImportedBankData ReadData(TextReader data, IBankDataPaymentsReader paymentsReader);
    }
}
