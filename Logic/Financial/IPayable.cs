using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Interfaces;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public interface IPayable: IApprovable, IHasIdentity
    {
        new FinancialAccount Budget { get; }
        void SetBudget (FinancialAccount newBudget, Person settingPerson);

        Int64 AmountCents { get; }
        void SetAmountCents (Int64 newAmount, Person settingPerson);

        string Description { get; }

        bool PaidOut { get; set; }
        bool Open { get; set; }
    }
}
