using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicPaymentCode : IHasIdentity
    {
        public BasicPaymentCode (int paymentCodeId, string paymentCode, string issuedToPhoneNumber, int issuedToPersonId,
                                 bool claimed)
        {
            this.paymentCodeId = paymentCodeId;
            this.paymentCode = paymentCode;
            this.issuedToPhoneNumber = issuedToPhoneNumber;
            this.issuedToPersonId = issuedToPersonId;
            this.claimed = claimed;
        }

        public BasicPaymentCode (BasicPaymentCode original)
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


        protected void SetClaimed()
        {
            this.claimed = true;
        }


        public PaymentCodeIssueType IssueType
        {
            get
            {
                if (issuedToPersonId == 0)
                {
                    return PaymentCodeIssueType.Phone;
                }
                else
                {
                    return PaymentCodeIssueType.Person;
                }
            }
        }


        private int paymentCodeId;
        private string paymentCode;
        private string issuedToPhoneNumber;
        private int issuedToPersonId;
        private bool claimed;
        //private bool balanced;

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.PaymentCodeId; }
        }

        #endregion
    }
}