using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Logic.Pirates;


public partial class Controls_v3_PersonList : System.Web.UI.UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{

	}
	protected void PeopleDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
	{
		e.InputParameters["people"] = (People)HttpContext.Current.Session["PersonList"];
	}

	protected void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
	{

		switch (e.CommandName)
		{
			case "ViewEdit":
				int index = Convert.ToInt32(e.CommandArgument);
				int personId = Convert.ToInt32(this.Grid.DataKeys[index].Value);
				Person displayedPerson = Person.FromIdentity(personId);
				HttpContext.Current.Session["DisplayedPerson"] = displayedPerson;
				Response.Redirect("~/Pages/v3/Members/BasicDetails.aspx");
				break;
		}
	}

	public People PersonList
	{
		set
		{
			HttpContext.Current.Session["PersonList"] = value; // Set for PeopleDataSource_Selecting
			Grid.DataBind();
		}
	}
    protected void Grid_RowDataBound (object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label phoneLabel = e.Row.FindControl("LabelPhone") as Label;
            if (!string.IsNullOrEmpty(phoneLabel.Text))
            {
                string phone=phoneLabel.Text.Trim();
                if (!(phone.StartsWith("+") || phone.StartsWith("00")))
                {
                    phone ="+46"+ phone.Substring(1);
                }
                phoneLabel.Text = "<a href='callto:" + phone + "'>" + phoneLabel.Text + "</a>";
            }
        }
    }
}
