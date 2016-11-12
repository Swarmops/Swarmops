using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Controls_v3_PersonDetailsPagesMenu : System.Web.UI.UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{
		switch (currentPage)
		{
			case 0:
				this.LinkBasicDetails.NavigateUrl = string.Empty;
				// this.LinkBasicDetails.Font.Bold = true;
				break;
			case 1:
				this.LinkMemberships.NavigateUrl = string.Empty;
				// this.LinkMemberships.Font.Bold = true;
				break;
			case 2:
				this.LinkRolesResponsibilities.NavigateUrl = string.Empty;
				// this.LinkRolesResponsibilities.Font.Bold = true;
				break;
			case 3:
				this.LinkAccountSettings.NavigateUrl = string.Empty;
				break;
		}
	}

	public int CurrentPage
	{
		set
		{
			this.currentPage = value;
		}
	}

	private int currentPage;
}
