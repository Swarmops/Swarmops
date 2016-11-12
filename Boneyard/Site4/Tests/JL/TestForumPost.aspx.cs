using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Special.Sweden;
using Activizr.Logic.Pirates;

public partial class Tests_JL_TestForumPost : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
        try
        {
            IForumDatabase forum= SwedishForumDatabase.GetDatabase();

            forum.TestMode = true;

            int forumPostId = forum.CreateNewPost(0, Person.FromIdentity( 7838),
                                            "Nyhetsbrev " + DateTime.Today.ToString("yyyy-MM-dd"),
                                            "TestTitle", "TestContenttext");

            Response.Write(forumPostId);
        }
        catch (Exception ex)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {   // ignore when debugging
                throw ex;
            }
        }

    }
}
