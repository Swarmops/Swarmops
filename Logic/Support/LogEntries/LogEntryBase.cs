using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class LogEntryBase<T>: IXmlPayload
    {
        public virtual string ToXml()
        {
            var serializer = new XmlSerializer(this.GetType());

            var stream = new MemoryStream();
            serializer.Serialize(stream, this);

            byte[] xmlBytes = stream.GetBuffer();
            return Encoding.UTF8.GetString(xmlBytes);
        }

        public static T FromXml (string xml)  // 'T' might not work here, like it didn't in ToXml()
        {
            // Compensate for stupid Mono encoding bugs

            if (xml.StartsWith("?"))
            {
                xml = xml.Substring(1);
            }

            xml = xml.Replace("&#x0;", "");
            xml = xml.Replace("\x00", "");

            var serializer = new XmlSerializer(typeof(T));

            var stream = new MemoryStream();
            byte[] xmlBytes = Encoding.UTF8.GetBytes(xml);
            stream.Write(xmlBytes, 0, xmlBytes.Length);

            stream.Position = 0;
            var result = (T)serializer.Deserialize(stream);
            stream.Close();

            return result;
        }
    }
}
