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
using Activizr.Logic.Communications;
using Activizr.Logic.Pirates;


public partial class Pages_v4_admin_validatecansee : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {

    }

    protected void Button1_Click (object sender, EventArgs e)
    {
        if (!_authority.HasRoleType(Activizr.Basic.Enums.RoleType.SystemAdmin))
        { //Will be stopped by masterpage logic, but just in case someone mess up permissions in menu setup.
            Response.Write("Not allowed");
            Response.End();
        }

        Person p1 = Person.FromIdentity(Convert.ToInt32(TextBox1.Text));
        Person p2 = Person.FromIdentity(Convert.ToInt32(TextBox2.Text));
        if (p1.GetAuthority().CanSeePerson(p2))
        {
            Label1.Text = TextBox1.Text + " CAN see " + TextBox2.Text;
        }
        else
        {
            Label1.Text = TextBox1.Text + " CAN NOT see " + TextBox2.Text;
        }


    }

}

