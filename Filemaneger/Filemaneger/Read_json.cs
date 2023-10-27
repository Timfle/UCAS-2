using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Filemaneger
{
    internal class Read_json
    {
        public static string Code(string jsonData)
        {
            string code = "00";

            JObject jObject = JObject.Parse(jsonData);
            try
            {
                code = jObject.GetValue("code").ToString();
            }
            catch
            {
                Debug.WriteLine("code error");
            }

            return code;
        }

        public static int Port(string jsonData)
        {

            int port = 0;

            JObject jObject = JObject.Parse(jsonData);
            try
            {
                port = jObject.GetValue("port").Value<int>();
            }
            catch
            {
                Debug.WriteLine("port error");
            }
            return port;
        }

        public static T Json_data<T>(string jsonData, string name)
        {
            Debug.WriteLine("\n");
            JToken value = null;
            JObject jObject = JObject.Parse(jsonData);
            try
            {
                value = jObject.GetValue(name);
                Debug.WriteLine(value);
                return value != null ? value.ToObject<T>() : default(T);
            }
            catch
            {

                return ( typeof(T) == typeof(bool) || typeof(T) == typeof(int) ) ? (T)(object)0 : (T)(object)"0";
            }

        } 

    }
}
