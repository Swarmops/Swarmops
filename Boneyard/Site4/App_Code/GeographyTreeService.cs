using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Services ;
using Activizr.Logic.Structure;
using Telerik.Web.UI;
using System.Web.Script.Services;

/// <summary>
/// Summary description for GeographyTreeService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class GeographyTreeService : System.Web.Services.WebService
{

    public GeographyTreeService ()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public RadTreeNodeData[] GetNodes(RadTreeNodeData node, object context)
    {
        List<RadTreeNodeData> nodes = new List<RadTreeNodeData>();
        int parentId = Geography.Root.Identity;
        int.TryParse(node.Value, out parentId);
        Geographies geos = Geography.FromIdentity(parentId).Children;

        foreach (Geography geo in geos)
        {
            RadTreeNodeData nodeData = new RadTreeNodeData();
            nodeData.Text = geo.Name;
            nodeData.Value = geo.Identity.ToString();
            Geographies geos2 = Geography.FromIdentity(geo.Identity ).Children;
            if (geos2.Count > 0)
                nodeData.ExpandMode = TreeNodeExpandMode.WebService;

            nodes.Add(nodeData);
        }
        return nodes.ToArray();
    }
}
