using System;
using System.Collections.Generic;
using System.Text;

namespace PirateWeb.Types
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
		ReceivedPayment
	}

	public enum EventSource
	{
		Unknown = 0,
		PirateWeb,
		PirateBot,
		WebServices,
		SignupPage,
		CustomServiceInterface
	}
}
