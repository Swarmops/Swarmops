using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Swarm
{
    public class BasicApplicant: IHasIdentity
    {



        public int ApplicantId { get; private set; }
        public int PersonId { get; private set; }
        public int OrganizationId { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public int Score1 { get; protected set; }
        public int Score2 { get; protected set; }
        public int Score3 { get; protected set; }

        public int Identity { get { return ApplicantId; } }
    }
}
