using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Swarmops.Logic;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;
using Swarmops.Utility.BotCode;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Swarmops.Backend.SocketServices
{
    internal class BackendServices: WebSocketBehavior
    {
        public BackendServices()
        {
            base.IgnoreExtensions = true; // Needed to suppress a Deflate which doesn't work
        }

        protected override void OnMessage(WebSocketSharp.MessageEventArgs e)
        {
            // Basically just echo whatever's sent here

            JObject json = JObject.Parse(e.Data);
            string serverRequest = (string)json["BackendRequest"];

            if (string.IsNullOrEmpty(serverRequest))
            {
                Console.WriteLine(" - echoing a message to all sessions");
                // Not a server request, so just echo the contents to everybody
                Sessions.Broadcast(e.Data);
            }

            Console.WriteLine(" - handling a socket message:" + serverRequest);

            switch (serverRequest)
            {
                case "UpdateQueueCounts":
                    //Sessions.Broadcast(Program.GetQueueInfoJson());
                    break;
                case "AddBitcoinAddress":
                    BackendLoop.AddBitcoinAddress((string) json["Address"]);
                    break;
                case "Metapackage":
                    BackendLoop.ProcessMetapackage(SocketMessage.FromXml((string) json["XmlData"]));
                    break;
                case "ConvertPdfHires":
                    // Convert with high quality -- done on backend with lower priority than the immediate frontend conversion
                    PdfProcessor.Rerasterize(Document.FromIdentity((int) json["DocumentId"]), PdfProcessor.PdfProcessorOptions.HighQuality);
                    break;
                default:
                    // do nothing;
                    break;
            }
        }

        protected override void OnOpen()
        {
            string authBase64 = Context.QueryString["Auth"];
            Console.WriteLine(" -- Authbase64 is " + authBase64);

            //authBase64 = Uri.UnescapeDataString(authBase64); // Defensive programming - % sign does not exist in base64 so this won't ever collapse a working encoding

            _systemAuthority = Authority.IsSystemAuthorityTokenValid(authBase64);

            if (!_systemAuthority)
            {
                throw new UnauthorizedAccessException("System access token invalid");
            }

            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);

            Sessions.Broadcast("{\"messageType\":\"EditorCount\"," + String.Format("\"editorCount\":\"{0}\"", Sessions.Count) + '}');
        }

        private bool _systemAuthority = false;
    }
}

