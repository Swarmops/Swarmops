using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Controls.v5.Base
{
    public partial class ProgressBarFake : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Header = Resources.Global.Global_ProcessingFile + "...";
        }

        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                this.LabelProcessingHeader.Text = value;
            }
        }

        private string _header;
    }
}