using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types.Financial;
using Swarmops.Basic.Types.Swarm;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Field reading code

        //                 original.Identity, original.PersonId, original.OrganizationId, original.CreatedDateTime, original.Open,
        // original.GrantedDateTime, original.GrantedByPersonId, original.Score1, original.Score2, original.Score3)


        private const string applicantFieldSequence =
            " ApplicantId, PersonId, OrganizationId, CreatedDateTime, Open, " + // 0-4
            "GrantedDateTime, GrantedByPersonId, Score1, Score2, Score3, " + // 5-9
            "(Score1+Score2+Score3) AS ScoreTotal " +  // included for sortability only
            "FROM Applicants ";

        private static BasicApplicant ReadApplicantFromDataReader (IDataRecord reader)
        {
            int applicantId = reader.GetInt32 (0);
            int personId = reader.GetInt32 (1);
            int organizationId = reader.GetInt32(2);
            DateTime createdDateTime = reader.GetDateTime (3);
            bool open = reader.GetBoolean(4);

            DateTime grantedDateTime = reader.GetDateTime(5);
            int grantedByPersonId = reader.GetInt32(6);
            int score1 = reader.GetInt32(7);
            int score2 = reader.GetInt32(8);
            int score3 = reader.GetInt32(9);

            return new BasicApplicant(applicantId, personId, organizationId, createdDateTime, open, grantedDateTime, grantedByPersonId, score1, score2, score3);
        }

        #endregion

        #region Record reading - SELECT statements

        /// <summary>
        ///     Gets a salary from the database.
        /// </summary>
        /// <param name="salaryId">The salary database identity.</param>
        /// <returns>The requested salary.</returns>
        /// <exception cref="ArgumentException">Thrown if there is no such identity.</exception>
        public BasicApplicant GetApplicant (int applicantId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + applicantFieldSequence + "WHERE ApplicantId=" + applicantId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadApplicantFromDataReader (reader);
                    }

                    throw new ArgumentException ("Unknown Applicant Id: " + applicantId);
                }
            }
        }

        /// <summary>
        ///     Gets salaries from the database.
        /// </summary>
        /// <param name="conditions">
        ///     An optional combination of a Person and/or Organization object and/or DatabaseCondition
        ///     specifiers.
        /// </param>
        /// <returns>A list of matching salaries.</returns>
        public BasicApplicant[] GetApplicants (params object[] conditions)
        {
            List<BasicApplicant> result = new List<BasicApplicant>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + applicantFieldSequence + " " +
                        ConstructWhereClause ("Applicants", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadApplicantFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion

        #region Creation and manipulation - stored procedures

        public int CreateApplicant (int personId, int organizationId)
        {
            DateTime now = DateTime.UtcNow;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateApplicant", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "personId", personId);
                AddParameterWithName (command, "organizationId", organizationId);
                AddParameterWithName (command, "dateTimeNow", now);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        /*
        public void SetSalaryNetPaid (int salaryId, bool netPaid)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetSalaryNetPaid", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "salaryId", salaryId);
                AddParameterWithName (command, "netPaid", netPaid);

                command.ExecuteNonQuery();

                // "Open" is set in the stored procedure to "NOT (TaxPaid AND NetPaid)".
                // So if both are paid, Open is set to false.
            }
        }


        public void SetSalaryNetSalary (int salaryId, Int64 netSalaryCents)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetSalaryNetSalaryPrecise", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "salaryId", salaryId);
                AddParameterWithName (command, "netSalaryCents", netSalaryCents);

                command.ExecuteNonQuery();
            }
        }


        public void SetSalaryTaxPaid (int salaryId, bool taxPaid)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetSalaryTaxPaid", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "salaryId", salaryId);
                AddParameterWithName (command, "taxPaid", taxPaid);

                command.ExecuteNonQuery();

                // "Open" is set in the stored procedure to "NOT (TaxPaid AND NetPaid)".
                // So if both are paid, Open is set to false.
            }
        }


        public void SetSalaryAttested(int salaryId, bool attested)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetSalaryAttested", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "salaryId", salaryId);
                AddParameterWithName(command, "attested", attested);

                command.ExecuteNonQuery();
            }
        }

        public void SetSalaryOpen(int salaryId, bool attested)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetSalaryOpen", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "salaryId", salaryId);
                AddParameterWithName(command, "open", attested);

                command.ExecuteNonQuery();
            }
        }*/

        #endregion
    }
}