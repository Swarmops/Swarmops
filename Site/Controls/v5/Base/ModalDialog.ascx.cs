using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Swarmops.Frontend.Controls.Base
{
    public partial class ModalDialog : ControlV5Base, INamingContainer
    {
        protected void Page_Init()
        {
            if (this.DialogCode != null)
            {
                ModalDialog dialog = new ModalDialog();
                this.DialogCode.InstantiateIn (dialog);
                this.PlaceHolderDialog.Controls.Add (dialog);
            }
        }

        protected void Page_Load (object sender, EventArgs e)
        {

        }

        [TemplateInstance(TemplateInstance.Single)]
        [TemplateContainer(typeof(ModalDialog))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate DialogCode { get; set; }

        public string OnClientClose { get; set; }
    }
}