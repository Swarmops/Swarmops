using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.UITests
{
    public class SocketTests: IUITestGroup
    {
        public string GroupId { get { return "Sockets"; } }

        public string GroupName { get { return "Sockets"; } }

        public void InitializeServerSide() { }

        public string JavaScriptClientCodeDocReady { get { return @"

            function pageOnHeartbeat(source) {  // called from Swarmops-v5.js script
                onUITestPassed('Sockets-Browser');
                if (source == 'Frontend') {
                    onUITestPassed('Sockets-Frontend');
                }
                else if (source == 'Backend') {
                    onUITestPassed('Sockets-Backend');
                }
            }

        "; } }

        public IUITest[] Tests
        {
            get
            {
                return new IUITest[ ] {
                    new TestSocketConnecting() /*,
                    new TestSocketEcho(),
                    new TestFrontendHeartbeatReceived(),
                    new TestBackendHeartbeatReceived()*/
                    };
            }
        }
    }

    public class TestSocketConnecting : IUITest
    {
        public string Id { get { return @"Browser";  } }

        public string Name { get { return @"Browser WebSocket connection to server"; } }

        public void ExecuteServerSide()
        {
            
        }

        public string JavaScriptClientCode { get { return string.Empty; } }
    }
}
