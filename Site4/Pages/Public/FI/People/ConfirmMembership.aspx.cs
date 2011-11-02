using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;

public partial class Pages_Public_FI_People_ConfirmMembership : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
        try
        {
            string token = Request.QueryString["ticket"].ToString();
            int pid = int.Parse(Request.QueryString["member"]);
            Person pers = Person.FromIdentity(pid);

            Authentication.ValidateEmailVerificationTicket(pers, token);

            pers.PasswordHash = pers.TempPasswordHash; //set the password originally requested;

            pers.ResetPasswordTicket = "";

            Response.Redirect("http://www.piraattipuolue.fi/jaesenyys", true);
        }
        catch (Exception ex)
        {
            Div1.Visible = true;
            Div2.InnerHtml = "<pre>" + ex.Message + "</pre>";
        }

    }
}
