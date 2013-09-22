using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Controls.Base
{

    public partial class FileUpload : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Guid = new Guid();

                if (Filter == UploadFilter.Unknown) // if not set in control
                {
                    Filter = UploadFilter.NoFilter;
                }
            }
        }

        public Guid Guid;
        public UploadFilter Filter;


        public enum UploadFilter
        {
            Unknown = 0,
            /// <summary>
            /// Do not apply any upload filter. Accept any file.
            /// </summary>
            NoFilter,
            /// <summary>
            /// Only accept files that successfully load as images.
            /// </summary>
            ImagesOnly,
            /// <summary>
            /// Only accept files that successfully load as PDFs.
            /// </summary>
            PdfOnly,
            /// <summary>
            /// Accept either images or PDFs, but none other.
            /// </summary>
            ImagesOrPdf
        }
    }
}