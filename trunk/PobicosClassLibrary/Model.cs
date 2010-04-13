﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace PobicosLibrary
{
    public class Model : PobicosLibrary.IPobicosModel
    {
        public Model(String clientID)
        {
            this.ClientID = clientID;
        }

        private Hashtable properties = new Hashtable();
        private DataSet _definition = new DataSet();
        private List<IView> views = new List<IView>();       

        #region IPobicosModel Members

        public Object GetProperty(string Name)
        {
            return properties[Name];
        }
        public void SetProperty(string Name, object value)
        {
            properties[Name] = value;
        }

        public DataSet Definition
        {
            get
            {
                return _definition;
            }
            set
            {
                _definition = value;
                if (_definition.Tables["result"] != null)
                {
                    DataColumn dc = new DataColumn("value");
                    dc.DefaultValue = 0;
                    _definition.Tables["result"].Columns.Add(dc);
                }
            }
        }






        public DataTable ResultTable
        {
            get
            {
                return Definition.Tables["result"];
            }
        }

        public void AddObserver(IView view)
        {
            views.Add(view);
            view.Model = this;
        }
        public void RemoveObserver(IView view)
        {
            views.Remove(view);
        }


        string _clientId;
        public string ClientID
        {
            get
            {
                return _clientId;
            }
            private set
            {
                _clientId = value;
            }
        }



        private static string  _serverPort = "40007";
        public static  string serverPort
        {
            get
            {
                return _serverPort;                
            }
            set
            {
                _serverPort = value;
                AdminTools.eventLog.WriteEntry("Server port set: " + _serverPort, EventLogEntryType.Information);
            }
        }
        private static string _serverIP = /*"192.168.46.155"; //*/"localhost"; 
        public  static  string serverIP
        {
            get
            {
                return _serverIP;
            }
            set
            {
                _serverIP = value;
                AdminTools.eventLog.WriteEntry("Server address set: " + _serverIP, EventLogEntryType.Information);
            }
 
        }

        public String Name
        {
            get
            {
                if (properties.ContainsKey("name"))
                {
                return properties["name"].ToString();
                }
                return "noname";

            }
        }

        #endregion

        #region IPobicosModel Members


        public DataTable EventTable
        {
            get
            {
                try
                {
                    return Definition.Tables["event"];
                }
                catch (Exception)
                {
                    return null;
                }
                
            }
        }      



        public void Instruction(String instructionLabel,String callID, string param)
        {
            foreach (IPobicosView view in views)
            {
                view.Instruction(instructionLabel,callID, param);                
            }
            
        }

        public void EventReturn(string callID, string returnValue)
        {
            foreach (IPobicosView view in views)
            {
                view.EventReturn(callID, returnValue);
            }
        }


        private StreamWriter _streamWriter;
        public System.IO.StreamWriter streamWriter
        {
            get
            {
                return _streamWriter; 
            }
            set
            {
                _streamWriter = value;
            }
        }
        private StreamReader _streamReader;
        public System.IO.StreamReader streamReader
        {
            get
            {
                return _streamReader;
            }
            set
            {
                _streamReader = value;
            }
        }

        #endregion



        #region IModel Members


        public string[] resourceDescripton
        {
            get
            {
                List<String> list = new List<String>();
                
                foreach (DataTable dt in Definition.Tables)
                {
                    if (dt.TableName.Equals("instruction"))
                    {
                      if (  dt.Columns.Contains("name") )
                        {
                        foreach (DataRow dr in dt.Rows)
                        {
                            
                            list.Add(dr["name"].ToString());
                        }
                        }
                    }
                    if(dt.TableName.Equals("physical_event"))
                    {
                        if (dt.Columns.Contains("label"))
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                list.Add(dr["label"].ToString());
                            }
                        }
                    }

                }
                
                return list.ToArray();
            }
            private set
            {
            }
        }

        #endregion

        #region IPobicosModel Members


        public void InstructionReturn(string callID, string returnValue)
        {
          //  throw new NotImplementedException();
        }

        #endregion


    }
}
