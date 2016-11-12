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

using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Activizr.Database;
using Activizr.Basic.Types;
using Activizr.Basic.Enums;

public partial class Pages_v4_CheckActivism : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        if (!_authority.HasPermission(Permission.CanValidateActivistEmail, Organization.PPSEid, -1, Authorization.Flag.AnyGeographyExactOrganization))
            Master.CurrentPageProhibited = true;

        if (IsPostBack && TextBox1.Text.Trim() != "")
        {
            int noOfMemberActivist = 0;
            int noOfNonMemberActivist = 0;
            int noMemberships = 0;
            BasicPerson[] persons = PirateDb.GetDatabase().GetPeopleFromEmail(TextBox1.Text.Trim());
            foreach (BasicPerson bp in persons)
            {
                Person p = Person.FromIdentity(bp.Identity);
                Memberships msList = p.GetMemberships();
                if (msList.Count > 0)
                {
                    ++noMemberships;
                    if ((p.IsActivist))
                        ++noOfMemberActivist;
                }
                else
                {
                    if ((p.IsActivist))
                        ++noOfNonMemberActivist; ;
                }
            }
            string format = litResultTemplate.Text;
            litResult.Text = string.Format(format,
                                 noMemberships.ToString(),
                                 noOfMemberActivist.ToString(),
                                 noOfNonMemberActivist.ToString());
        }

        this.Form.DefaultButton = Button1.UniqueID;
    }
}



