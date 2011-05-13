using System;
using System.Collections.Generic;
using System.Text;

namespace Activizr.Basic.Enums
{
    public enum EventType
    {
        Unknown = 0,
        AddedRole,
        DeletedRole,
        AddedMember,
        AddedMembership,
        ExtendedMembership,
        TerminatedMembership,
        ReceivedMembershipPayment,
        ExpenseCreated,
        ExpenseChanged,
        ExpensesRepaidClosed,
        EmailAccountRequested,
        RefreshEmailAccount,
        LostMember,
        NewActivist,
        LostActivist,
        NewVolunteer,
        TransferredMembership,
        ExpenseAttested,
        ExpenseValidated,
        InboundInvoiceReceived,
        InboundInvoiceAttested,
        InboundInvoiceClosed,
        PaperLetterReceived,
        PayoutCreated,
        SalaryCreated,
        SalaryAttested,
        LocalDonationReceived,
        PhoneMessagesCreated,
        ActivistMailsCreated,
        OutboundInvoiceCreated,
        OutboundInvoicePaid,
        CandidateDocumentationReceived,
        ActivismLogged,
        CryptoKeyRequested,
        ParleyCreated,
        ParleyAttested,
        ParleyCancelled,
        ParleyClosed,
        ParleyAttendeeCreated,
        ParleyAttendeeConfirmed,
        FinancialDataUploaded,
        RefundCreated,
    }

    public enum EventSource
    {
        Unknown = 0,
        PirateWeb,
        PirateBot,
        WebServices,
        SignupPage,
        CustomServiceInterface,
        SMS
    }
}