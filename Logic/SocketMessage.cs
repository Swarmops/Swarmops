using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Support;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using Swarmops.Logic.Security;
using WebSocketSharp;


namespace Swarmops.Logic
{
    [Serializable]
    public class SocketMessage : SocketMessageBase<SocketMessage>
    {
        
    }


    [Serializable]
    public class SocketMessageBase<T>
    {
        public static T FromXml(string xml) // 'T' might not work here, like it didn't in ToXml()
        {
            // Compensate for stupid Mono encoding bugs

            if (xml.StartsWith("?"))
            {
                xml = xml.Substring(1);
            }

            xml = xml.Replace("&#x0;", "");
            xml = xml.Replace("\x00", "");

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            MemoryStream stream = new MemoryStream();
            byte[] xmlBytes = Encoding.UTF8.GetBytes(xml);
            stream.Write(xmlBytes, 0, xmlBytes.Length);

            stream.Position = 0;
            T result = (T)serializer.Deserialize(stream);
            stream.Close();

            return result;
        }
        public virtual string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer(GetType());

            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, this);

            byte[] xmlBytes = stream.GetBuffer();
            string xml = Encoding.UTF8.GetString(xmlBytes);

            xml = xml.Replace("&#x0;", "");
            xml = xml.Replace("\x00", "");

            return xml;
        }

        public void SendUpstream()
        {
            if (SupportFunctions.OperatingTopology != OperatingTopology.FrontendWeb)
            {
                throw new InvalidOperationException("From web only");    
            }


            using (
                WebSocket socket =
                    new WebSocket("ws://localhost:" + SystemSettings.WebsocketPortFrontend + "/Front?Auth=" +
                                  Uri.EscapeDataString(Authority.GetSystemAuthorityToken("Financial"))))
            {
                socket.Connect();

                JObject data = new JObject();
                data["ServerRequest"] = "Metapackage";
                data["XmlData"] = this.ToXml();
                socket.Send(data.ToString());
                socket.Ping(); // wait a little little while for send to work
                socket.Close();
            }
        }


        public string MessageType { get; set; }
        public int OrganizationId { get; set; }
        public int FinancialTransactionId { get; set; }
        public int FinancialAccountId { get; set; }
        public int GeographyId { get; set; }
        public int PersonId { get; set; }
        public string XmlData { get; set; }
    }

}
