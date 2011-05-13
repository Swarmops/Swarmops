using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Database;
using System.Data.Common;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using MySql.Data.MySqlClient;
using Activizr.Basic.Types;


public partial class Pages_v4_admin_Exceptions : PageV4Base
{

    protected void Page_Load (object sender, EventArgs e)
    {
        if (!_authority.HasRoleType(Activizr.Basic.Enums.RoleType.SystemAdmin))
        { //Will be stopped by masterpage logic, but just in case someone mess up permissions in menu setup.
            Response.Write("Not allowed");
            Response.End();
        }
        RebuildTable();
    }
    
    protected void Button1_Click (object sender, EventArgs e)
    {
        RebuildTable();
    }

    private void RebuildTable ()
    {
        tab.Rows.Clear();

        TableRow row = new TableRow();
        tab.Rows.Add(row);
        TableCell c = null;
        c = new TableCell();
        row.Cells.Add(c);
        c.Text = "Id";
        c = new TableCell();
        row.Cells.Add(c);
        c.Text = "Source";
        c = new TableCell();
        row.Cells.Add(c);
        c.Text = "Time";
        c = new TableCell();
        row.Cells.Add(c);
        c.Text = "Exception";

        BasicExceptionLog[] exceptions = Activizr.Database.PirateDb.GetDatabase().GetExceptionLogTopEntries(500);
        foreach (BasicExceptionLog item in exceptions)
        {
            row = new TableRow();
            tab.Rows.Add(row);

            c = new TableCell(); row.Cells.Add(c);
            c.Text = item.ExceptionId.ToString();

            c = new TableCell(); row.Cells.Add(c);
            c.Text = item.Source;

            c = new TableCell(); row.Cells.Add(c);
            c.Text = item.ExceptionDateTime.ToString();
            c.Attributes["nowrap"] = "nowrap";

            c = new TableCell(); row.Cells.Add(c);
            HtmlGenericControl preTag = new HtmlGenericControl("PRE");
            c.Controls.Add(preTag);
            preTag.Style["font-size"] = "0.9em";
            preTag.InnerHtml = item.ExceptionText.ToString();
        }
    }
}
