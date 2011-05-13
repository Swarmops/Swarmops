using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Services;
using Activizr.Logic.Structure;
using Telerik.Web.UI;
using System.Web.Script.Services;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;

/// <summary>
/// Summary description for OrgTreeService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class OrgTreeService : System.Web.Services.WebService
{
    private Authority authority = null;
    private Permission requiredPermission = Permission.Undefined;

    public OrgTreeService ()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public RadTreeNodeData[] GetNodes (RadTreeNodeData node, object context)
    {
        if (node.Attributes.ContainsKey("perm"))
        {
            int persID = int.Parse(node.Attributes["uid"].ToString());
            authority = Person.FromIdentity(persID).GetAuthority();
            PermissionSet ps = new PermissionSet(node.Attributes["perm"].ToString());
            requiredPermission = ps.permsList[0].perm;
        }


        List<RadTreeNodeData> nodes = new List<RadTreeNodeData>();
        int parentId = Organization.RootIdentity;
        int.TryParse(node.Value, out parentId);
        Organizations orgs = Organization.FromIdentity(parentId).Children;
        foreach (Organization org in orgs)
        {
            RadTreeNodeData nodeData = new RadTreeNodeData();
            nodeData.Text = org.Name;
            nodeData.Value = org.Identity.ToString();
            Organizations orgs2 = Organization.FromIdentity(org.Identity).Children;
            if (orgs2.Count > 0)
                nodeData.ExpandMode = TreeNodeExpandMode.WebService;

            SetAuthorityForNode(nodeData);

            nodes.Add(nodeData);
        }
        return nodes.ToArray();
    }

    private void SetAuthorityForNode (RadTreeNodeData nod)
    {
        if (authority != null && requiredPermission != Permission.Undefined)
        {
            nod.Attributes["uid"] = authority.PersonId.ToString();
            nod.Attributes["perm"] = requiredPermission.ToString();
            int org = int.Parse(nod.Value);
            if (!authority.HasPermission(requiredPermission, org, -1 , Authorization.Flag.Default ))
                nod.CssClass="nonAccessNode";
        }
    }

}
