using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Logic.Support;

public partial class Pages_Special_Debug_Default : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		this.LabelDebug.Text = PirateWebDebug.GetDebugInformation();
	}
}
