using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Data;
using System.Xml;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PobicosLibrary
{
    /// <summary>
    /// helper class, used for logging initialization  and reading configuration of objects ( XML files )
    /// </summary>
    public class AdminTools
    {      
        private static  Random _rand = new Random(DateTime.Now.Millisecond);
        /// <summary>
        /// initializes logging in the library
        /// </summary>
        public static void Init()
        {          
                Trace.Listeners.Clear();
                if (!Directory.Exists("Logs"))
                {
                    Directory.CreateDirectory("Logs");
                }
                Trace.Listeners.Add(new TextWriterTraceListener(@"Logs\log-"+DateTime.Now.ToShortDateString()+".log"));
                Trace.AutoFlush = true;
                Trace.TraceInformation("New instance started");           
        }        
        //public static void PrintDataSet(DataSet ds)
        //{
        //    // Print out any name and extended _properties.
        //    Console.WriteLine("DataSet is named: {0}", ds.DataSetName);
        //    foreach (DataRelation drel in ds.Relations)
        //    {
        //        Console.WriteLine("Relation : {0} ", drel.ToString());
        //    }

        //    foreach (System.Collections.DictionaryEntry de in ds.ExtendedProperties)
        //    {
        //        Console.WriteLine("Key = {0}, Value = {1}", de.Key, de.Value);
        //    }
        //    Console.WriteLine();
        //    foreach (DataTable dt in ds.Tables)
        //    {
        //        Console.WriteLine("=> {0} Table:", dt.TableName);
        //        // Print out the column names.
        //        for (int curCol = 0; curCol < dt.Columns.Count; curCol++)
        //        {
        //            Console.Write(dt.Columns[curCol].ColumnName.Trim() + "\t");
        //        }
        //        Console.WriteLine("\n----------------------------------");
        //        // Call our new helper method.
        //        PrintTable(dt);
        //    }
        //}
        private static String ConvertXML(String xml)
        {
            xml = xml.Replace("&gt;", "<");
            xml = xml.Replace("&lt;", ">");
            return xml;
        }
        //public static void PrintTable(DataTable dt)
        //{
        //    // Get the DataTableReader type.
        //    DataTableReader dtReader = dt.CreateDataReader();
        //    // The DataTableReader works just like the DataReader.
        //    while (dtReader.Read())
        //    {
        //        for (int i = 0; i < dtReader.FieldCount; i++)
        //        {
        //            Console.Write("{0}\t", dtReader.GetValue(i).ToString().Trim());
        //        }
        //        Console.WriteLine();
        //    }
        //    dtReader.Close();
        //}
        /// <summary>
        /// reads configuration of pobicos objects
        /// </summary>
        /// <param name="filename">file</param>
        /// <returns>list of models</returns>
        static public List<IPobicosModel> ReadConfiguration(string filename)
        {
            List<IPobicosModel> models = new List<IPobicosModel>();
            if (!filename.EndsWith("xml"))
            {
                Trace.TraceError("Input file should have xml extension");                
                return null;
            }
            Model model;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(filename);
                xmlDocument.Normalize();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);               
                return models;
            }
            if (xmlDocument.FirstChild.NextSibling.Name.Equals("res:resource"))
            {
                throw new Exception("XML format no longer valid in this app");
                //model = new model(rand.next(10000).tostring());
                //dataset ds = new dataset();
                //ds.readxml(filename);
                //model.definition = ds;
                //Models.add(model);
                //eventlog.writeentry("model loaded: " + model.clientid, eventlogentrytype.information);
                
            }
            else
            {
                foreach (XmlAttribute attrib in xmlDocument.FirstChild.NextSibling.Attributes)
                {
                    if (attrib.Name.Equals("serverIP"))
                        Model.ServerIP = attrib.Value;
                    if (attrib.Name.Equals("serverPort"))
                        Model.ServerPort = attrib.Value;
                }
                foreach (XmlNode node in xmlDocument.FirstChild.NextSibling.ChildNodes)
                {
                    if (node.Name.Equals("node"))
                    {
                        string id = _rand.Next(10000).ToString();
                        foreach (XmlAttribute attrib in node.Attributes)
                        {
                            if (attrib.Name.Equals("id"))
                                id = attrib.Value;


                        }
                        model = new Model(id);
                        foreach (XmlAttribute atr in node.Attributes)
                        {
                            model.SetProperty(atr.Name, atr.Value);
                        }

                        try
                        {
                            XmlDocument nodeDefinition = new XmlDocument();
                            string xml = AdminTools.ConvertXML(node.InnerText);
                            nodeDefinition.LoadXml(AdminTools.ConvertXML(node.InnerText));
                            DataSet ds = new DataSet();
                            String str = AdminTools.ConvertXML(node.InnerText);
                            StringReader stringReader = new StringReader(str);
                            ds.ReadXml(stringReader);
                            model.Definition = ds;
                            models.Add(model);
                            Trace.TraceInformation("Model loaded: " + model.ClientID);
                            
                        }
                        catch (XmlException)
                        {
                            Trace.TraceError("Wrong model definition in XML file ");                            
                        }
                        
                    }
                }
            }
            return models;
        }
        private static string RemoveWhitespace(string str)
        {
            try
            {
                return new Regex(@"\s").Replace(str, string.Empty);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return str;
            }
        }
    }
}
