using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Xml.Serialization;
using Swarmops.Frontend;
using Swarmops.Interface.Objects;
using Swarmops.Logic.Swarm;


// ReSharper disable once CheckNamespace

namespace Swarmops.Controls.Base
{
    public partial class MainMenu : ControlV5Base
    {
        public MainMenuItem[] MainMenuData { get; set; }

        private string _regularLocalized;
        private string _regularsLocalized;
        private string _regularshipLocalized;

        private static Dictionary<string, string> _imageSizeLookup = new Dictionary<string, string>();

        protected void Page_Load (object sender, EventArgs e)
        {
            ParticipantTitle regular = CurrentOrganization.RegularLabel;
            _regularLocalized = Participant.Localized (regular);
            _regularsLocalized = Participant.Localized (regular, TitleVariant.Plural);
            _regularshipLocalized = Participant.Localized (regular, TitleVariant.Ship);

            // TODO: Put a small cache on this

            // TODO: Put plugins on it

            XmlSerializer serializer = new XmlSerializer (typeof (MainMenuItem[]));

            string mainMenuFile = "~/MainMenu-v5.xml";

            if (CurrentUser.Identity == Person.OpenLedgersIdentity)
            {
                mainMenuFile = "~/MainMenu-v5-OpenLedgers.xml";
            }

            using (TextReader reader = new StreamReader (Server.MapPath (mainMenuFile)))
            {
                MainMenuData = (MainMenuItem[]) serializer.Deserialize (reader);
            }
        }

        protected override void Render (HtmlTextWriter output)
        {
            output.Write ("<ul id='MainMenuContainer' class='sf-menu'>");
            foreach (MainMenuItem menuItem in MainMenuData)
            {
                WriteMenuItem (menuItem, output);
            }
            output.Write ("</ul>");
        }

        private void WriteMenuItem (MainMenuItem menuItem, HtmlTextWriter output)
        {
            output.Write ("<li class=\"{0}\">", menuItem.Type);
            string localizedText = "RESOURCE NOT FOUND";
            string prettyNavUrl = menuItem.NavigateUrl ?? string.Empty;

            if (menuItem.Type == MenuItemType.SpecialSelfSignup)
            {
                if (string.IsNullOrEmpty (CurrentOrganization.VanityDomain))
                {
                    prettyNavUrl = "/Signup?OrganizationId=" + CurrentOrganization.Identity;
                }
                else
                {
                    prettyNavUrl = "//" + CurrentOrganization.VanityDomain + "/Signup";
                }
                menuItem.Type = MenuItemType.Link;  // once the link is set, change to type Link to go into normal logic
            }

            if (!Debugger.IsAttached)
            {
                prettyNavUrl = prettyNavUrl.Replace ("/Pages/v5/", "/").Replace (".aspx", "");
            }

            if (!String.IsNullOrEmpty (menuItem.ResourceKey))
            {
                object resourceObject = GetGlobalResourceObject ("Menu5", "Menu5_" + menuItem.ResourceKey);
                if (resourceObject != null)
                {
                    localizedText = resourceObject.ToString();
                }
            }
            localizedText = localizedText.Replace ("[Regular]", _regularLocalized);
            // TODO IF APPLICABLE - add other [Regulars] [Regularship] etc.
            localizedText = Server.HtmlEncode (localizedText); // muy importante
            string cssClass = String.Empty;

            if (menuItem.Type == MenuItemType.BuildNumber)
            {
                localizedText = Swarmops.Logic.Support.Formatting.SwarmopsVersion;
                cssClass = " dir=\"ltr\"";
            }

            string iconSize = string.Empty;

            if (menuItem.ImageUrl != null && _imageSizeLookup.ContainsKey (menuItem.ImageUrl))
            {
                iconSize = _imageSizeLookup[menuItem.ImageUrl]; // from cache
            }
            else if (menuItem.ImageUrl != null)
            {
                string[] iconSizePreferences = { "40", "96", "128", "20", "16" };

                foreach (string testSize in iconSizePreferences)
                {
                    if (
                        File.Exists (
                            Server.MapPath ("~/Images/PageIcons/" + menuItem.ImageUrl + "-" + testSize + "px.png")))
                    {
                        iconSize = testSize + "px";
                        _imageSizeLookup[menuItem.ImageUrl] = iconSize;
                        break; // break at first located preference
                    }
                }

                // Let it break if none is found, that'll generate a broken image and be visible immediately
            }

            switch (menuItem.Type)
            {
                    // TODO: More types here, and check with the CSS. Some work to get good looking

                case MenuItemType.Link:
                    if (!string.IsNullOrEmpty (menuItem.ImageUrl))
                    {
                        output.Write (
                            "<a href=\"{1}\"><img src=\"/Images/PageIcons/{0}-{3}.png\"  height=\"20\" width=\"20\"  />{2}</a>",
                            menuItem.ImageUrl, prettyNavUrl, localizedText, iconSize);
                    }
                    else
                    {
                        output.Write(
                            "<a href=\"{0}\">{1}</a>",
                            prettyNavUrl, localizedText);
                    }
                    break;
                case MenuItemType.Disabled:
                case MenuItemType.BuildNumber:
                    string imageUrl = "/Images/PageIcons/" + menuItem.ImageUrl + "-" + iconSize + ".png";
                    if (String.IsNullOrEmpty (menuItem.ImageUrl))
                    {
                        imageUrl = "/Images/PageIcons/transparency-16px.png";
                        // doesn't matter in slightest if browser resizes to 20px
                    }
                    output.Write ("<a href='#disabled'><img src=\"{0}\" height=\"20\" width=\"20\" /><span{2}>{1}</span></a>", imageUrl,
                        localizedText, cssClass);
                    break;
                case MenuItemType.Submenu:
                    if (string.IsNullOrEmpty (menuItem.ImageUrl))
                    {
                        output.Write ("<a href='#'>" + localizedText + "</a>");
                    }
                    else
                    {
                        output.Write(
                            "<a href=\"#\"><img src=\"/Images/PageIcons/{0}-{3}.png\"  height=\"20\" width=\"20\"  />{2}</a>",
                            menuItem.ImageUrl, prettyNavUrl, localizedText, iconSize);
                    }
                    break;
                case MenuItemType.Separator:
                    output.Write ("&nbsp;<hr/>");
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (menuItem.Children.Length > 0)
            {
                output.Write ("<ul>");
                foreach (MainMenuItem child in menuItem.Children)
                {
                    WriteMenuItem (child, output);
                }
                output.Write ("</ul>");
            }

            output.Write ("</li>");
        }
    }
}