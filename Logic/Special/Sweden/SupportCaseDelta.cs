using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Logic.Special.Sweden
{
    public class SupportCaseDelta: IHasIdentity
    {
        public SupportCaseDelta (int supportCaseDeltaId, int supportCaseId, int supportPersonId, DateTime dateTime, string verb, string changes)
        {
            this.SupportCaseDeltaId = supportCaseDeltaId;
            this.SupportCaseId = supportCaseId;
            this.Verb = verb;
            this.DateTime = dateTime;
            this.Changes = changes;
            this.SupportPersonId = supportPersonId;
        }


        public int SupportCaseDeltaId { get; private set; }
        public int SupportCaseId { get; private set; }
        public DateTime DateTime { get; private set; }
        public string Verb { get; private set; }
        public string Changes { get; private set; }
        public int SupportPersonId { get; private set; }


        public int Identity
        {
            get { return this.SupportCaseDeltaId; }
        }
    }
}
