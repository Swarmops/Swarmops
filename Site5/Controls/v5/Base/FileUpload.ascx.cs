using System;

namespace Swarmops.Controls.Base
{
    public partial class FileUpload : ControlV5Base
    {
        public enum UploadFilter
        {
            Unknown = 0,

            /// <summary>
            ///     Do not apply any upload filter. Accept any file.
            /// </summary>
            NoFilter,

            /// <summary>
            ///     Only accept files that successfully load as images.
            /// </summary>
            ImagesOnly,

            /// <summary>
            ///     Only accept files that successfully load as PDFs.
            /// </summary>
            PdfOnly,

            /// <summary>
            ///     Accept either images or PDFs, but none other.
            /// </summary>
            ImagesOrPdf
        }

        public string GuidString;
        public UploadFilter Filter { get; set; }
        public int DisplayCount { get; set; }
        public bool HideTrigger { get; set; }
        public string ClientUploadCompleteCallback { get; set; }
        public string ClientUploadStartedCallback { get; set; }

        protected void Page_Init (object sender, EventArgs e)
        {
            // cause master to include necessary script
            ((PageV5Base)Page).RegisterControl(IncludedControl.FileUpload);
        }

        protected void Page_Load (object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Guid newGuid = Guid.NewGuid();
                this.HiddenGuid.Value = this.GuidString = newGuid.ToString();

                if (Filter == UploadFilter.Unknown) // if not set in control
                {
                    Filter = UploadFilter.NoFilter;
                }
                if (DisplayCount == 0) // if not set
                {
                    DisplayCount = 16;
                }
                if (ClientUploadCompleteCallback == null)
                {
                    ClientUploadCompleteCallback = string.Empty;
                }
                if (ClientUploadStartedCallback == null)
                {
                    ClientUploadStartedCallback = string.Empty;
                }
            }
            else
            {
                this.GuidString = this.HiddenGuid.Value;
            }
        }


        public void Reset()
        {
            Guid newGuid = Guid.NewGuid();
            this.HiddenGuid.Value = this.GuidString = newGuid.ToString();
        }
    }
}