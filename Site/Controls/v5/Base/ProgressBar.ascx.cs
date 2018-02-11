using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Controls.v5.Base
{
    public partial class ProgressBar : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            _guid = System.Guid.NewGuid().ToString();
            this.Header = Resources.Global.Global_ProcessingFile;
        }

        protected string GuidToken
        {
            get { return _guid.Replace("-", "_"); } // creates a valid JS token
        }

        public string Guid
        {
            get { return _guid; }
            set
            {
                if (!value.Contains("-") || value.Length != 36)
                {
                    throw new FormatException("This doesn't look like a valid GUID: " + value);
                }

                _guid = value;
            }
        }

        public string OnClientProgressHalfwayCallback { get; set; }
        public string OnClientProgressCompleteCallback { get; set; }

        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                this.LabelProcessingHeader.Text = value;
            }
        }

        private string _guid;
        private string _header;
    }
}