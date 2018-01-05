using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.System;
using Swarmops.Common.Exceptions;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support.BackendServices;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    public class BackendServiceOrder: BasicBackendServiceOrder
    {
        private BackendServiceOrder(BasicBackendServiceOrder basic): base (basic)
        {
            // private ctor prevents involuntary construction
        }

        public static BackendServiceOrder FromBasic(BasicBackendServiceOrder basic)
        {
            return new BackendServiceOrder(basic);
        }

        public static BackendServiceOrder FromIdentity(int backendServiceOrderId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetBackendServiceOrder(backendServiceOrderId));
        }


        public void Execute()
        {
            // Instantiate the class

            string[] components = BackendServiceClassName.Split(',');
            // string assemblyName = components[0];  // TODO: For plugin use later
            string className = components[1];

            Assembly assembly = Assembly.GetExecutingAssembly(); // TODO: Make this actually select the named assembly

            Type serviceClassType = assembly.GetType(className);

            if (serviceClassType == null)
            {
                SwarmDb.GetDatabaseForWriting().SetBackendServiceOrderException(this.Identity, new ArgumentNullException("ClassName", "Class failed to instantiate -- service engine is null: " + className));
                return;
            }

            MethodInfo methodFromXml = serviceClassType.GetMethod("FromXml", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);

            if (methodFromXml == null)
            {
                SwarmDb.GetDatabaseForWriting().SetBackendServiceOrderException(this.Identity, new ArgumentNullException("MethodName", "Class failed to instantiate -- method search returned null"));
                return;
            }

            IBackendServiceOrderBase order = (IBackendServiceOrderBase) methodFromXml.Invoke(null, new object[] { this.OrderXml });
            order.ServiceOrderIdentity = this.Identity; // not part of serialization

            // Supply with organization and person, if we have them

            order.Person = this.PersonId == 0 ? null : Person.FromIdentity(this.PersonId);
            order.Organization = this.OrganizationId == 0 ? null : Organization.FromIdentity(this.OrganizationId);

            // Mark as started

            SwarmDb.GetDatabaseForWriting().SetBackendServiceOrderActive(this.Identity);

            // Run the Run() function

            try
            {
                Console.WriteLine(" - Executing BSO: " + className);

                order.Run();
            }
            catch (Exception exception)
            {
                if (!order.HasWorkerThread)
                {
                    order.HasTerminated = true;
                }
                order.Close();
                order.ThrewException(exception);
            }
            finally
            {
                if (!order.HasWorkerThread)
                {
                    try
                    {
                        order.HasTerminated = true;
                        order.Close();
                    }
                    catch (DatabaseConcurrencyException)
                    {
                        // Ignore if we try to close it twice in this particular failure scenario
                    }
                }
            }


            // Mark as closed, if it has not marked itself as forked and still working
            // TODO (in which case, do something else -- add to a list of still working tasks.)
        }
    }
}
