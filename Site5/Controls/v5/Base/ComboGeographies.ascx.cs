using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Structure;

namespace Swarmops.Controls.Base
{
    public partial class ComboGeographies : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RootGeography = Geography.Root; // set to "world" unless manually set to something else
        }

        public string OnClientLoaded { get; set; }
        public string OnClientSelect { get; set; }
        protected int RootGeographyId { get; private set; }
        protected string RootGeographyName { get; private set; }

        public Geography RootGeography 
        { 
            set 
            { 
                this.RootGeographyId = value.Identity;
                RootGeographyName = value.Name; 
            } 
        }
    }
}