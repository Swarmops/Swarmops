using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Financial;
using Activizr.Logic.Structure;
using Activizr.Logic.Special.Mail;
using Telerik.Web.UI;
using Activizr.Basic.Enums;
using Activizr.Logic.Security;

public partial class Pages_v4_Admin_MailServerAccounts : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        if (!_authority.HasPermission(Permission.CanEditMailDB,Organization.PPSEid,-1,Authorization.Flag.AnyGeographyExactOrganization))
            Master.CurrentPageProhibited=true;
        ButtonAdd.UseSubmitBehavior = false;
        ButtonAdd.Attributes["onclick"] = "return ShowAddForm(''+document.getElementById('" + TextBoxFindAccount.ClientID + "').value);";
    }

    protected void RadGrid1_ItemDataBound (object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            MailServerDatabase.MailAccount item = e.Item.DataItem as MailServerDatabase.MailAccount;
            GridDataItem gridItem = e.Item as GridDataItem;
            string toForwardings = "";
            string fromForwardings = "";
            foreach (string tmp in item.forwardedTo)
                toForwardings += "<a href='#' onclick=\"ShowEditForm('"+tmp+"')\">" + tmp + "</a><br>";
            foreach (string tmp in item.forwardedFrom)
                fromForwardings += "<a href='#' onclick=\"ShowEditForm('" + tmp + "')\">" + tmp + "</a><br>";
            gridItem["ForwardedTo"].Text = toForwardings;
            gridItem["ForwardedFrom"].Text = fromForwardings;
            gridItem["Account"].Text = "<a href='#' onclick=\"ShowEditForm('" + item.account + "')\">" + item.account + "</a>";

        }
    }

    private void PopulateGrid ()
    {
        List<MailServerDatabase.MailAccount> accountList = MailServerDatabase.SearchAccount(TextBoxFindAccount.Text);
        RadGrid1.DataSource = accountList;
    }

    protected void ButtonFind_Click (object sender, EventArgs e)
    {
        PopulateGrid();
        RadGrid1.DataBind();

    }


    protected void RadAjaxManager1_AjaxRequest (object sender, AjaxRequestEventArgs e)
    {
        if (e.Argument == "Rebind")
        {
            //this.GridTransactions.MasterTableView.SortExpressions.Clear();
            //this.GridTransactions.MasterTableView.GroupByExpressions.Clear();
            PopulateGrid();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            /* This should not happen. */
        }
    }

    protected void RadGrid1_NeedDataSource (object source, GridNeedDataSourceEventArgs e)
    {
        PopulateGrid();
    }
}
