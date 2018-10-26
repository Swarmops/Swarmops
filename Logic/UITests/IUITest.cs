using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.UITests
{
    public interface IUITest
    {
        string Id { get; }
        string Name { get; }
        void ExecuteServerSide();
        string JavaScriptClientCode { get; }
    }

    public interface IUITestGroup
    {
        string GroupId { get; }
        string GroupName { get; } 
        string JavaScriptClientCodeDocReady { get; }
        void InitializeServerSide();
        IUITest[] Tests { get; }
    }
}
