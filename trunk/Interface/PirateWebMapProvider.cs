using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;


namespace Swarmops.Interface
{
	public class PirateWebMapProvider: XmlSiteMapProvider
	{
		public PirateWebMapProvider()
		{
			NameValueCollection parameters = new NameValueCollection();
#if __MonoCS__
			parameters.Add("siteMapFile", HttpContext.Current.Server.MapPath("~/PirateWeb.sitemap"));
#else
			parameters.Add("siteMapFile", "~/PirateWeb.sitemap");
#endif
			base.Initialize("PirateWebMapProvider", parameters);
		}

		public override SiteMapNode FindSiteMapNode(string rawUrl)
		{
			return base.FindSiteMapNode(rawUrl);
		}

		public override SiteMapNodeCollection GetChildNodes(SiteMapNode node)
		{
			SiteMapNodeCollection baseCollection = base.GetChildNodes(node);

			SiteMapNodeCollection newCollection = new SiteMapNodeCollection();

			int count = baseCollection.Count;
			for (int index = 0; index < count; index++)
			{
				SiteMapNode oldNode = baseCollection[index];

				// TODO: Filter on oldNode.Roles here

				SiteMapNode newNode = new SiteMapNode(oldNode.Provider, oldNode.Key, oldNode.Url, Localization.LocalizationManager.GetLocalString (oldNode.Description, oldNode.Title), oldNode.Description);
				newCollection.Add(newNode);
			}

            return baseCollection ;
            //return newCollection;
        }

		public override SiteMapNode GetParentNode(SiteMapNode node)
		{
			return base.GetParentNode(node);
		}

		protected override SiteMapNode GetRootNodeCore()
		{
            //return LocalizeNode (base.GetRootNodeCore());
            return base.GetRootNodeCore();
        }

		private SiteMapNode LocalizeNode(SiteMapNode node)
		{
			if (node == null)
			{
				return null;
			}
            

			return new SiteMapNode(node.Provider, node.Key, node.Url, Localization.LocalizationManager.GetLocalString(node.Description, node.Title), node.Description);
		}
	}
}
