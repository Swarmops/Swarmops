using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Logic;
using Swarmops.Logic.Support;
using Swarmops.Logic.Security;
using System.Globalization;
using System.Web.Services;
using System.Text.RegularExpressions;
using Swarmops.Frontend.Controls.v5.Base;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Admin
{
    public partial class OrganizationStaffing : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // HACK: The organization part must be removed once proper access control is in place
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.System, AccessType.Write);
            this.PageTitle = Resources.Pages.Admin.SystemSettings_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Admin.SystemSettings_Info;

            if (!Page.IsPostBack)
            {
                Localize();
            }

            CheckPositionsPopulated();

            RegisterControl (EasyUIControl.Tabs);
            RegisterControl (IncludedControl.JsonParameters | IncludedControl.SwitchButton);
        }

        private void CheckPositionsPopulated()
        {
            Positions orgPositions = Positions.ForOrganization(CurrentOrganization);

            if (orgPositions.Count == 0)
            {
                // not initalized. Initialize.

                Positions.CreateOrganizationDefaultPositions (CurrentOrganization);
            }


        }

        private static string FormatSmtpAccessString (string user, string pass, string host, int port)
        {
            string result = host;
            if (port != 25)
            {
                result += ":" + port.ToString(CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(pass))
            {
                user += ":" + pass;
            }

            if (!string.IsNullOrEmpty(user))
            {
                result = user + "@" + result;
            }

            return result;
        }

        private void Localize()
        {
            this.LabelHeaderStrategic.Text = String.Format(Resources.Pages.Admin.OrgStaffing_Header_StrategicPositions, CurrentOrganization.Name);
            this.LabelHeaderExecutive.Text = String.Format(Resources.Pages.Admin.OrgStaffing_Header_ExecutivePositions, CurrentOrganization.Name);
            this.LabelHeaderGeographicDefault.Text = Resources.Pages.Admin.OrgStaffing_Header_LocalDefaultPositions;
            this.LabelHeaderVolunteers.Text = Resources.Pages.Admin.OrgStaffing_Header_WaitingVolunteers;
            this.LabelHeaderPayroll.Text = Resources.Pages.Admin.OrgStaffing_Header_Payroll;
        }



    }

}