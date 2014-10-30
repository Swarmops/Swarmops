using System;

public partial class Security_AccessDenied : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageTitle = Resources.Pages_Security.AccessDenied_PageTitle;
        this.PageIcon = "iconshock-disconnect";

        this.LabelAccessDeniedHeader.Text = Resources.Pages_Security.AccessDenied_Header;
        this.InfoBoxLiteral = Resources.Pages_Security.AccessDenied_Info;
        this.LiteralAccessDeniedRant.Text = String.Format(Resources.Pages_Security.AccessDenied_Rant, this.CurrentOrganization.Name);
    }
}