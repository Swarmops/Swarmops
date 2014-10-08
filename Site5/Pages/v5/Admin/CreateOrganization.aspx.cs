using System;
using System.Web;

namespace Swarmops.Frontend.Pages.v5.Admin
{
    public partial class CreateOrganization : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageTitle = Resources.Pages.Financial.FileExpenseClaim_PageTitle;
            this.PageIcon = "iconshock-moneyback";
            this.InfoBoxLiteral = Resources.Pages.Financial.FileExpenseClaim_Info;

            if (!Page.IsPostBack)
            {
                Localize();
            }
        }


        private void Localize()
        {

        }


        protected void ButtonRequest_Click(object sender, EventArgs e)
        {
            string successMessage = string.Empty;

            // Create org here

            Response.AppendCookie(new HttpCookie("DashboardMessage", HttpUtility.UrlEncode(successMessage)));

            // Redirect to dashboard

            Response.Redirect("/", true);
        }

    }
}