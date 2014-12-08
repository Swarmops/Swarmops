using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types.Governance;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Field reading code

        private const string motionFieldSequence =
            " MotionId,MeetingId,Designation,SubmittedByPersonId,CreatedByPersonId," + // 0-4
            "CreatedDateTime,AmendedByPersonId,AmendedDateTime,Title,Text," + // 5-9
            "AmendedText,DecisionPoints,AmendedDecisionPoints,Open,Carried," + // 10-14
            "ThreadUrl,Amended,SequenceNumber " + // 15-17
            "FROM Motions ";

        private static BasicMotion ReadMotionFromDataReader (IDataRecord reader)
        {
            int motionId = reader.GetInt32 (0);
            int meetingId = reader.GetInt32 (1);
            string designation = reader.GetString (2);
            int submittedByPersonId = reader.GetInt32 (3);
            int createdByPersonId = reader.GetInt32 (4);
            DateTime createdDateTime = reader.GetDateTime (5);
            int amendedByPersonId = reader.GetInt32 (6);
            DateTime amendedDateTime = reader.GetDateTime (7);
            string title = reader.GetString (8);
            string text = reader.GetString (9);
            string amendedText = reader.GetString (10);
            string decisionPoints = reader.GetString (11);
            string amendedDecisionPoints = reader.GetString (12);
            bool open = reader.GetBoolean (13);
            bool passed = reader.GetBoolean (14);
            string threadUrl = reader.GetString (15);
            bool amended = reader.GetBoolean (16);
            int sequenceNumber = reader.GetInt32 (17);

            return new BasicMotion (motionId, meetingId, sequenceNumber, designation, submittedByPersonId,
                createdByPersonId, createdDateTime, amended, amendedByPersonId, amendedDateTime, title, text,
                amendedText, decisionPoints, amendedDecisionPoints, threadUrl, open, passed);
        }

        #endregion

        #region Record reading - SELECT statements

        public BasicMotion GetMotion (int motionId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + motionFieldSequence + "WHERE MotionId=" + motionId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadMotionFromDataReader (reader);
                    }

                    throw new ArgumentException ("Unknown Motion Id: " + motionId);
                }
            }
        }

        public BasicMotion[] GetMotions (params object[] conditions)
        {
            List<BasicMotion> result = new List<BasicMotion>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + motionFieldSequence + ConstructWhereClause ("Motions", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadMotionFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion

        #region Creation and manipulation - stored procedures

        public int CreateMotion (int meetingId, int submittingPersonId, int creatingPersonId, string title, string text,
            string decisionPoints)
        {
            DateTime now = DateTime.Now;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateMotion", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "meetingId", meetingId);
                AddParameterWithName (command, "submittingPersonId", submittingPersonId);
                AddParameterWithName (command, "creatingPersonId", creatingPersonId);
                AddParameterWithName (command, "createdDateTime", now);
                AddParameterWithName (command, "title", title);
                AddParameterWithName (command, "text", text);
                AddParameterWithName (command, "decisionPoints", decisionPoints);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        /*

        public void SetSalaryNetPaid(int salaryId, bool netPaid)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetSalaryNetPaid", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "salaryId", salaryId);
                AddParameterWithName(command, "netPaid", netPaid);

                command.ExecuteNonQuery();

                // "Open" is set in the stored procedure to "NOT (TaxPaid AND NetPaid)".
                // So if both are paid, Open is set to false.
            }
        }


        public void SetSalaryNetSalary(int salaryId, double netSalary)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetSalaryNetSalary", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "salaryId", salaryId);
                AddParameterWithName(command, "netSalary", netSalary);

                command.ExecuteNonQuery();

                // "Open" is set in the stored procedure to "NOT (TaxPaid AND NetPaid)".
                // So if both are paid, Open is set to false.
            }
        }


        public void SetSalaryTaxPaid(int salaryId, bool taxPaid)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetSalaryTaxPaid", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "salaryId", salaryId);
                AddParameterWithName(command, "taxPaid", taxPaid);

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
        */

        #endregion
    }
}