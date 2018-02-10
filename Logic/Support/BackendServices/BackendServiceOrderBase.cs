using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Swarmops.Common.Interfaces;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;


namespace Swarmops.Logic.Support.BackendServices
{
    // This is the roughly the same base code as the LogEntryBase, plus some more processing things

    [Serializable]
    public abstract class BackendServiceOrderBase<T> : IXmlPayload, IBackendServiceOrderBase
    {
        public virtual string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());

            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, this);

            byte[] xmlBytes = stream.GetBuffer();
            string xml = Encoding.UTF8.GetString(xmlBytes);

            xml = xml.Replace("&#x0;", "");
            xml = xml.Replace("\x00", "");

            return xml;
        }

        public static T FromXml(string xml) // 'T' might not work here, like it didn't in ToXml()
        {
            // Compensate for stupid Mono encoding bugs

            if (xml.StartsWith("?"))
            {
                xml = xml.Substring(1);
            }

            xml = xml.Replace("&#x0;", "");
            xml = xml.Replace("\x00", "");

            XmlSerializer serializer = new XmlSerializer(typeof (T));

            MemoryStream stream = new MemoryStream();
            byte[] xmlBytes = Encoding.UTF8.GetBytes(xml);
            stream.Write(xmlBytes, 0, xmlBytes.Length);

            stream.Position = 0;
            T result = (T) serializer.Deserialize(stream);
            stream.Close();

            return result;
        }

        /// <summary>
        /// This function gets called by the backend for the class to carry out its mission. Override it to
        /// fill it with purpose. You SHOULD return within 50 milliseconds. Start a new thread if you need
        /// longer processing, and if so, set HasWorkerThread to true and point WorkerThread at your
        /// thread (so it can be called to WaitForExit()).
        /// </summary>
        public virtual void Run()
        {
            throw new NotImplementedException("The base Run() has no implementation. Derive and declare 'override'!");
        }

        /// <summary>
        /// If you are deriving this class into something that spawns a worker thread, then that derived
        /// class MUST have the worker thread terminate within 1.00 seconds from when Terminate is called.
        /// </summary>
        public virtual void Terminate()
        {
            throw new NotImplementedException("The base Terminate() has no implementation. Derive and declare 'override'!");
        }

        /// <summary>
        /// This function MUST be called when the task has been completed, either on completion of Run()
        /// if you take a short amount of time, or at the end of your worker thread.
        /// </summary>
        public void Close()
        {
            SwarmDb.GetDatabaseForWriting().SetBackendServiceOrderClosed(this.ServiceOrderIdentity);
        }

        public void ThrewException(Exception exception)
        {
            SwarmDb.GetDatabaseForWriting().SetBackendServiceOrderException(this.ServiceOrderIdentity, exception);
        }

        /// <summary>
        /// This creates the order and serializes it into the database for execution.
        /// </summary>
        public virtual void Create(Organization organization = null, Person person = null)
        {
            string executingAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            string className = this.GetType().FullName;
            string serviceClassName = BackendServiceClass = executingAssemblyName + "," + className;
            this.CreatedDateTime = DateTime.UtcNow;

            // Create the service order through reflection and XML serialization

            this.ServiceOrderIdentity = SwarmDb.GetDatabaseForWriting()
                .CreateBackendServiceOrder(serviceClassName, this.ToXml(),
                    organization?.Identity ?? 0, person?.Identity ?? 0);
        }

        public DateTime CreatedDateTime { get; set; }
        public string BackendServiceClass { get; set; }

        [XmlIgnore]
        public int ServiceOrderIdentity { get; set; }

        [XmlIgnore]
        public bool HasTerminated { get; set; }

        [XmlIgnore]
        public Organization Organization { get; set; }
        [XmlIgnore]
        public Person Person { get; set; }

        [XmlIgnore]
        public bool HasWorkerThread { get; set; }

        [XmlIgnore]
        public Thread WorkerThread { get; set; }

    }
}

