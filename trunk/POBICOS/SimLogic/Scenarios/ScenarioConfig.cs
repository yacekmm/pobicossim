using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PobicosLibrary;
using System.Xml.Serialization;
using System.IO;
using System.Collections;

namespace POBICOS.SimLogic.Scenarios
{
    public class ScenarioConfig
    {
        /*   enum status
           {
               ON = 1 ,
               OFF = 0
           }*/

        private ScenarioConfig()
        {

            objectsList = new Dictionary<string, string>();
        }
        private static ScenarioConfig _instance;

        public static ScenarioConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ScenarioConfig();
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        private static Dictionary<string, string> objectsList;

        public static void BuildFile()
        {
            object tmp = Instance;
            objectsList.Clear();
            foreach (IPobicosView so in SimScenario.pobicosObjectList)
            {
                objectsList.Add(so.Model.Name + '#' + so.Model.ClientID, "ON");
            }
            Serialize(objectsList);
        }


        private static void Serialize(IDictionary dictionary)
        {
            if (!Directory.Exists(@"Configurations"))
                Directory.CreateDirectory("Configurations");
            TextWriter writer = new StreamWriter(@"Configurations\objects.xml");
            List<PobicosObject> entries = new List<PobicosObject>(dictionary.Count);
            foreach (object key in dictionary.Keys)
            {
                entries.Add(new PobicosObject(key, dictionary[key]));
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<PobicosObject>));
            serializer.Serialize(writer, entries);
        }

        private static void Deserialize()
        {
            TextReader reader = new StreamReader(@"Configurations\objects.xml");
            if (objectsList == null)
                objectsList = new Dictionary<string, string>();

            objectsList.Clear();
            XmlSerializer serializer = new XmlSerializer(typeof(List<PobicosObject>));
            List<PobicosObject> list = (List<PobicosObject>)serializer.Deserialize(reader);

            foreach (PobicosObject entry in list)
            {
                objectsList[entry.ObjectName.ToString()] = entry.ObjectStatus.ToString();
            }
        }

        public static void readConfiguration()
        {
            if (File.Exists(@"Configurations\objects.xml"))
            {
                Deserialize();
                setConfiguration();


            }
            else
            {
                BuildFile();
            }
        }

        private static void setConfiguration()
        {
            String tmp;
            foreach (IPobicosView so in SimScenario.pobicosObjectList)
            {
                tmp = so.Model.Name + '#' + so.Model.ClientID;
                foreach (KeyValuePair<String, String> pair in objectsList)
                {
                    if (pair.Key.Equals(tmp))
                    {
                        if (pair.Value.Equals("ON"))
                            so.Model.Enabled = true;
                        else so.Model.Enabled = false;
                        break;
                    }
                }
            }
        }

        public class PobicosObject
        {
            public object ObjectName;
            public object ObjectStatus;

            public PobicosObject()
            {
            }

            public PobicosObject(object key, object value)
            {
                ObjectName = key;
                ObjectStatus = value;
            }
        }



    }
}
