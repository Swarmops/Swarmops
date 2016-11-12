using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Swarmops.Logic.Security;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Admin.Hacks
{
    public partial class Temp_SetOrganizationAdminAccess : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Write);
            // As good as any access aspect
            PageIcon = "iconshock-battery-drill";
            InfoBoxLiteral =
                "This is a <strong>temporary</strong> access mechanism to get Swarmops usage off the ground. Real, fine-grained, responsibility-based access control per organizational role is scheduled to be implemented by the <em>Swarmops Orange</em> release (Jun 30, 2015).";
            PageTitle = "Set Admin Access (Temporary)";

            if (!Page.IsPostBack)
            {
                this.TextPeopleWriteAccessList.Text = CurrentOrganization.Parameters.TemporaryAccessListWrite;
                this.TextPeopleReadAccessList.Text = CurrentOrganization.Parameters.TemporaryAccessListRead;

                InterpretPersonIds (this.TextPeopleWriteAccessList, this.LabelPeopleWriteAccessList);
                InterpretPersonIds (this.TextPeopleReadAccessList, this.LabelPeopleReadAccessList);
            }
        }

        protected void ButtonSave_Click (object sender, EventArgs e)
        {
            // Rough stuff. This will throw up on the user if the input is bad. That's kind of intentional as it's before the save and this is a very temporary page.

            InterpretPersonIds (this.TextPeopleWriteAccessList, this.LabelPeopleWriteAccessList);
            InterpretPersonIds (this.TextPeopleReadAccessList, this.LabelPeopleReadAccessList);

            CurrentOrganization.Parameters.TemporaryAccessListWrite = this.TextPeopleWriteAccessList.Text;
            CurrentOrganization.Parameters.TemporaryAccessListRead = this.TextPeopleReadAccessList.Text;
        }

        private void InterpretPersonIds (TextBox textPersonIds, Label labelInterpretedPeople)
        {
            if (textPersonIds.Text.Trim().Length == 0)
            {
                labelInterpretedPeople.Text = "(nobody)";
                return;
            }

            List<string> resultingPeople = new List<string>();
            string[] idStrings = textPersonIds.Text.Trim().Replace ("  ", " ").Split (' ');

            foreach (string idString in idStrings)
            {
                resultingPeople.Add (Person.FromIdentity (Int32.Parse (idString)).Canonical);
            }

            labelInterpretedPeople.Text = String.Join (", ", resultingPeople.ToArray());
        }
    }
}