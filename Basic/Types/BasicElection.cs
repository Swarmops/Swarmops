using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicElection : IHasIdentity
    {
        public BasicElection(int electionId, string name, int geographyId, DateTime date)
        {
            ElectionId = electionId;
            Name = name;
            GeographyId = geographyId;
            Date = date;
        }

        public BasicElection(BasicElection original) :
            this(original.ElectionId, original.Name, original.GeographyId, original.Date)
        {
            // empty copy ctor
        }

        public int ElectionId { get; private set; }
        public string Name { get; private set; }
        public int GeographyId { get; private set; }
        public DateTime Date { get; private set; }

        public int Identity
        {
            get { return ElectionId; }
        }
    }
}