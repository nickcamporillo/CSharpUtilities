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

namespace TmoLibrary.Common
{
    public static class Utilities
    {
        /// <summary>
        /// WARNING: This is a case-sensitive function, because C# properties are case-sensitive when using Reflection!
        /// </summary>
        public static string GetPropertyValue(object item, string propertyName)
        {
            string retVal = string.Empty;

            try
            {
                retVal = item.GetType().GetProperty(propertyName).GetValue(item).ToString();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return retVal;
        }

        /// <summary>
        /// WARNING: This is a case-sensitive function, because C# properties are case-sensitive when using Reflection!
        /// </summary>
        public static void SetPropertyValue(object item, string propertyName, string newVal)
        {               
            try
            {
                var r = new ReflectionCommon();
                r.SetValue(item, propertyName, newVal);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static string GetExecutingDirectory()
        {
            FileSystemManager f1 = new FileSystemManager();
            string directory = f1.GetBaseDirectory();   //This is a misleading name, that's why I'm wrapping it inside a more accurate name

            return directory;
        }

        public static string GetApplicationOutputDirectoryDirectory()
        {
            FileSystemManager f1 = new FileSystemManager();
            string outputDir = string.Empty;

            string executingDirectory = GetExecutingDirectory();

            IList<string> applicationDirectories = executingDirectory.Split('\\').Where(c=>c.ToString().Length>0).ToList();

            for (int i = 0; i < applicationDirectories.Count; i++)
            {
                outputDir = outputDir + (i==0 ? "" : "\\") + applicationDirectories[i];
                if (f1.DirectoryExists(outputDir + "\\output"))
                {
                    outputDir = outputDir + "\\output";                    
                }
            }
            return (!string.IsNullOrEmpty(outputDir) && outputDir.Contains("\\output") ? outputDir : string.Empty);
        }

        public static void CreateApplicationOutputDirectory()
        {
            FileSystemManager f1 = new FileSystemManager();
            string baseDir = GetApplicationOutputDirectoryDirectory();
            f1.CreateDirectory($@"{baseDir}\output");
        }

        //Btw, a variant of this is found in the WTF library, but this is the only func in that library.  I removed a hard coded string, "BOC", from the WTF version
        public static string GetJamesBond()
        {
            string identityName =  WindowsIdentity.GetCurrent().Name;
            string jamesBond = WTF.Core.Extensions.String.StringExtensions.GetLastToken(identityName, '\\');
            return jamesBond;
        }
        public static string FormatToYyyyMm(DateTime dt)
        {
            return dt.Year.ToString() + dt.Month.ToString().PadLeft(2, '0');
        }
        public static bool StringHasData(string s)
        {
            bool retVal = false;

            retVal = !(string.IsNullOrWhiteSpace(s));
            return retVal;
        }
        

        #region "Not in WTF library but should be!"
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
        public static bool IsWellFormedXml(string xml)
        {
            XmlUtility x = new XmlUtility();

            return XmlUtility.IsWellFormedXml(xml);
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
        #endregion
    }
}
