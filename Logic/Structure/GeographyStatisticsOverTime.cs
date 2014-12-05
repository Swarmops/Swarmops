using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Swarmops.Logic.Structure
{
    internal class GeographyStatisticsOverTime : List<GeographyStatistics>
    {
        public static GeographyStatisticsOverTime FromXml (string xml)
        {
            XmlSerializer serializer = new XmlSerializer (typeof (GeographyStatisticsOverTime));

            MemoryStream stream = new MemoryStream();
            byte[] xmlBytes = Encoding.Default.GetBytes (xml);
            stream.Write (xmlBytes, 0, xmlBytes.Length);

            stream.Position = 0;
            GeographyStatisticsOverTime result = (GeographyStatisticsOverTime) serializer.Deserialize (stream);
            stream.Close();

            return result;
        }


        public string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer (typeof (GeographyStatisticsOverTime));

            MemoryStream stream = new MemoryStream();
            serializer.Serialize (stream, this);

            byte[] xmlBytes = stream.GetBuffer();
            return Encoding.Default.GetString (xmlBytes);
        }
    }
}