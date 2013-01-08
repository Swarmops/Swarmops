using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class PirateDb
    {
        #region Field reading code

        private const string pwEventFieldSequence =
            " PWEvents.EventId, PWEventTypes.Name AS EventType, PWEventSources.Name AS EventSource, PWEvents.ActingPersonId, PWEvents.AffectedPersonId, " + // 0-4
            " PWEvents.CreatedDateTime, PWEvents.ProcessedDateTime, PWEvents.Open, PWEvents.OrganizationId, PWEvents.GeographyId, " + // 5-9
            " PWEvents.ParameterInt, PWEvents.ParameterText " + // 10-11
            " FROM PWEvents " +
            " JOIN PWEventTypes ON (PWEvents.EventTypeId=PWEventTypes.EventTypeId) " +
            " JOIN PWEventSources ON (PWEvents.EventSourceId=PWEventSources.EventSourceId) ";


        private BasicPWEvent ReadPWEventFromDataReader (DbDataReader reader)
        {
            int eventId = reader.GetInt32(0);
            EventType eventType = (EventType)Enum.Parse(typeof(EventType), reader.GetString(1));
            EventSource eventSource = (EventSource)Enum.Parse(typeof(EventSource), reader.GetString(2));
            int actingPersonId = reader.GetInt32(3);
            int affectedPersonId = reader.GetInt32(4);
            DateTime eventDateTime = reader.GetDateTime(5);
            DateTime processedDateTime = reader.GetDateTime(6);
            bool open = reader.GetBoolean(7);
            int organizationId = reader.GetInt32(8);
            int geographyId = reader.GetInt32(9);
            int parameterInt = reader.GetInt32(10);
            string parameterText = reader.GetString(11);

            return new BasicPWEvent(eventId, eventDateTime, open, processedDateTime, eventType, eventSource, actingPersonId,
                affectedPersonId, organizationId, geographyId, parameterInt, parameterText);
        }

        #endregion


        public BasicPWEvent[] GetTopUnprocessedEvents ()
        {
            List<BasicPWEvent> result = new List<BasicPWEvent>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + pwEventFieldSequence + " WHERE Open=1 AND ProcessDateTime < NOW() LIMIT 10", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPWEventFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public BasicPWEvent[] GetEventsForPerson (int personId)
        {
            List<BasicPWEvent> result = new List<BasicPWEvent>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + pwEventFieldSequence + " WHERE AffectedPersonId=" + personId + " ORDER BY CreatedDateTime",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPWEventFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }
        public Dictionary<int, List<BasicPWEvent>> GetEventsForPersons (int[] personId, EventType[] eventTypes)
        {
            Dictionary<int, List<BasicPWEvent>> result = new Dictionary<int, List<BasicPWEvent>>();
            List<string> eventTypesAsString = new List<string>();
            foreach (EventType et in eventTypes)
                eventTypesAsString.Add(et.ToString());
                
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + pwEventFieldSequence
                        + " WHERE AffectedPersonId IN (" + JoinIds(personId) + ") "
                        + (eventTypes.Length > 0 ? " AND PWEventTypes.Name IN (" + JoinStrings(eventTypesAsString.ToArray()) + ") " : "")
                        + " ORDER BY CreatedDateTime desc",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BasicPWEvent evt = ReadPWEventFromDataReader(reader);
                        if (!result.ContainsKey(evt.AffectedPersonId))
                            result[evt.AffectedPersonId] = new List<BasicPWEvent>();
                        result[evt.AffectedPersonId].Add(evt);
                    }

                    return result;
                }
            }
        }

        public BasicPWEvent[] GetEventsOfType (EventType eventType)
        {
            List<BasicPWEvent> result = new List<BasicPWEvent>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + pwEventFieldSequence + " WHERE PWEventTypes.Name='" + eventType.ToString() +
                        "' ORDER BY CreatedDateTime", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPWEventFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }



        public int CreateEvent (EventSource eventSource, EventType eventType, int actingPersonId,
                                int organizationId,
                                int geographyId, int affectedPersonId, int parameterInt, string parameterText)
        {
            return CreateEvent(eventSource, eventType, actingPersonId, organizationId, geographyId, affectedPersonId,
                               parameterInt, parameterText, DateTime.Now);
        }

        public int CreateEvent (EventSource eventSource, EventType eventType, int actingPersonId,
                                int organizationId,
                                int geographyId, int affectedPersonId, int parameterInt, string parameterText,
                                DateTime processTime)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreatePWEvent", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "eventSource", eventSource.ToString());
                AddParameterWithName(command, "eventType", eventType.ToString());
                AddParameterWithName(command, "actingPersonId", actingPersonId);
                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "geographyId", geographyId);
                AddParameterWithName(command, "affectedPersonId", affectedPersonId);
                AddParameterWithName(command, "parameterInt", parameterInt);
                AddParameterWithName(command, "parameterText", parameterText);
                AddParameterWithName(command, "processDateTime", processTime);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void SetEventProcessed (int eventId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CloseEvent", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "eventId", eventId);

                command.ExecuteNonQuery();
            }
        }
    }
}