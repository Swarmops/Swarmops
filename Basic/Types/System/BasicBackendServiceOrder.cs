using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.System
{
    public class BasicBackendServiceOrder: IHasIdentity
    {

        public BasicBackendServiceOrder(int backendServiceOrderId, int organizationId, int personId,
            string backendServiceClassName, string orderXml,
            bool open, bool active, DateTime createdDateTime, DateTime startedDateTime, DateTime closedDateTime,
            string exceptionText)
        {
            this.BackendServiceOrderId = backendServiceOrderId;
            this.OrganizationId = organizationId;
            this.PersonId = personId;
            this.BackendServiceClassName = backendServiceClassName;
            this.OrderXml = orderXml;
            this.Open = open;
            this.Active = active;
            this.CreatedDateTime = createdDateTime;
            this.StartedDateTime = startedDateTime;
            this.ClosedDateTime = closedDateTime;
            this.ExceptionText = exceptionText;
        }

        public BasicBackendServiceOrder(BasicBackendServiceOrder original) :
            this(original.BackendServiceOrderId, original.OrganizationId, original.PersonId,
                original.BackendServiceClassName, original.OrderXml,
                original.Open, original.Active, original.CreatedDateTime, original.StartedDateTime,
                original.ClosedDateTime, original.ExceptionText)
        {
            // empty copy ctor required for higher logic
        }

        // fields

        public int BackendServiceOrderId { get; set; }
        public int OrganizationId { get; set; }
        public int PersonId { get; set; }
        public string BackendServiceClassName { get; set; }
        public string OrderXml { get; set; }
        public bool Open { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime StartedDateTime { get; set; }
        public DateTime ClosedDateTime { get; set; }
        public string ExceptionText { get; set; }

        public int Identity
        {
            get { return this.BackendServiceOrderId;  }
        }
    }
}
