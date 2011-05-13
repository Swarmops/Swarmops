using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Activizr.Interface.Collections;
using Activizr.Interface.Objects;

namespace Activizr.Interface.DataObjects
{
#if !__MonoCS__
	[DataObject(true)]
#endif
	public class ExpensePayoutsDataObject
	{
#if !__MonoCS__
		[DataObjectMethod(DataObjectMethodType.Select)]
#endif
		static public ExpensePayout[] SelectByOrganization(int organizationId)
		{
			return ExpensePayouts.FromOrganization(organizationId).ToArray();
		}
	}
}
