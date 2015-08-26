using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class LogEntryBase<T> : IXmlPayload
    {
        public virtual string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer (GetType());

            MemoryStream stream = new MemoryStream();
            serializer.Serialize (stream, this);

            byte[] xmlBytes = stream.GetBuffer();
            string xml = Encoding.UTF8.GetString (xmlBytes);

            xml = xml.Replace("&#x0;", "");
            xml = xml.Replace("\x00", "");

            return xml;
        }

        public static T FromXml (string xml) // 'T' might not work here, like it didn't in ToXml()
        {
            // Compensate for stupid Mono encoding bugs

            if (xml.StartsWith ("?"))
            {
                xml = xml.Substring (1);
            }

            xml = xml.Replace ("&#x0;", "");
            xml = xml.Replace ("\x00", "");

            XmlSerializer serializer = new XmlSerializer (typeof (T));

            MemoryStream stream = new MemoryStream();
            byte[] xmlBytes = Encoding.UTF8.GetBytes (xml);
            stream.Write (xmlBytes, 0, xmlBytes.Length);

            stream.Position = 0;
            T result = (T) serializer.Deserialize (stream);
            stream.Close();

            return result;
        }
    }
}