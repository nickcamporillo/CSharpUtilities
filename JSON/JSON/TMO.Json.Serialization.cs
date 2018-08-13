using System;
using Newtonsoft.Json;
using WTF.Core.FileSystem;

namespace TMO.Json
{
    public class Serialization
    {
        private static FileSystemManager fmgr = new FileSystemManager();

        public static string SerializeObjectToJson(object objectToSerialize)
        {
            string jsonResultString = JsonConvert.SerializeObject(objectToSerialize);

            return jsonResultString;
        }

        public static void SerializeObjectToJsonFile(object objectToSerialize, string jsonFiile)
        {
            string jsonResults = JsonConvert.SerializeObject(objectToSerialize);

            fmgr.WriteFile(jsonFiile, jsonResults);
        }

        public static T DeserializeJsonFileToObject<T>(string jsonFileName)
        {
            string jsonData = fmgr.ReadFile(jsonFileName);

            T rawParsed = JsonConvert.DeserializeObject<T>(jsonData);
            return rawParsed;
        }
    }
}
