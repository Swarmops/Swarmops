﻿using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Support
{
    [Serializable]
    public class PayloadBase<T> : IXmlPayload
    {
        public string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer (typeof (T));

            MemoryStream stream = new MemoryStream();
            serializer.Serialize (stream, this);

            byte[] xmlBytes = stream.GetBuffer();
            string xml = Encoding.UTF8.GetString (xmlBytes);

            xml = xml.Replace("&#x0;", "");
            xml = xml.Replace("\x00", "");

            return xml;
        }

        public static T FromXml (string xml)
        {
            // Compensate for stupid Mono encoding bugs

            if (xml.StartsWith ("?"))
            {
                xml = xml.Substring (1);
            }

            xml = xml.Replace ("&#x0;", "");
            xml = xml.Replace ("\x00", "");

            Console.WriteLine("XML: " + xml);
            Console.WriteLine("FromXml before new XmlSerializer()");

            XmlSerializer serializer = new XmlSerializer (typeof (T));

            MemoryStream stream = new MemoryStream();
            byte[] xmlBytes = Encoding.UTF8.GetBytes (xml);
            stream.Write (xmlBytes, 0, xmlBytes.Length);

            Console.WriteLine("Before Deserialize()");

            stream.Position = 0;
            T result = (T) serializer.Deserialize (stream);
            stream.Close();

            Console.WriteLine("Before Returning");

            return result;
        }
    }
}