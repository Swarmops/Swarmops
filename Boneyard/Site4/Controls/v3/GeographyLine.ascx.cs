using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Basic.Types;
using Activizr.Logic.Structure;

public partial class Controls_GeographyLine : System.Web.UI.UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{
		ConstructLine();
	}


	private void ConstructLine()
	{
		// Cut line down to three last nodes

		Geographies lastThree = geographies;  // shallow copy - bad

		if (lastThree.Count > 3)
		{
			lastThree.RemoveRange(0, geographies.Count - 3);
		}

		string result = lastThree [0].Name;

		for (int index = 1; index < lastThree.Count; index++)
		{
			result += Server.HtmlEncode(" > " + lastThree[index].Name);
		}

		this.LabelGeography.Text = result;
	}


	public Geography Geography
	{
		set
		{
			geographies = value.GetLine();
			ConstructLine();
		}
	}

	private Geographies geographies;
}
