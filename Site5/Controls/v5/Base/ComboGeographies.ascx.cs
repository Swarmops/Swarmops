using System;
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

                if (value.Name.StartsWith("[LOC]"))
                {
                    RootGeographyName = Resources.GeographyNames.ResourceManager.GetString(value.Name.Substring(5));
                }
            } 
        }
    }
}