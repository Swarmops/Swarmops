using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicExpenseClaimGroup: IHasIdentity
    {
        public BasicExpenseClaimGroup(int expenseClaimGroupId, DateTime createdDateTime, int createdByPersonId, bool open, ExpenseClaimGroupType groupType, string groupData)
        {
            this.ExpenseClaimGroupId = expenseClaimGroupId;
            this.CreatedDateTime = createdDateTime;
            this.CreatedByPersonId = createdByPersonId;
            this.Open = open;
            this.GroupType = groupType;
            this.GroupData = groupData;
        }

        public BasicExpenseClaimGroup(BasicExpenseClaimGroup original)
            : this (original.ExpenseClaimGroupId, original.CreatedDateTime, original.CreatedByPersonId, original.Open, original.GroupType, original.GroupData)
        {
            // copy ctor
        }

        public int ExpenseClaimGroupId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int CreatedByPersonId { get; set; }
        public bool Open { get; set; }
        public ExpenseClaimGroupType GroupType { get; set; }
        public string GroupData { get; set; }


        public int Identity {  get { return this.ExpenseClaimGroupId; } }
    }
}
