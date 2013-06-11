using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Swarmops.Logic.Communications.Transmission
{
    [Serializable]
    public class PayloadBase<T>
    {
        public string ToXml()
        {
            var serializer = new XmlSerializer(typeof(T));

            var stream = new MemoryStream();
            serializer.Serialize(stream, this);

            byte[] xmlBytes = stream.GetBuffer();
            return Encoding.Default.GetString(xmlBytes);
        }

        static T FromXml (string xml)
        {
            var serializer = new XmlSerializer(typeof(T));

            var stream = new MemoryStream();
            byte[] xmlBytes = Encoding.Default.GetBytes(xml);
            stream.Write(xmlBytes, 0, xmlBytes.Length);

            stream.Position = 0;
            var result = (T)serializer.Deserialize(stream);
            stream.Close();

            return result;
        }
    }
}
