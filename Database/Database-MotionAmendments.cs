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

        private const string motionAmendmentFieldSequence =
            " MotionAmendmentId,MotionAmendments.MotionId,MotionAmendments.SequenceNumber,MotionAmendments.SubmittedByPersonId,MotionAmendments.CreatedByPersonId," +
            // 0-4
            "MotionAmendments.CreatedDateTime,MotionAmendments.Title,MotionAmendments.Text,MotionAmendments.DecisionPoint,MotionAmendments.Open," +
            // 5-9
            "MotionAmendments.Carried " + // 10
            "FROM MotionAmendments ";

        private static BasicMotionAmendment ReadMotionAmendmentFromDataReader(IDataRecord reader)
        {
            int motionAmendmentId = reader.GetInt32(0);
            int motionId = reader.GetInt32(1);
            int sequenceNumber = reader.GetInt32(2);
            int submittedByPersonId = reader.GetInt32(3);
            int createdByPersonId = reader.GetInt32(4);
            DateTime createdDateTime = reader.GetDateTime(5);
            string title = reader.GetString(6);
            string text = reader.GetString(7);
            string decisionPoint = reader.GetString(8);
            bool open = reader.GetBoolean(9);
            bool carried = reader.GetBoolean(10);

            return new BasicMotionAmendment(motionAmendmentId, motionId, sequenceNumber, submittedByPersonId,
                createdByPersonId, createdDateTime, title, text, decisionPoint, open, carried);
        }

        #endregion

        #region Record reading - SELECT statements

        public BasicMotionAmendment GetMotionAmendment(int motionAmendmentId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + motionAmendmentFieldSequence + "WHERE MotionAmendmentId=" + motionAmendmentId + ";",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadMotionAmendmentFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown MotionAmendment Id: " + motionAmendmentId);
                }
            }
        }

        public BasicMotionAmendment[] GetMotionAmendments(params object[] conditions)
        {
            List<BasicMotionAmendment> result = new List<BasicMotionAmendment>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + motionAmendmentFieldSequence + ConstructWhereClause("MotionAmendments", conditions),
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadMotionAmendmentFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicMotionAmendment[] GetMotionAmendmentsForMeeting(int meetingId)
        {
            List<BasicMotionAmendment> result = new List<BasicMotionAmendment>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + motionAmendmentFieldSequence +
                        " JOIN Motions ON (Motions.MotionId=MotionAmendments.MotionId) WHERE Motions.MeetingId=" +
                        meetingId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadMotionAmendmentFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion

        #region Creation and manipulation - stored procedures

        public int CreateMotionAmendment(int motionId, int submittingPersonId, int createdByPersonId, string title,
            string text, string decisionPoint)
        {
            DateTime now = DateTime.Now;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateMotionAmendment", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "motionId", motionId);
                AddParameterWithName(command, "title", title);
                AddParameterWithName(command, "text", text);
                AddParameterWithName(command, "decisionPoint", decisionPoint);
                AddParameterWithName(command, "submittingPersonId", submittingPersonId);
                AddParameterWithName(command, "createdByPersonId", createdByPersonId);
                AddParameterWithName(command, "createdDateTime", now);

                return Convert.ToInt32(command.ExecuteScalar());
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