using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Swarmops.Logic.Support.BackendServices
{
    [Serializable]
    public class RasterizeDocumentHiresOrder: BackendServiceOrderBase<RasterizeDocumentHiresOrder>, IBackendServiceOrderBase
    {
        [Obsolete("Do not call the parameterless public ctor directly. It is needed for serialization.", true)]
        public RasterizeDocumentHiresOrder()
        { }

        public RasterizeDocumentHiresOrder(Document document)
        {
            this.DocumentId = document.Identity;
        }

        public override void Run()
        {
            HasWorkerThread = true;
            WorkerThread = new Thread(LongRun);
            WorkerThread.Start(this);
        }

        private static void LongRun(object orderObject)
        {
            RasterizeDocumentHiresOrder order = null;
            try
            {
                order = (RasterizeDocumentHiresOrder)orderObject;
                Document document = Document.FromIdentity(order.DocumentId);
                PdfProcessor.Rerasterize((Document)document, PdfProcessor.PdfProcessorOptions.HighQuality | PdfProcessor.PdfProcessorOptions.ForceOrphans);
                order.Close();
            }
            catch (Exception exception)
            {
                order?.ThrewException(exception);
            }

            if (order != null)
            {
                order.HasTerminated = true;
            }
        }

        public override void Terminate()
        {
            
        }
        public int DocumentId { get; set; }
    }
}
