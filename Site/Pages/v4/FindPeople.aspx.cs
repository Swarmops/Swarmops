using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Activizr.Logic.Pirates;

using Telerik.Web.UI;

public partial class Pages_v4_FindPeople : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            GridPeople.DataSource = People.FromSingle(Person.FromIdentity(1));
        }
    }

    protected void GridPeople_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            HyperLink editLink = (HyperLink)e.Item.FindControl("EditLink");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowManageForm('{0}','{1}');",
                                                           e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex][
                                                               "Identity"], e.Item.ItemIndex);

            Label labelGeography = (Label)e.Item.FindControl("LabelGeography");
            Person currentPerson = e.Item.DataItem as Person;
            if (currentPerson != null)
            {
                string geographyName = currentPerson.Country.Code + "-" + currentPerson.PostalCode;

                geographyName += " " + currentPerson.Geography.Name;

                if (currentPerson.Geography.ParentGeographyId != 0)
                {
                    geographyName += ", " + currentPerson.Geography.Parent.Name;
                }

                labelGeography.Text = geographyName;
            }
        }
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        if (e.Argument == "Rebind")
        {
            this.GridPeople.MasterTableView.SortExpressions.Clear();
            this.GridPeople.MasterTableView.GroupByExpressions.Clear();
            // PopulateGrids();
            this.GridPeople.Rebind();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            this.GridPeople.MasterTableView.SortExpressions.Clear();
            this.GridPeople.MasterTableView.GroupByExpressions.Clear();
            this.GridPeople.MasterTableView.CurrentPageIndex = this.GridPeople.MasterTableView.PageCount - 1;
            // PopulateGrids();
            this.GridPeople.Rebind();
        }
    }
}
