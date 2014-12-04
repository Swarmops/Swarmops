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

        private const string meetingFieldSequence =
            " MeetingId,OrganizationId,Name,MotionSubmissionEnds,AmendmentSubmissionEnds," + // 0-4
            "AmendmentVotingStarts,AmendmentVotingEnds,MotionVotingStarts,MotionVotingEnds " + // 5-8
            "FROM Meetings ";

        private static BasicMeeting ReadMeetingFromDataReader(IDataRecord reader)
        {
            int meetingId = reader.GetInt32(0);
            int organizationId = reader.GetInt32(1);
            string name = reader.GetString(2);
            DateTime motionSubmissionEnds = reader.GetDateTime(3);
            DateTime amendmentSubmissionEnds = reader.GetDateTime(4);
            DateTime amendmentVotingStarts = reader.GetDateTime(5);
            DateTime amendmentVotingEnds = reader.GetDateTime(6);
            DateTime motionVotingStarts = reader.GetDateTime(7);
            DateTime motionVotingEnds = reader.GetDateTime(8);

            return new BasicMeeting(meetingId, organizationId, name, motionSubmissionEnds, amendmentSubmissionEnds,
                amendmentVotingStarts, amendmentVotingEnds, motionVotingStarts, motionVotingEnds);
        }

        #endregion

        #region Record reading - SELECT statements

        public BasicMeeting GetMeeting(int meetingId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + meetingFieldSequence + "WHERE MeetingId=" + meetingId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadMeetingFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown Meeting Id: " + meetingId);
                }
            }
        }

        public BasicMeeting[] GetMeetings(params object[] conditions)
        {
            List<BasicMeeting> result = new List<BasicMeeting>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + motionFieldSequence + ConstructWhereClause("Meetings", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadMeetingFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion

        #region Creation and manipulation - stored procedures

        /*
        public int CreateMotion(int meetingId, int submittingPersonId, int creatingPersonId, string title, string text, string decisionPoints)
        {
            DateTime now = DateTime.Now;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateMotion", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "meetingId", meetingId);
                AddParameterWithName(command, "submittingPersonId", submittingPersonId);
                AddParameterWithName(command, "creatingPersonId", creatingPersonId);
                AddParameterWithName(command, "createdDateTime", now);
                AddParameterWithName(command, "title", title);
                AddParameterWithName(command, "text", text);
                AddParameterWithName(command, "decisionPoints", decisionPoints);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }*/
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