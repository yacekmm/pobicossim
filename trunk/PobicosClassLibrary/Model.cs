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
    /// <summary>
    /// Model class (according to  MVC pattern)
    /// </summary>
    public class Model : PobicosLibrary.IPobicosModel
    {
        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="clientID"></param>
        public Model(String clientID)
        {
            this.ClientID = clientID;
            Enabled = true;
        }
        /// <summary>
        /// allows to disable pobicos functionality in object
        /// </summary>
        public Boolean Enabled { get; set; }
        private Hashtable _properties = new Hashtable();
        private DataSet _definition = new DataSet();
        private List<IView> _views = new List<IView>();
        private string _clientId;
        private static string _serverPort = "40007";
        private static string _serverIP = /*"192.168.46.155"; //*/"localhost";
        private Socket _socket;
        private LinkStatus _linkStat = LinkStatus.OFF;

        #region IPobicosModel Members
        /// <summary>
        /// allows to get property by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Object GetProperty(string Name)
        {
            return _properties[Name];
        }
        /// <summary>
        /// allows to set property by name
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="value"></param>
        public void SetProperty(string Name, object value)
        {
            _properties[Name] = value;
        }
        /// <summary>
        /// holds object definition
        /// </summary>
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

        /// <summary>
        /// registers view in the class
        /// </summary>
        /// <param name="view"></param>
        public void RegisterObserver(IView view)
        {
            _views.Add(view);
            view.Model = this;
        }
        /// <summary>
        /// unregister view in the class
        /// </summary>
        /// <param name="view"></param>
        public void RemoveObserver(IView view)
        {
            _views.Remove(view);
        }
        
        /// <summary>
        /// returns client unique ID
        /// </summary>
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
        /// <summary>
        /// holds server port
        /// </summary>
        public static  string ServerPort
        {
            get
            {
                return _serverPort;                
            }
            set
            {
                _serverPort = value;                
            }
        }
        /// <summary>
        /// holds serverIP
        /// </summary>
        public  static  string ServerIP
        {
            get
            {
                return _serverIP;
            }
            set
            {
                _serverIP = value;                
            }
 
        }
        /// <summary>
        /// returns the name of the object
        /// </summary>
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


        /// <summary>
        /// passes instruction command to all registered views
        /// </summary>
        /// <param name="instructionLabel"></param>
        /// <param name="callID"></param>
        /// <param name="param"></param>
        public void Instruction(String instructionLabel,String callID, string param)
        {
            foreach (IPobicosView view in _views)
            {
                view.Instruction(instructionLabel,callID, param);                
            }
            
        }
        /// <summary>
        /// passes event return command to all registered views
        /// </summary>
        /// <param name="callID"></param>
        /// <param name="returnValue"></param>
        public void EventReturn(string callID, string returnValue)
        {
            foreach (IPobicosView view in _views)
            {
                view.EventReturn(callID, returnValue);
            }
        }      
        /// <summary>
        /// not supported
        /// </summary>
        /// <param name="callID"></param>
        /// <param name="returnValue"></param>
        public void InstructionReturn(string callID, string returnValue)
        {
          //  throw new NotImplementedException();
        }

        #endregion

        #region IModel Members

/// <summary>
/// returns list of instructions and events
/// </summary>
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
                        if (dt.Columns.Contains("name"))
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
        /// <summary>
        /// holds the socket for current model
        /// </summary>
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

        /// <summary>
        /// holds link status info
        /// </summary>
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
