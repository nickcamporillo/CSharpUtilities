using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using WTF.Core.Reflection;
using WTF.Core.FileSystem;
using WTF.Core.Xml;
using System.Security.Principal;

namespace TMO.WTFX.Xml
{
    public static class Serializer
    {       
        public static string SerializeObjectToXmlString(object serializableTarget)
        {
            //From: https://stackoverflow.com/questions/1772004/how-can-i-make-the-xmlserializer-only-serialize-plain-xml
            string xml = string.Empty;
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(serializableTarget.GetType());
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, serializableTarget, emptyNamepsaces);
                    xml = stream.ToString();
                }
            }

            return xml;
        }
        public static void SerializeObjectToBinaryFile(object serializableObject, string outputFileFullPath)
        {
            FileSystemManager mgr = new FileSystemManager();
            if (mgr.FileExists(outputFileFullPath))
            {
                mgr.DeleteFile(outputFileFullPath);
            }

            using (Stream stream = new FileStream(outputFileFullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, serializableObject);
                stream.Close();
            }
        }
        public static object DeserializeBinaryXmlFromFile(string outputFileFullPath)
        {            
            object item = null;

            using (Stream stream = new FileStream(outputFileFullPath, FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                item = formatter.Deserialize(stream);
                stream.Close();
            }

            return item;
        }

        public static bool SerializeObjectToXmlFile(string fullFileName, object objectForSerialization, bool stripXmlDirective)
        {
            bool retVal = false;
            FileSystemManager fileMgr = new FileSystemManager();

            if (fileMgr.FileExists(fullFileName))
            {
                fileMgr.DeleteFile(fullFileName);
            }

            if (stripXmlDirective)
            {                
                //--Can't use ISB's WTF library, doesn't allow me to strip the xml directive, so use "CreateXml"
                fileMgr.WriteFile(fullFileName, SerializeObjectToXmlString(objectForSerialization));
            }
            else
            {
                Serializer.SerializeToXmlFile(objectForSerialization, fullFileName);
            }                


            retVal = (fileMgr.FileExists(fullFileName));

            fileMgr = null;

            return retVal;
        }

        public static void TransformXmlFile(string xmlFileFullPathAndName, string styleSheetFileFullPathAndName, string outputfileFullPathAndName)
        {
            var inputReader = new XmlUtility();
            var transformer = new XsltTransformer();

            string input = inputReader.ReadXmlFile(xmlFileFullPathAndName);
            string style = transformer.ReadStyleSheet(styleSheetFileFullPathAndName);            

            transformer.Transform(input, style, outputfileFullPathAndName);
        }
    }
}
