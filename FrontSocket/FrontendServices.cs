using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Swarmops.Frontend.Socket;
using Swarmops.Logic.Security;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Swarmops.Frontend.Socket
{ 
    internal class FrontendServices: WebSocketBehavior
    {
        public FrontendServices()
        {
            base.IgnoreExtensions = true; // Necessary to suppress a Deflate exception that kills server otherwise
        }

        protected override void OnMessage(WebSocketSharp.MessageEventArgs e)
        {
            // Basically just echo whatever's sent here

            Console.WriteLine(" - a client says " + e.Data);

            JObject json = JObject.Parse (e.Data);
            string serverRequest = (string) json["ServerRequest"];

            if (string.IsNullOrEmpty (serverRequest))
            {
                // Not a server request, so just echo the contents to everybody
                Sessions.Broadcast(e.Data);
            }

            switch (serverRequest)
            {
                case "AddBitcoinAddress":
                    FrontendLoop.AddBitcoinAddress((string) json["Address"]);
                    break;
                case "Ping":
                    // TODO: Request heartbeat from backend
                    // Sessions.Broadcast("{\"messageType\":\"Heartbeat\"}");
                    break;
                default:
                    // do nothing;
                    break;
            }
        }

        protected override void OnOpen()
        {
            Console.WriteLine(" * Attempted socket connection");

            string authBase64 = Context.QueryString["Auth"];

            // TODO: Check for valid system authority token

            authBase64 = Uri.UnescapeDataString (authBase64); // Defensive programming - % sign does not exist in base64 so this won't ever collapse a working encoding

            _authority = Authority.FromEncryptedXml (authBase64);

            Console.WriteLine(" - - authenticated: " + _authority.Person.Canonical);

            base.OnOpen();
        }

        protected override void OnClose (CloseEventArgs e)
        {
            Console.WriteLine(" - client closed");
            base.OnClose (e);

            // Sessions.Broadcast("{\"messageType\":\"EditorCount\"," + String.Format("\"editorCount\":\"{0}\"", Sessions.ActiveIDs.ToArray().Length) + '}');
        }

        private Authority _authority = null;
    }
}
