using System;
using Swarmops.Logic.Resources;

namespace Swarmops.Logic.Financial
{
    public class FinancialTransactionTagSetType
    {
        public static string GetLocalizedName (int identity)
        {
            if (identity < 0)
            {
                string resource = ((FinancialTransactionTagSetTypesStock) identity).ToString();

                return
                    Logic_Financial_FinancialTransactionTagSetTypesStock.ResourceManager.GetString (
                        resource);
            }

            throw new NotImplementedException ("Custom set names are not implemented yet");
        }
    }
}