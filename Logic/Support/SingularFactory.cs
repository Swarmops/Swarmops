using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Communications;
using Activizr.Logic.Financial;
using Activizr.Logic.Governance;
using Activizr.Logic.Pirates;
using Activizr.Basic.Interfaces;
using Activizr.Basic.Types;
using Activizr.Basic.Types.Governance;

namespace Activizr.Logic.Support
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

                case "PirateWeb.Basic.Types.BasicCommunicationTurnaround":
                    return CommunicationTurnaround.FromBasic((BasicCommunicationTurnaround)basic);


                // ----------- FINANCIAL CLASSES ----------

                case "PirateWeb.Basic.Types.BasicExpenseClaim":
                    return ExpenseClaim.FromBasic((BasicExpenseClaim) basic);

                case "PirateWeb.Basic.Types.BasicInboundInvoice":
                    return InboundInvoice.FromBasic((BasicInboundInvoice) basic);

                case "PirateWeb.Basic.Types.BasicFinancialAccount":
                    return FinancialAccount.FromBasic((BasicFinancialAccount)basic);

                case "PirateWeb.Basic.Types.BasicFinancialTransaction":
                    return FinancialTransaction.FromBasic((BasicFinancialTransaction)basic);

                case "PirateWeb.Basic.Types.BasicFinancialValidation":
                    return FinancialValidation.FromBasic((BasicFinancialValidation)basic);

                case "PirateWeb.Basic.Types.BasicOutboundInvoice":
                    return OutboundInvoice.FromBasic((BasicOutboundInvoice)basic);

                case "PirateWeb.Basic.Types.BasicOutboundInvoiceItem":
                    return OutboundInvoiceItem.FromBasic((BasicOutboundInvoiceItem)basic);

                case "PirateWeb.Basic.Types.BasicPayment":
                    return Payment.FromBasic((BasicPayment)basic);

                case "PirateWeb.Basic.Types.BasicPaymentGroup":
                    return PaymentGroup.FromBasic((BasicPaymentGroup)basic);

                case "PirateWeb.Basic.Types.BasicPayout":
                    return Payout.FromBasic((BasicPayout)basic);

                case "PirateWeb.Basic.Types.BasicPayrollAdjustment":
                    return PayrollAdjustment.FromBasic((BasicPayrollAdjustment) basic);

                case "PirateWeb.Basic.Types.BasicPayrollItem":
                    return PayrollItem.FromBasic((BasicPayrollItem)basic);

                case "PirateWeb.Basic.Types.BasicSalary":
                    return Salary.FromBasic((BasicSalary) basic);


                // ------------ GOVERNANCE CLASSES ------------

                case "PirateWeb.Basic.Types.BasicBallot":
                    return Ballot.FromBasic((BasicBallot)basic);

                case "PirateWeb.Basic.Types.BasicMeetingElectionCandidate":
                    return MeetingElectionCandidate.FromBasic((BasicInternalPollCandidate)basic);

                case "PirateWeb.Basic.Types.BasicMeetingElection":
                    return MeetingElection.FromBasic((BasicInternalPoll)basic);

                case "PirateWeb.Basic.Types.BasicMeetingElectionVote":
                    return MeetingElectionVote.FromBasic((BasicInternalPollVote)basic);

                    case "PirateWeb.Basic.Types.Governance.BasicMotion":
                    return Motion.FromBasic((BasicMotion)basic);

                case "PirateWeb.Basic.Types.Governance.BasicMotionAmendment":
                    return MotionAmendment.FromBasic((BasicMotionAmendment)basic);


                // ------------ PIRATE CLASSES ------------

                case "PirateWeb.Basic.Types.BasicExternalActivity":
                    return ExternalActivity.FromBasic((BasicExternalActivity) basic);

                case "PirateWeb.Basic.Types.BasicParley":
                    return Parley.FromBasic((BasicParley)basic);

                case "PirateWeb.Basic.Types.BasicParleyAttendee":
                    return ParleyAttendee.FromBasic((BasicParleyAttendee)basic);

                case "PirateWeb.Basic.Types.BasicParleyOption":
                    return ParleyOption.FromBasic((BasicParleyOption)basic);

                case "PirateWeb.Basic.Types.BasicPerson":
                    return Person.FromBasic((BasicPerson)basic);

                // ------------------ FAIL ----------------

                default:
                    throw new NotImplementedException("Unimplemented argument type: " + argumentType);

            }


        }
    }
}
