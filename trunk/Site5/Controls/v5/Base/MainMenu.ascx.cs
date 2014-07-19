using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Controls.Base
{
    public partial class MainMenu : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public MainMenuItem[] MainMenuData { get; set; }
    }

    [Serializable]
    public class MainMenuItem
    {
        public string ResourceString;
        public string IconUrl;
        public string NavigateUrl;
        public int UserLevel;
        public string Permission;
        public bool IsSeparator;
        public MainMenuItem[] Children;
    }
}