using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Controls_v5_Swarm_TreePersonRoles : ControlV5Base
{
    protected void Page_Init (object sender, EventArgs e)
    {
        // Request control framework in Init - the Load is too late for child controls

        ((PageV5Base)this.Page).RegisterControl(EasyUIControl.Tree | EasyUIControl.DataGrid);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public RoleLevel Level { get; set; }
    public int OrganizationId { get; set; }
    public int GeographyId { get; set; }


    public enum RoleLevel
    {
        Unknown = 0,
        Systemwide,
        Organizationwide,
        SuborganizationwideDefault,
        OrganizationTop,
        SuborganizationTopDefault,
        GeographicDefault,
        Geographic
    }
}