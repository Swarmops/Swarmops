using System;
using Resources.Pages;

public partial class Security_AccessDenied : PageV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        PageTitle = Security.AccessDenied_PageTitle;
        PageIcon = "iconshock-disconnect";

        this.LabelAccessDeniedHeader.Text = Security.AccessDenied_Header;
        InfoBoxLiteral = Security.AccessDenied_Info;
        this.LiteralAccessDeniedRant.Text = String.Format (Security.AccessDenied_Rant,
            CurrentOrganization.Name);
    }
}