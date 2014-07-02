using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Controller.Util
{
    class XmlSerializer
    {
        public static string Serialize<T>(T item)
        {
            MemoryStream memStream = new MemoryStream();
            using (XmlTextWriter textWriter = new XmlTextWriter(memStream, Encoding.Unicode))
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                serializer.Serialize(textWriter, item);

                memStream = textWriter.BaseStream as MemoryStream;
            }
            if (memStream != null)
                return Encoding.Unicode.GetString(memStream.ToArray());
            else
                return null;
        }

        public static void SerializeFile<T>(T item, string filepath)
        {
            FileStream fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
            using (XmlTextWriter textWriter = new XmlTextWriter(fileStream, Encoding.Unicode))
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                serializer.Serialize(textWriter, item);
            }
        }

        public static T DeserializeFile<T>(string filepath)
        {
            if (!File.Exists(filepath))
                return default(T);

            using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fileStream);
            }
        }

        public static T Deserialize<T>(string xmlString)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
                return default(T);

            using (MemoryStream memStream = new MemoryStream(Encoding.Unicode.GetBytes(xmlString)))
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(memStream);
            }
        }
    }
}
