using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Structure;
using Swarmops.Utility.Special.Sweden;
using Swarmops.Database;
using Swarmops.Logic.Special.Sweden;
using Swarmops.Logic.Support;

namespace Swarmops.Utility.BotCode
{
    public class TurnaroundTracker
    {
        public static void Run()
        {
            const string persistenceKey = "PPSE-LastSupportDeltaId";

            string lastDeltaIdString = Persistence.Key[persistenceKey];
            int lastDeltaId = 0;

            if (!String.IsNullOrEmpty(lastDeltaIdString))
            {
                lastDeltaId = Int32.Parse(lastDeltaIdString);
            }

            SupportCaseDelta[] deltas = SupportDatabase.GetCaseDeltas(lastDeltaId);

            foreach (SupportCaseDelta delta in deltas)
            {
                if (delta.Verb == "Incoming email")
                {
                    try
                    {
                        CommunicationTurnaround.Create(Organization.PPSE, delta.SupportCaseId, delta.DateTime);
                    }
                    catch (Exception)
                    {
                        // Ignore if can't create
                    }
                }
                else if (delta.Verb == "Replied")
                {
                    try
                    {
                        CommunicationTurnaround turnaround = CommunicationTurnaround.FromIdentity(Organization.PPSE, 1,
                                                                                                  delta.SupportCaseId);
                        turnaround.SetResponded(delta.DateTime, null);
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (delta.Verb == "Closed")
                {
                    try
                    {
                        CommunicationTurnaround turnaround = CommunicationTurnaround.FromIdentity(Organization.PPSE, 1,
                                                                                                  delta.SupportCaseId);
                        turnaround.Close(delta.DateTime, null);
                    }
                    catch (Exception)
                    {
                        // if can't find, ignore
                    }
                }
                else if (delta.Changes.Contains("'Spam'"))
                {
                    try
                    {
                        CommunicationTurnaround turnaround = CommunicationTurnaround.FromIdentity(Organization.PPSE, 1,
                                                                                                  delta.SupportCaseId);
                        turnaround.Close(delta.DateTime, null);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (delta.SupportCaseDeltaId > lastDeltaId)
                {
                    lastDeltaId = delta.SupportCaseDeltaId;
                }
            }

            Persistence.Key[persistenceKey] = lastDeltaId.ToString();
        }

        public static void Housekeeping()
        {
            CommunicationTurnarounds turnarounds = CommunicationTurnarounds.ForOrganization(Organization.PPSE);

            foreach (CommunicationTurnaround turnaround in turnarounds)
            {
                if (!SupportDatabase.IsCaseOpen(turnaround.CommunicationId))
                {
                    DateTime? dateTimeClosed = SupportDatabase.GetCaseCloseDateTime(turnaround.CommunicationId);
                    if (dateTimeClosed == null)
                    {
                        dateTimeClosed = turnaround.DateTimeOpened;
                    }

                    PirateDb.GetDatabaseForWriting().SetCommunicationTurnaroundClosed(Organization.PPSEid, 1, turnaround.CommunicationId, (DateTime) dateTimeClosed, 0);
                }
            }
        }
    }
}
