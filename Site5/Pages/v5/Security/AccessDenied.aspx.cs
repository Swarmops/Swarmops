using System;

public partial class Security_AccessDenied : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle = Resources.Pages.Security.AccessDenied_PageTitle;
        PageIcon = "iconshock-disconnect";

        this.LabelAccessDeniedHeader.Text = Resources.Pages.Security.AccessDenied_Header;
        InfoBoxLiteral = Resources.Pages.Security.AccessDenied_Info;
        this.LiteralAccessDeniedRant.Text = String.Format(Resources.Pages.Security.AccessDenied_Rant,
            CurrentOrganization.Name);
    }
}