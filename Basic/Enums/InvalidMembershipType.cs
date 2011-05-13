using System;
using System.Collections.Generic;
using System.Text;

namespace Activizr.Basic.Enums
{
    public enum InvalidMembershipType
    {
        /// <summary>
        /// This membership was closed normally.
        /// </summary>
        Valid = 0,
        /// <summary>
        /// This membership was closed because it was a duplicate.
        /// </summary>
        DuplicateMember = 1,
        /// <summary>
        /// This membership was closed because the personae were bogus (like "Mickey Mouse").
        /// </summary>
        PersonDoesNotExist = 2
    }
}