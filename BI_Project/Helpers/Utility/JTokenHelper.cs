using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Xml.XPath;

namespace BI_Project.Helpers.Utility
{
    public static class JTokenHelper
    {
        

        public static JObject GetLanguage(string folderpath, string langName)
        {
            string fullXMLFilePath = folderpath + "/" + langName + ".xml";
            fullXMLFilePath=  System.Web.Hosting.HostingEnvironment.MapPath(fullXMLFilePath);

            JObject output = null;

            XmlDocument xmlDoc = null;
            xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(fullXMLFilePath);
            output = JObject.FromObject(xmlDoc);
            return output;
        }

        public static JObject GetXML2Jobject(string fullXMLFilePath)
        {


            JObject output = null;

            XmlDocument xmlDoc = null;
            xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(fullXMLFilePath);
            output = JObject.FromObject(xmlDoc);
            return output;
        }
        /// <summary>
        /// Get string value of element leaf in xml language
        /// </summary>
        /// <param name="language">Language get from xml file</param>
        /// <param name="objectType">block or page or common, or block</param>
        /// <param name="object_name">name of block as block_login</param>
        /// <param name="element_name">example title, the leaf element, that return string value </param>
        /// <returns></returns>
        public static string GetElementLanguage(JObject language, string objectType, string object_name, string element_name )
        {
            string output = "";
            try
            {
                output = language.SelectToken("$.roots.." + objectType + ".." + object_name + ".." + element_name).Value<string>();
            }
            catch(Exception ex)
            {
                output = ex.ToString();
            }
            return output;

        }

        /// <summary>
        /// Get string value of a element by the elementPath
        /// </summary>
        /// <param name="language">the xml language inform Jobject</param>
        /// <param name="elementPath">the path inform ("level1.level2.level3") </param>
        /// <returns></returns>
        public static string GetElementLanguage(JObject language, string elementPath)
        {
            string output = "";
            try
            {
                output = language.SelectToken("$.roots.." + elementPath).Value<string>();
            }
            catch (Exception ex)
            {
                output = ex.ToString();
            }
            return output;

        }
        public static string GetElementLanguage(Object language, string elementPath)
        {
            string output = "";
            JObject languageTempt = (JObject)language;
            try
            {
                output = languageTempt.SelectToken("$.roots.." + elementPath).Value<string>();
            }
            catch (Exception ex)
            {
                output = ""; // ex.ToString();
            }
            return output;

        }

        public static List<JToken> GetList(Object jObject, string path)
        {
            List<JToken> output = null;



            JObject jobject = (JObject)jObject;
            output = jobject.SelectTokens(path).ToList<JToken>();
            output = ((JToken)output[0]).ToList<JToken>();
            
            return output;
        }

        public static string GetElementValue(string xmlPath, string elementPath)
        {
            string output = "";
            JObject xmlJson = JTokenHelper.GetXML2Jobject(xmlPath);
            try
            {
                output = xmlJson.SelectToken("$.roots.." + elementPath).Value<string>();
            }
            catch (Exception ex)
            {
                output = ex.ToString();
            }
            return output;

        }

        public static JObject GetActiveJObject(JObject jObject,string elementPath)
        {
            JObject output = null;

            output = jObject.SelectToken("$.roots.." + elementPath).Value<JObject>();

            return output;
        }

    }
}