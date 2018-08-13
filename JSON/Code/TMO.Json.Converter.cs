using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WTF.Core.Xml;
using Newtonsoft.Json;

namespace TMO.Json
{
    public static class Converter
    {
        public static void ConvertXmlFileToJsonFile<T>(object myObject, string xmlFileName, string jsonFileName)
        {
            object xmlData = Serializer.DeserializeXmlFile(myObject.GetType(), xmlFileName);
            Serialization.SerializeObjectToJsonFile(xmlData, jsonFileName);
        }

        public static T ConvertXmlFileToJsonObject<T>(object myObject, string xmlFileName)
        {
            object xmlData = Serializer.DeserializeXmlFile(myObject.GetType(), xmlFileName);
            string jsonData = JsonConvert.SerializeObject(xmlData);

            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        public static string ConvertJsonStringToXml<T>(string jsonString) 
        {
            object obj = JsonConvert.DeserializeObject<T>(jsonString);            
            string retVal = Serializer.SerializeToXml(obj);
            return retVal; 
        }

        public static void ConvertJsonToXmlFile<T>(string jsonData, string outputFilePath)
        {
            string xml = ConvertJsonStringToXml<T>(jsonData);
            object item = Serializer.DeserializeXml(typeof(T), xml);
            Serializer.SerializeToXmlFile(xml, outputFilePath);
        }
    }
}
