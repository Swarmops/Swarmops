using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Activizr.Logic.Structure
{
    internal class GeographyStatisticsOverTime : List<GeographyStatistics>
    {
        public static GeographyStatisticsOverTime FromXml (string xml)
        {
            var serializer = new XmlSerializer(typeof (GeographyStatisticsOverTime));

            var stream = new MemoryStream();
            byte[] xmlBytes = Encoding.Default.GetBytes(xml);
            stream.Write(xmlBytes, 0, xmlBytes.Length);

            stream.Position = 0;
            var result = (GeographyStatisticsOverTime) serializer.Deserialize(stream);
            stream.Close();

            return result;
        }


        public string ToXml()
        {
            var serializer = new XmlSerializer(typeof (GeographyStatisticsOverTime));

            var stream = new MemoryStream();
            serializer.Serialize(stream, this);

            byte[] xmlBytes = stream.GetBuffer();
            return Encoding.Default.GetString(xmlBytes);
        }
    }
}