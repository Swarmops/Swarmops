using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class _Default : PageV4Base 
{
	protected void Page_Load(object sender, EventArgs e)
	{
        ((MasterV4Base)this.Master).CurrentPageAllowed = true;

	    this.ListOfficerTodos.Person = _currentUser;
	    this.ListLocalContacts.Person = _currentUser;
	    this.ListBudgets.Person = _currentUser;
	}
}
