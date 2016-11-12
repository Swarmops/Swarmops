using System;
using Resources;
using Swarmops.Logic.Structure;

namespace Swarmops.Controls.Base
{
    public partial class ComboGeographies : ControlV5Base
    {
        public string OnClientLoaded { get; set; }
        public string OnClientSelect { get; set; }
        protected int RootGeographyId { get; private set; }
        protected int ParentGeographyId { get; private set; }
        protected string RootGeographyName { get; private set; }

        public Geography RootGeography
        {
            set
            {
                ParentGeographyId = value.ParentIdentity;
                RootGeographyId = value.Identity;
                RootGeographyName = value.Localized;
            }
        }

        protected void Page_Init (object sender, EventArgs e)
        {
            ((PageV5Base) this.Page).RegisterControl (EasyUIControl.Tree);
        }

        protected void Page_Load (object sender, EventArgs e)
        {
            RootGeography = Geography.Root; // set to "world" unless manually set to something else
        }
    }
}