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
    internal class MasterServices: WebSocketBehavior
    {
        public MasterServices()
        {
            base.IgnoreExtensions = true; // Necessary to suppress a Deflate exception that kills server otherwise
        }

        protected override void OnMessage(WebSocketSharp.MessageEventArgs e)
        {
            // Basically just echo whatever's sent here

            JObject json = JObject.Parse (e.Data);
            string serverRequest = (string) json["serverRequest"];

            if (string.IsNullOrEmpty (serverRequest))
            {
                // Not a server request, so just echo the contents to everybody
                Sessions.Broadcast(e.Data);
            }

            switch (serverRequest)
            {
                case "UpdateQueueCounts":
                    Sessions.Broadcast (FrontendLoop.GetQueueInfoJson());
                    break;
                case "Ping":
                    Sessions.Broadcast("{\"messageType\":\"Heartbeat\"}");
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
            authBase64 = Uri.UnescapeDataString (authBase64); // Defensive programming - % sign does not exist in base64 so this won't ever collapse a working encoding

            _authority = Authority.FromEncryptedXml (authBase64);

            Console.WriteLine(" - - authenticated: " + _authority.Person.Canonical);

            base.OnOpen();

            if (Context.QueryString["Notify"] != "false")
            {
                Sessions.Broadcast ("{\"messageType\":\"EditorCount\"," +
                                    String.Format("\"editorCount\":\"{0}\"", Sessions.ActiveIDs.ToArray().Length) + '}');
                Send(FrontendLoop.GetQueueInfoJson());
            }
        }

        protected override void OnClose (CloseEventArgs e)
        {
            base.OnClose (e);

            Sessions.Broadcast("{\"messageType\":\"EditorCount\"," + String.Format("\"editorCount\":\"{0}\"", Sessions.ActiveIDs.ToArray().Length) + '}');
        }

        private Authority _authority = null;
    }
}
