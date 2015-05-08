using System;
using System.IO;
using System.Xml.Serialization;

namespace Swarmops.Interface.Objects
{
    /// <summary>
    ///     Summary description for MainMenuItem
    /// </summary>
    [Serializable]
    public class MainMenuItem
    {
        public MainMenuItem()
        {
            Children = new MainMenuItem[0];
        }

        public static MainMenuItem Separator
        {
            get
            {
                MainMenuItem result = new MainMenuItem();
                result.Type = MenuItemType.Separator;

                return result;
            }
        }

        public string ResourceKey { get; set; }
        public int UserLevel { get; set; }
        public string Permissions { get; set; }
        public string ImageUrl { get; set; }
        public string NavigateUrl { get; set; }
        public MenuItemType Type { get; set; }

        // TODO: INSERTION POINT (for plugins)

        public MainMenuItem[] Children { get; set; }

        public static MainMenuItem Link(string image, string navUrl, string resourceKey, int userLevel,
            string permissions)
        {
            MainMenuItem result = new MainMenuItem
            {
                ImageUrl = image,
                NavigateUrl = navUrl,
                Permissions = permissions,
                ResourceKey = resourceKey,
                UserLevel = userLevel,
                Type = MenuItemType.Link
            };

            return result;
        }

        public static MainMenuItem Submenu(string resourceKey, int userLevel, string permissions)
        {
            MainMenuItem result = new MainMenuItem
            {
                Permissions = permissions,
                ResourceKey = resourceKey,
                UserLevel = userLevel,
                Type = MenuItemType.Submenu
            };

            return result;
        }

        public static MainMenuItem Disabled(string image, string resourceKey, int userLevel, string permissions)
        {
            MainMenuItem result = new MainMenuItem
            {
                ImageUrl = image,
                Permissions = permissions,
                ResourceKey = resourceKey,
                UserLevel = userLevel,
                Type = MenuItemType.Disabled
            };

            return result;
        }

        public static void PrimeXml(MainMenuItem[] items)
        {
            // This is only supposed to be used once, to prime the initial menu
            XmlSerializer serializer = new XmlSerializer(typeof (MainMenuItem[]));

            using (TextWriter writer = new StreamWriter("C:\\Windows\\Temp\\temp.xml"))
            {
                serializer.Serialize(writer, items);
            }
        }

        public static void PrimeNewMenuFile()
        {
            MainMenuItem todoItem = Disabled(string.Empty, "Placeholder_Todo", 1, string.Empty);
            MainMenuItem[] todoItemArray = {todoItem};

            MainMenuItem peopleItem = Submenu("People", 1, string.Empty);
            peopleItem.Children = todoItemArray;

            MainMenuItem commsItem = Submenu("Communications", 1, string.Empty);
            peopleItem.Children = todoItemArray;

            MainMenuItem claimRefundItem = Link("iconshock-moneyback",
                "/Pages/v5/Financial/FileExpenseClaim.aspx", "Financial_ClaimExpense", 2, String.Empty);
            MainMenuItem separatorItem = Separator;

            MainMenuItem[] financialChildren = {claimRefundItem, separatorItem};
            MainMenuItem financialItem = Submenu("Financial", 2, string.Empty);
            financialItem.Children = financialChildren;

            MainMenuItem[] protoMenu = {peopleItem, commsItem, financialItem};
            PrimeXml(protoMenu);
        }
    }

    public enum MenuItemType
    {
        Unknown = 0,

        /// <summary>
        ///     A normal link
        /// </summary>
        Link,

        /// <summary>
        ///     Something that looks like a menu item but is disabled
        /// </summary>
        Disabled,

        /// <summary>
        ///     A header (with or without image) that leads to more options as sub
        /// </summary>
        Submenu,

        /// <summary>
        ///     A thin line separating menu items
        /// </summary>
        Separator,

        /// <summary>
        ///     A major header (non-link) introducing new menu section
        /// </summary>
        Header,

        /// <summary>
        ///     Build number indicator
        /// </summary>
        BuildNumber,

        /// <summary>
        ///     SPECIAL CASE: Close Books menu item
        /// </summary>
        SpecialCloseBooks,

        /// <summary>
        ///     SPECIAL CASE: Self-signup menu item
        /// </summary>
        SpecialSelfSignup
    }
}