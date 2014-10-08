using System;
using System.IO;
using System.Web.UI;
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
            output.Write("<li class=\"{0}\">", menuItem.Type.ToString());
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

            if (menuItem.Type == MenuItemType.BuildNumber)
            {
                localizedText = GetBuildIdentity();
            }

            string iconSize = "40px";

            if (File.Exists(Server.MapPath("~/Images/PageIcons/" + menuItem.ImageUrl + "-20px.png")))
            {
                iconSize = "20px";
            }

            switch (menuItem.Type)
            {
                // TODO: More types here, and check with the CSS. Some work to get good looking

                    // MEH forcing build

                case MenuItemType.Link:
                    output.Write("<a href=\"{1}\"><img src=\"/Images/PageIcons/{0}-{3}.png\"  height=\"20\" width=\"20\"  />{2}</a>",
                        menuItem.ImageUrl, menuItem.NavigateUrl, localizedText, iconSize);
                    break;
                case MenuItemType.Disabled:
                case MenuItemType.BuildNumber:
                    string imageUrl = "/Images/PageIcons/" + menuItem.ImageUrl + "-" + iconSize + ".png";
                    if (String.IsNullOrEmpty(menuItem.ImageUrl))
                    {
                        imageUrl = "/Images/PageIcons/transparency-16px.png"; // doesn't matter in slightest if browser resizes to 20px
                    }
                    output.Write("<a href='#disabled'><img src=\"{0}\" height=\"20\" width=\"20\" />{1}</a>", imageUrl, localizedText);
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