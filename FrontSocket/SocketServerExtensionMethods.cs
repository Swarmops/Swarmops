using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Swarmops.Frontend.Socket
{
    public static class SocketServerExtensionMethods
    {
        static SocketServerExtensionMethods()
        {
            _sessionAuthorityLookup = new Dictionary<string, Authority>();
            _sessionLookup = new Dictionary<string, IWebSocketSession>();
        }

        public static void BroadcastToOrganization(this WebSocketSharp.Server.WebSocketServiceManager extendedObject,
            Organization organization, JObject jsonMessage)
        {
            lock (_sessionLookup)
            {
                foreach (string key in _sessionAuthorityLookup.Keys)
                {
                    IWebSocketSession session = _sessionLookup[key];
                    if (session.State == WebSocketState.Open) // protect against open/close race condition
                    {
                        if (_sessionAuthorityLookup.ContainsKey(key) &&
                            _sessionAuthorityLookup[key].Organization.Identity == organization.Identity)
                        {
                            session.Context.WebSocket.Send(jsonMessage.ToString());
                        }
                    }
                }
            }
        }

        public static void RegisterSession(this WebSocketSessionManager extendedObject, IWebSocketSession session, Authority authority)
        {
            lock (_sessionLookup)
            {
                _sessionLookup[session.ID] = session;
                _sessionAuthorityLookup[session.ID] = authority;
            }
        }

        public static void UnregisterSession(this WebSocketSessionManager extendedObject, IWebSocketSession session)
        {
            lock (_sessionLookup)
            {
                string id = session.ID;
                if (_sessionLookup.ContainsKey(id))
                {
                    _sessionLookup.Remove(id);
                }
                if (_sessionAuthorityLookup.ContainsKey(id))
                {
                    _sessionLookup.Remove(id);
                }
            }
        }

        private static Dictionary<string, Authority> _sessionAuthorityLookup;
        private static Dictionary<string, IWebSocketSession> _sessionLookup;
    }
}
