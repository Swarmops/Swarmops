using System;
using System.Collections.Generic;
using System.Text;

namespace Activizr.Basic.Enums
{
	public enum AutoMailType
	{
		Unknown = 0,
		Welcome,
        MemberMail,
        OfficerMail,
        Reminder,
        ActivistMail,
        Expired,
        Newsletter,
        PressRelease,
        WelcomeTimeLimitedContent=100 //Not implemented yet..
	}
}
