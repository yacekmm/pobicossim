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
    public class AdminTools
    {
        private static EventLog _eventLog;
       // private static int number = 1;
        private static  Random rand = new Random(DateTime.Now.Millisecond);
        public static EventLog eventLog
        {
            private set
            {
                _eventLog = value;
            }
            get
            {
                if (_eventLog == null)
                {
                    _eventLog = new EventLog(Const.logName);
                    _eventLog.Source = Const.logSource;
                    _eventLog.EnableRaisingEvents = true;
                }
                return _eventLog;
            }
        }
        public AdminTools()
        {
        }

        public static void prepareLog()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                EventLog eventLog = new EventLog("POBICOS");
                if (!EventLog.SourceExists(Const.logSource))
                {
                    EventLog.CreateEventSource(Const.logName, Const.logName);
                }
                eventLog.Clear();
            }
        }

        public static void PrintDataSet(DataSet ds)
        {
            // Print out any name and extended properties.
            Console.WriteLine("DataSet is named: {0}", ds.DataSetName);
            foreach (DataRelation drel in ds.Relations)
            {
                Console.WriteLine("Relation : {0} ", drel.ToString());
            }

            foreach (System.Collections.DictionaryEntry de in ds.ExtendedProperties)
            {
                Console.WriteLine("Key = {0}, Value = {1}", de.Key, de.Value);
            }
            Console.WriteLine();
            foreach (DataTable dt in ds.Tables)
            {
                Console.WriteLine("=> {0} Table:", dt.TableName);
                // Print out the column names.
                for (int curCol = 0; curCol < dt.Columns.Count; curCol++)
                {
                    Console.Write(dt.Columns[curCol].ColumnName.Trim() + "\t");
                }
                Console.WriteLine("\n----------------------------------");
                // Call our new helper method.
                PrintTable(dt);
            }
        }

        public static String convertXML(String xml)
        {
            xml = xml.Replace("&gt;", "<");
            xml = xml.Replace("&lt;", ">");
            return xml;
        }

        public static void PrintTable(DataTable dt)
        {
            // Get the DataTableReader type.
            DataTableReader dtReader = dt.CreateDataReader();
            // The DataTableReader works just like the DataReader.
            while (dtReader.Read())
            {
                for (int i = 0; i < dtReader.FieldCount; i++)
                {
                    Console.Write("{0}\t", dtReader.GetValue(i).ToString().Trim());
                }
                Console.WriteLine();
            }
            dtReader.Close();
        }

        static public List<IPobicosModel> readConfiguration(string filename)
        {
            List<IPobicosModel> models = new List<IPobicosModel>();
            if (!filename.EndsWith("xml"))
            {
                eventLog.WriteEntry("Input file should have xml extension", EventLogEntryType.Error);
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
                eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                return models;
            }
            if (xmlDocument.FirstChild.NextSibling.Name.Equals("res:resource"))
            {
                throw new Exception("XML format no longer valid in this app");
                //model = new model(rand.next(10000).tostring());
                //dataset ds = new dataset();
                //ds.readxml(filename);
                //model.definition = ds;
                //models.add(model);
                //eventlog.writeentry("model loaded: " + model.clientid, eventlogentrytype.information);
                
            }
            else
            {
                foreach (XmlAttribute attrib in xmlDocument.FirstChild.NextSibling.Attributes)
                {
                    if (attrib.Name.Equals("serverIP"))
                        Model.serverIP = attrib.Value;
                    if (attrib.Name.Equals("serverPort"))
                        Model.serverPort = attrib.Value;
                }
                foreach (XmlNode node in xmlDocument.FirstChild.NextSibling.ChildNodes)
                {
                    if (node.Name.Equals("node"))
                    {
                        string id = rand.Next(10000).ToString();
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
                            string xml = AdminTools.convertXML(node.InnerText);
                            nodeDefinition.LoadXml(AdminTools.convertXML(node.InnerText));
                            DataSet ds = new DataSet();
                            String str = AdminTools.convertXML(node.InnerText);
                            StringReader stringReader = new StringReader(str);
                            ds.ReadXml(stringReader);
                            model.Definition = ds;
                            models.Add(model);
                            eventLog.WriteEntry("Model loaded: " + model.ClientID, EventLogEntryType.Information);
                        }
                        catch (XmlException)
                        {
                            eventLog.WriteEntry("Wrong model definition in XML file ", EventLogEntryType.Error);
                        }
                        
                    }
                }
            }
            return models;
        }

        public static string RemoveWhitespace(string str)
        {
            try
            {
                return new Regex(@"\s").Replace(str, string.Empty);
            }

            catch (Exception)
            {
                return str;
            }
        }
    }
}
