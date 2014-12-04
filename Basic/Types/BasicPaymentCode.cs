using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicPaymentCode : IHasIdentity
    {
        private readonly int issuedToPersonId;
        private readonly string issuedToPhoneNumber;
        private readonly string paymentCode;
        private readonly int paymentCodeId;
        private bool claimed;

        #region IHasIdentity Members

        public int Identity
        {
            get { return PaymentCodeId; }
        }

        #endregion

        public BasicPaymentCode(int paymentCodeId, string paymentCode, string issuedToPhoneNumber, int issuedToPersonId,
            bool claimed)
        {
            this.paymentCodeId = paymentCodeId;
            this.paymentCode = paymentCode;
            this.issuedToPhoneNumber = issuedToPhoneNumber;
            this.issuedToPersonId = issuedToPersonId;
            this.claimed = claimed;
        }

        public BasicPaymentCode(BasicPaymentCode original)
            : this(
                original.paymentCodeId, original.paymentCode, original.issuedToPhoneNumber, original.issuedToPersonId,
                original.claimed)
        {
            // TODO: Expand with the rest of the fields
        }


        public int PaymentCodeId
        {
            get { return this.paymentCodeId; }
        }

        public string PaymentCode
        {
            get { return this.paymentCode; }
        }

        public string IssuedToPhoneNumber
        {
            get { return this.issuedToPhoneNumber; }
        }

        public int IssuedToPersonId
        {
            get { return this.issuedToPersonId; }
        }

        public bool Claimed
        {
            get { return this.claimed; }
        }


        public PaymentCodeIssueType IssueType
        {
            get
            {
                if (this.issuedToPersonId == 0)
                {
                    return PaymentCodeIssueType.Phone;
                }
                return PaymentCodeIssueType.Person;
            }
        }

        protected void SetClaimed()
        {
            this.claimed = true;
        }


        //private bool balanced;
    }
}