using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types.Communications;
using Swarmops.Basic.Types.Financial;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;
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

                case "Swarmops.Basic.Types.BasicCommunicationTurnaround":
                    return CommunicationTurnaround.FromBasic((BasicCommunicationTurnaround)basic);

                case "Swarmops.Basic.Types.Communications.BasicOutboundComm":
                    return OutboundComm.FromBasic((BasicOutboundComm)basic);

                case "Swarmops.Basic.Types.Communications.BasicOutboundCommRecipient":
                    return OutboundCommRecipient.FromBasic((BasicOutboundCommRecipient)basic);


                // ----------- FINANCIAL CLASSES ----------

                case "Swarmops.Basic.Types.BasicExpenseClaim":
                    return ExpenseClaim.FromBasic((BasicExpenseClaim)basic);

                case "Swarmops.Basic.Types.Financial.BasicCashAdvance":
                    return CashAdvance.FromBasic((BasicCashAdvance)basic);

                case "Swarmops.Basic.Types.BasicInboundInvoice":
                    return InboundInvoice.FromBasic((BasicInboundInvoice) basic);

                case "Swarmops.Basic.Types.Financial.BasicFinancialAccount":
                    return FinancialAccount.FromBasic((BasicFinancialAccount)basic);

                case "Swarmops.Basic.Types.BasicFinancialTransaction":
                    return FinancialTransaction.FromBasic((BasicFinancialTransaction)basic);

                case "Swarmops.Basic.Types.BasicFinancialValidation":
                    return FinancialValidation.FromBasic((BasicFinancialValidation)basic);

                case "Swarmops.Basic.Types.BasicOutboundInvoice":
                    return OutboundInvoice.FromBasic((BasicOutboundInvoice)basic);

                case "Swarmops.Basic.Types.BasicOutboundInvoiceItem":
                    return OutboundInvoiceItem.FromBasic((BasicOutboundInvoiceItem)basic);

                case "Swarmops.Basic.Types.BasicPayment":
                    return Payment.FromBasic((BasicPayment)basic);

                case "Swarmops.Basic.Types.BasicPaymentGroup":
                    return PaymentGroup.FromBasic((BasicPaymentGroup)basic);

                case "Swarmops.Basic.Types.BasicPayout":
                    return Payout.FromBasic((BasicPayout)basic);

                case "Swarmops.Basic.Types.BasicPayrollAdjustment":
                    return PayrollAdjustment.FromBasic((BasicPayrollAdjustment) basic);

                case "Swarmops.Basic.Types.BasicPayrollItem":
                    return PayrollItem.FromBasic((BasicPayrollItem)basic);

                case "Swarmops.Basic.Types.BasicSalary":
                    return Salary.FromBasic((BasicSalary) basic);


                // ------------ GOVERNANCE CLASSES ------------

                case "Swarmops.Basic.Types.BasicBallot":
                    return Ballot.FromBasic((BasicBallot)basic);

                case "Swarmops.Basic.Types.BasicMeetingElectionCandidate":
                    return MeetingElectionCandidate.FromBasic((BasicInternalPollCandidate)basic);

                case "Swarmops.Basic.Types.BasicMeetingElection":
                    return MeetingElection.FromBasic((BasicInternalPoll)basic);

                case "Swarmops.Basic.Types.BasicMeetingElectionVote":
                    return MeetingElectionVote.FromBasic((BasicInternalPollVote)basic);

                    case "Swarmops.Basic.Types.Governance.BasicMotion":
                    return Motion.FromBasic((BasicMotion)basic);

                case "Swarmops.Basic.Types.Governance.BasicMotionAmendment":
                    return MotionAmendment.FromBasic((BasicMotionAmendment)basic);


                // ------------ PARLEY/ACTIVISM CLASSES ------------

                case "Swarmops.Basic.Types.BasicExternalActivity":
                    return ExternalActivity.FromBasic((BasicExternalActivity) basic);

                case "Swarmops.Basic.Types.BasicParley":
                    return Parley.FromBasic((BasicParley)basic);

                case "Swarmops.Basic.Types.BasicParleyAttendee":
                    return ParleyAttendee.FromBasic((BasicParleyAttendee)basic);

                case "Swarmops.Basic.Types.BasicParleyOption":
                    return ParleyOption.FromBasic((BasicParleyOption)basic);

                case "Swarmops.Basic.Types.BasicPerson":
                    return Person.FromBasic((BasicPerson)basic);

                // ------------------ FAIL ----------------

                default:
                    throw new NotImplementedException("Unimplemented argument type: " + argumentType);

            }


        }
    }
}
