using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Pirates;
using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Governance;
using Swarmops.Logic.Governance;

namespace Swarmops.Logic.Support
{
    /// <summary>
    /// This class supports the PluralBase foundation. Mirroring FromBasic() here is the
    /// price we pay to get FromArray(), FromSingle(), LogicalOr() etc in all the plural 
    /// classes.
    /// </summary>
    internal class SingularFactory
    {
        static public object FromBasic(IHasIdentity basic)
        {
            string argumentType = basic.GetType().ToString();

            switch (argumentType)
            {
                // ----- Is there any way to make self-writing code here through replication or similar, so
                // ----- that every case doesn't need to be listed?


                // ------------ COMMUNICATION CLASSES ------------

                case "Activizr.Basic.Types.BasicCommunicationTurnaround":
                    return CommunicationTurnaround.FromBasic((BasicCommunicationTurnaround)basic);


                // ----------- FINANCIAL CLASSES ----------

                case "Activizr.Basic.Types.BasicExpenseClaim":
                    return ExpenseClaim.FromBasic((BasicExpenseClaim) basic);

                case "Activizr.Basic.Types.BasicInboundInvoice":
                    return InboundInvoice.FromBasic((BasicInboundInvoice) basic);

                case "Activizr.Basic.Types.BasicFinancialAccount":
                    return FinancialAccount.FromBasic((BasicFinancialAccount)basic);

                case "Activizr.Basic.Types.BasicFinancialTransaction":
                    return FinancialTransaction.FromBasic((BasicFinancialTransaction)basic);

                case "Activizr.Basic.Types.BasicFinancialValidation":
                    return FinancialValidation.FromBasic((BasicFinancialValidation)basic);

                case "Activizr.Basic.Types.BasicOutboundInvoice":
                    return OutboundInvoice.FromBasic((BasicOutboundInvoice)basic);

                case "Activizr.Basic.Types.BasicOutboundInvoiceItem":
                    return OutboundInvoiceItem.FromBasic((BasicOutboundInvoiceItem)basic);

                case "Activizr.Basic.Types.BasicPayment":
                    return Payment.FromBasic((BasicPayment)basic);

                case "Activizr.Basic.Types.BasicPaymentGroup":
                    return PaymentGroup.FromBasic((BasicPaymentGroup)basic);

                case "Activizr.Basic.Types.BasicPayout":
                    return Payout.FromBasic((BasicPayout)basic);

                case "Activizr.Basic.Types.BasicPayrollAdjustment":
                    return PayrollAdjustment.FromBasic((BasicPayrollAdjustment) basic);

                case "Activizr.Basic.Types.BasicPayrollItem":
                    return PayrollItem.FromBasic((BasicPayrollItem)basic);

                case "Activizr.Basic.Types.BasicSalary":
                    return Salary.FromBasic((BasicSalary) basic);


                // ------------ GOVERNANCE CLASSES ------------

                case "Activizr.Basic.Types.BasicBallot":
                    return Ballot.FromBasic((BasicBallot)basic);

                case "Activizr.Basic.Types.BasicMeetingElectionCandidate":
                    return MeetingElectionCandidate.FromBasic((BasicInternalPollCandidate)basic);

                case "Activizr.Basic.Types.BasicMeetingElection":
                    return MeetingElection.FromBasic((BasicInternalPoll)basic);

                case "Activizr.Basic.Types.BasicMeetingElectionVote":
                    return MeetingElectionVote.FromBasic((BasicInternalPollVote)basic);

                    case "Activizr.Basic.Types.Governance.BasicMotion":
                    return Motion.FromBasic((BasicMotion)basic);

                case "Activizr.Basic.Types.Governance.BasicMotionAmendment":
                    return MotionAmendment.FromBasic((BasicMotionAmendment)basic);


                // ------------ PIRATE CLASSES ------------

                case "Activizr.Basic.Types.BasicExternalActivity":
                    return ExternalActivity.FromBasic((BasicExternalActivity) basic);

                case "Activizr.Basic.Types.BasicParley":
                    return Parley.FromBasic((BasicParley)basic);

                case "Activizr.Basic.Types.BasicParleyAttendee":
                    return ParleyAttendee.FromBasic((BasicParleyAttendee)basic);

                case "Activizr.Basic.Types.BasicParleyOption":
                    return ParleyOption.FromBasic((BasicParleyOption)basic);

                case "Activizr.Basic.Types.BasicPerson":
                    return Person.FromBasic((BasicPerson)basic);

                // ------------------ FAIL ----------------

                default:
                    throw new NotImplementedException("Unimplemented argument type: " + argumentType);

            }


        }
    }
}
