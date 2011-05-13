using System;
using System.Collections.Generic;
using System.Text;

namespace Activizr.Basic.Enums
{
    /// <summary>
    /// For handling of membership fee payment
    /// Also used for marking of accepted memberships for PPFI
    /// </summary>
    public enum MembershipPaymentStatus
    {
        /// <summary>
        /// Undefined
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Newly registered
        /// </summary>
        NewlyRegistered = 1,
        /// <summary>
        /// Membership accepted, used as bit flag
        /// </summary>
        MembershipAccepted = 2,
        /// <summary>
        /// Due for payment, to be used if payments are 
        /// </summary>
        DueForNewPayment = 3,
        /// <summary>
        /// Has been listed to receive notice of payment 
        /// to be used if the sending out of payments is a manual process
        /// just to stop them being listed as Due for payment again
        /// </summary>
        ListedForPayment = 4,
        /// <summary>
        /// Notice of payment have been sent
        /// </summary>
        PaymentRequested = 5,
        /// <summary>
        /// Payment have been recieved
        /// </summary>
        PaymentRecieved = 6

    }
}
