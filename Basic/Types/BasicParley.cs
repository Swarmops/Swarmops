using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicParley: IHasIdentity
    {
        public BasicParley (int parleyId, int organizationId, int personId, int budgetId, DateTime createdDateTime, bool open, bool attested, string name, int geographyId, string description, string informationUrl, DateTime startDate, DateTime endDate, Int64 budgetCents, Int64 guaranteeCents,
            Int64 attendanceFeeCents, DateTime closedDateTime)
        {
            this.ParleyId = parleyId;
            this.OrganizationId = organizationId;
            this.PersonId = personId;
            this.BudgetId = budgetId;
            this.CreatedDateTime = createdDateTime;
            this.Open = open;
            this.Attested = attested;
            this.Name = name;
            this.GeographyId = geographyId;
            this.Description = description;
            this.InformationUrl = informationUrl;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.BudgetCents = budgetCents;
            this.GuaranteeCents = guaranteeCents;
            this.AttendanceFeeCents = attendanceFeeCents;
            this.ClosedDateTime = closedDateTime;
        }

        public BasicParley (BasicParley original)
            : this (original.ParleyId, original.OrganizationId, original.PersonId, original.BudgetId, original.CreatedDateTime, original.Open, original.Attested, original.Name, original.GeographyId, original.Description, original.InformationUrl, original.StartDate, original.EndDate, original.BudgetCents, original.GuaranteeCents, original.AttendanceFeeCents, original.ClosedDateTime)
        {
            // empty copy ctor
        }


        public int ParleyId { get; private set; }
        public int OrganizationId { get; private set; }
        public int PersonId { get; private set; }
        public int BudgetId { get; protected set; }
        public bool Open { get; protected set; }
        public string Name { get; protected set; }
        public int GeographyId { get; protected set; }
        public string Description { get; protected set; }
        public string InformationUrl { get; protected set; }
        public DateTime StartDate { get; protected set; }
        public DateTime EndDate { get; protected set; }
        public Int64 BudgetCents { get; protected set; }
        public Int64 GuaranteeCents { get; protected set; }
        public Int64 AttendanceFeeCents { get; protected set; }
        public bool Attested { get; protected set; }
        public DateTime CreatedDateTime { get; private set; }
        public DateTime ClosedDateTime { get; protected set; }

        public int Identity
        {
            get { return this.ParleyId; }
        }
    }
}
