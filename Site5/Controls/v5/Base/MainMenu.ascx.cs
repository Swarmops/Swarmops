using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Swarmops.Interface.Objects;

// ReSharper disable once CheckNamespace
namespace Swarmops.Controls.Base
{
    public partial class MainMenu : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: Put a small cache on this

            // TODO: Put plugins on it

            XmlSerializer serializer = new XmlSerializer(typeof(MainMenuItem[]));

            using (TextReader reader = new StreamReader(Server.MapPath("~/MainMenu-v5.xml")))
            {
                this.MainMenuData = (MainMenuItem[]) serializer.Deserialize(reader);
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            output.Write("<ul id='MainMenuContainer' class='sf-menu'>");
            foreach (MainMenuItem menuItem in MainMenuData)
            {
                WriteMenuItem(menuItem, output);
            }
            output.Write("</ul>");
        }

        private void WriteMenuItem(MainMenuItem menuItem, HtmlTextWriter output)
        {
            output.Write("<li>");
            string localizedText = "RESOURCE NOT FOUND";

            if (!String.IsNullOrEmpty(menuItem.ResourceKey))
            {
                object resourceObject = GetGlobalResourceObject("Menu5", "Menu5_" + menuItem.ResourceKey);
                if (resourceObject != null)
                {
                    localizedText = resourceObject.ToString();
                }
            }
            localizedText = Server.HtmlEncode(localizedText); // muy importante

            switch (menuItem.Type)
            {
                // TODO: More types here, and check with the CSS. Some work to get good looking

                case MenuItemType.Link:
                    output.Write("<a href=\"{1}\"><img src=\"/Images/PageIcons/{0}-16px.png\" />{2}</a>",
                        menuItem.ImageUrl, menuItem.NavigateUrl, localizedText);
                    break;
                case MenuItemType.Disabled:
                    string imageUrl = "/Images/PageIcons/" + menuItem.ImageUrl + "-16px.png";
                    if (String.IsNullOrEmpty(menuItem.ImageUrl))
                    {
                        imageUrl = "/Images/PageIcons/transparency-16px.png";
                    }
                    output.Write("<a href='#disabled'><img src=\"{0}\" />{1}</a>", imageUrl, localizedText);
                    break;
                case MenuItemType.Submenu:
                    output.Write("<a href='#'>" + localizedText + "</a>");
                    break;
                case MenuItemType.Separator:
                    output.Write("&nbsp;<hr/>");
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (menuItem.Children.Length > 0)
            {
                output.Write("<ul>");
                foreach (MainMenuItem child in menuItem.Children)
                {
                    WriteMenuItem(child, output);
                }
                output.Write("</ul>");
            }

            output.Write("</li>");
        }

        public MainMenuItem[] MainMenuData { get; set; }
    }
}