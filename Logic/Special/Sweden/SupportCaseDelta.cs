using System;
using Swarmops.Common.Interfaces;

namespace Swarmops.Logic.Special.Sweden
{
    public class SupportCaseDelta : IHasIdentity
    {
        public SupportCaseDelta (int supportCaseDeltaId, int supportCaseId, int supportPersonId, DateTime dateTime,
            string verb, string changes)
        {
            SupportCaseDeltaId = supportCaseDeltaId;
            SupportCaseId = supportCaseId;
            Verb = verb;
            DateTime = dateTime;
            Changes = changes;
            SupportPersonId = supportPersonId;
        }


        public int SupportCaseDeltaId { get; private set; }
        public int SupportCaseId { get; private set; }
        public DateTime DateTime { get; private set; }
        public string Verb { get; private set; }
        public string Changes { get; private set; }
        public int SupportPersonId { get; private set; }


        public int Identity
        {
            get { return SupportCaseDeltaId; }
        }
    }
}