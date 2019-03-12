using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Interfaces;


/* 2018-Dec-22: This is a started refactoring of the Database namespace into separate derived classes
 *              from a generic that contains most of the gruntwork. It is not nearly complete and
 *              the code is still experimental, not taken into production yet.
 */


namespace Swarmops.Database
{
    public class TableBase<TBasic> where TBasic: IDatabaseColumnAware<TBasic>, new()
    {
        public TBasic GetRecordByIdentity(int identity, bool aggressive = false)
        {
            throw new NotImplementedException();
            /*
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT" + paymentFieldSequence +
                                  "WHERE PaymentId=" + paymentId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new TBasic().FromDataReader(reader);
                    }

                    throw new ArgumentException("No such PaymentId:" + paymentId);
                }
            }*/
        }

        public TBasic[] GetRecordsByIdentity(int[] identities)
        {
            throw new NotImplementedException();
        }

        public TBasic[] GetRecords(params object[] conditions)
        {
            throw new NotImplementedException();
        }
    }
}
