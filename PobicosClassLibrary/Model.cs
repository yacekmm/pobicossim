using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace PobicosLibrary
{
    public class Model : PobicosLibrary.IPobicosModel
    {
        public Model(String clientID)
        {
            this.ClientID = clientID;
            Enabled = true;

        }

        public Boolean Enabled { get; set; }
        private Hashtable _properties = new Hashtable();
        private DataSet _definition = new DataSet();
        private List<IView> _views = new List<IView>();       

        #region IPobicosModel Members

        public Object GetProperty(string Name)
        {
            return _properties[Name];
        }
        public void SetProperty(string Name, object value)
        {
            _properties[Name] = value;
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
            _views.Add(view);
            view.Model = this;
        }
        public void RemoveObserver(IView view)
        {
            _views.Remove(view);
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
                //AdminTools.eventLog.WriteEntry("Server port set: " + _serverPort, EventLogEntryType.Information);
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
                //AdminTools.eventLog.WriteEntry("Server address set: " + _serverIP, EventLogEntryType.Information);
            }
 
        }

        public String Name
        {
            get
            {
                if (_properties.ContainsKey("name"))
                {
                return _properties["name"].ToString();
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
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    return null;
                }
                
            }
        }      



        public void Instruction(String instructionLabel,String callID, string param)
        {
            foreach (IPobicosView view in _views)
            {
                view.Instruction(instructionLabel,callID, param);                
            }
            
        }

        public void EventReturn(string callID, string returnValue)
        {
            foreach (IPobicosView view in _views)
            {
                view.EventReturn(callID, returnValue);
            }
        }      
        #endregion



        #region IModel Members


        public string[] ResourceDescripton
        {
            get
            {                
                List<String> list = new List<String>();
                String tmp;               
                
                foreach (DataTable dt in Definition.Tables)
                {
                    if (dt.TableName.Equals("instruction"))
                    {
                      if (  dt.Columns.Contains("name") )
                        {
                        foreach (DataRow dr in dt.Rows)
                        {                                                       
                            list.Add(dr["name"].ToString().ToLower());
                        }
                        }
                    }
                    if (dt.TableName.Equals("event"))
                    {
                        if (dt.Columns.Contains("event_Text"))
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                tmp = dr["event_Text"].ToString().Split('.')[0].ToLower();
                                if (!list.Contains(tmp))
                                    list.Add(tmp);
                            }
                        }
                        if (dt.Columns.Contains("name"))
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                tmp = dr["name"].ToString().ToLower();
                                if (!list.Contains(tmp))
                                    list.Add(tmp);
                            }
                        }
                    }
                    if (dt.TableName.Equals("definition"))
                    {
                        if (dt.Columns.Contains("definition_Text"))
                        {
                            foreach (DataRow dr in dt.Rows)
                            {                                
                                tmp = dr["definition_Text"].ToString().Split('.')[0].ToLower();
                                if (!list.Contains(tmp))
                                    list.Add(tmp);
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



        #region IModel Members

        private Socket _socket; 
        public System.Net.Sockets.Socket Socket
        {
            get
            {
                return _socket;
            }
            set
            {
                _socket = value;
            }
        }

        #endregion

        #region IModel Members

        private LinkStatus _linkStat = LinkStatus.OFF;
        public LinkStatus LinkStat
        {
            get
            {
                return _linkStat;
            }
            set
            {
                _linkStat = value;
            }
        }

        #endregion
    }
}
