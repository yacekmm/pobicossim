using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using PobicosLibrary;
using System.Collections.Generic;
using System.Text;



namespace PobicosLibrary
{
    public class Client : PobicosLibrary.IPobicosController, IDisposable
    {
        public bool Running { private set; get; }
        private List<IModel> _models = new List<IModel>();
        public List<IModel> Models
        {
            get
            {
                return _models;               
            }
            set
            {
                _models = value;
            }
        }
        private clientType _type = clientType.OBJECT;
        public clientType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        private AsyncCallback _aSyncCallback;
        #region commandEvent
        public event CommandReceivedEventHandler CommandReceived;
        public delegate void CommandReceivedEventHandler(object sender, CommandArgs args);

        public void CommandReceivedCallback(object sender, CommandArgs args)
        {
        }
        #endregion

        #region contructions
        public Client()
        {
            Running = false;
            AdminTools.eventLog.EntryWritten += new EntryWrittenEventHandler(EventLogEntryWritten);
        }

        private void EventLogEntryWritten(object sender, EntryWrittenEventArgs e)
        {

            

        }
        #endregion

        #region connection

        public class SocketPacket
        {
            public IModel thisModel;
            public byte[] dataBuffer = new byte[1024];
        }

        private void WaitForData()
        {
            if (_aSyncCallback == null)
            {
                _aSyncCallback = new AsyncCallback(OnDataReceived);
            }

            SocketPacket socketPacket;
            if (this.Type.Equals(clientType.NODE))
            {
                socketPacket = new SocketPacket();
                socketPacket.thisModel = Models[0];
                Models[0].Socket.BeginReceive(socketPacket.dataBuffer, 0, socketPacket.dataBuffer.Length, SocketFlags.None, _aSyncCallback, socketPacket);
            }
            else
            {
                foreach (IModel model in Models)
                {
                    socketPacket = new SocketPacket();
                    socketPacket.thisModel = model;
                    model.Socket.BeginReceive(socketPacket.dataBuffer, 0, socketPacket.dataBuffer.Length, SocketFlags.None, _aSyncCallback, socketPacket);
                }
            }
        }

        private void OnDataReceived(IAsyncResult result)
        {
            try
            {

                SocketPacket theSockId = (SocketPacket)result.AsyncState;
                int iRx = theSockId.thisModel.Socket.EndReceive(result);
                char[] chars = new char[iRx + 1];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(theSockId.dataBuffer, 0, iRx, chars, 0);
                System.String szData = new System.String(chars);
                HandleCommand(theSockId.thisModel, szData);
                // szData = szData.Replace("\0", "");
                // AdminTools.eventLog.WriteEntry("RCV: " + szData, EventLogEntryType.Information);
                WaitForData();
            }
            catch (SocketException)
            {
                AdminTools.eventLog.WriteEntry("SS Sockect error, socket unexpectedly closed", EventLogEntryType.Information);
                Running = false;
                return;
            }
            catch (ObjectDisposedException)
            {
                AdminTools.eventLog.WriteEntry("OnDataReceived: Socket has been closed", EventLogEntryType.Information);
                //Running = false;
                return;
            }
        }
        private bool ConnectNode()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(Model.serverIP, int.Parse(Model.serverPort));
            foreach (IModel model in Models)
            {
                model.Socket = socket;
            }
            return true;
        }
        private bool ConnectObject()
        {
            foreach (IModel model in Models)
            {
                model.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                model.Socket.Connect(Model.serverIP, int.Parse(Model.serverPort));
            }
            return true;
        }
        //private 

        public bool Connect()
        {
            if (Models.Count == 0 || Running)
                return false;

            try
            {
                if (this.Type.Equals(clientType.OBJECT))
                    ConnectObject();
                else
                    ConnectNode();
                StringBuilder sb;
                Running = true;
                foreach (Model model in Models)
                {
                    sb = new StringBuilder();
                    foreach (String s in model.ResourceDescripton)
                    {
                        sb.Append(s);
                        sb.Append(';');
                    }
                    sb = new StringBuilder(Const.CONNECT + Const.DIV + Type + Const.DIV + model.ClientID + Const.DIV + sb.ToString());
                    send(model, sb.ToString());

                }
                
                WaitForData();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                AdminTools.eventLog.WriteEntry("Connection error, host: " + Model.serverIP + ", port: " + Model.serverPort, EventLogEntryType.Error);
                return false;
            }
        }

        public bool Disconnect()
        {
            int counter = 0;
            if (Running)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Model model in Models)
                    {
                        counter++;

                        if (Type.Equals(clientType.NODE))
                        {
                            sb.Append(Const.DISCONNECT + Const.DIV + model.ClientID + Environment.NewLine);
                            AdminTools.eventLog.WriteEntry("NODE " + model.ClientID + " disconnected ", EventLogEntryType.Information);
                            if (counter == Models.Count)
                            {
                                sb.Append(Const.DISCONNECT);                                
                                send(model, sb.ToString());
                                sb.Remove(0, sb.Length);
                                AdminTools.eventLog.WriteEntry("MW  disconnected ", EventLogEntryType.Information);
                            }
                        }
                        else
                        {
                            sb.Append(Const.DISCONNECT);                          
                            send(model, sb.ToString());
                            sb.Remove(0, sb.Length);
                            AdminTools.eventLog.WriteEntry("OBJECT " + model.ClientID + " disconnected ", EventLogEntryType.Information);
                        }
                    }
                    Running = false;
                }
                catch (IOException e)
                {
                    AdminTools.eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
                }
                Dispose();
            }
            return true;
        }
        #endregion


        private bool HandleCommand(IModel mdl, String command)
        {

            try
            {
                CommandArgs commandArgs = new CommandArgs();
                string[] parts = command.Split(Const.DIV);
                bool commandCorrect = false;
                switch (parts[0])
                {
                    #region protocolver1
                    case Const.HELLO:
                        /*     commandArgs.Command = Const.HELLO;
                             commandArgs.Status = parts[1];
                             StringReader sr = new StringReader(parts[2]);
                             DataSet ds = new DataSet();
                             ds.ReadXml(sr);
                             commandArgs.nodeDefinition = ds;*/
                        commandCorrect = true;
                        break;
                    case Const.BYE:
                        commandArgs.Command = Const.BYE;
                        commandArgs.Status = parts[1];
                        commandCorrect = true;
                        break;
                    case Const.STOP:
                        commandArgs.Command = Const.STOP;
                        Disconnect();
                        commandCorrect = true;
                        break;
                    #endregion
                    case Const.LINK_STATUS:
                        commandArgs.Command = Const.LINK_STATUS;
                        commandArgs.Status = parts[1];
                        if (this.Type.Equals(clientType.OBJECT))
                        {
                         if (commandArgs.Status.Equals(LinkStatus.ON.ToString()) )
                         {
                             mdl.LinkStat = LinkStatus.ON;
                         } else
                         {
                             mdl.LinkStat = LinkStatus.OFF;
                         }
                            
                        }
                        commandCorrect = true;
                        break;
                    case Const.INSTR:
                        commandArgs.Command = Const.INSTR;
                        commandArgs.NodeId = parts[1].Split('#')[0];
                        commandArgs.CallID = parts[1].Split('#')[1];
                        commandArgs.InstructionLabel = parts[2];
                        commandArgs.Params = parts[3];
                        (mdl as IPobicosModel).Instruction(commandArgs.InstructionLabel, commandArgs.CallID, commandArgs.Params);

                        /*foreach (IPobicosModel model in Models)
                        {
                            if (model.ClientID.Equals(commandArgs.NodeId))
                            {
                                model.Instruction(commandArgs.InstructionLabel, commandArgs.CallID, commandArgs.Params);
                            }
                        }*/
                        commandCorrect = true;
                        break;
                    case Const.EVENT:
                        commandArgs.Command = Const.EVENT;
                        commandArgs.Status = parts[1];
                        commandArgs.InstructionLabel = parts[2];
                        commandArgs.Params = parts[3];
                        commandCorrect = true;
                        break;
                    case Const.INSTR_RET:
                        commandArgs.Command = Const.INSTR_RET;
                        commandArgs.Status = parts[1].Split('#')[0];
                        commandArgs.InstructionLabel = parts[2];
                        commandArgs.Params = parts[1].Split('#')[1];
                        foreach (IPobicosModel model in Models)
                        {
                            if (model.ClientID.Equals(commandArgs.Status))
                            {
                                model.InstructionReturn(commandArgs.Params, commandArgs.InstructionLabel);
                            }
                        }
                        commandCorrect = true;
                        break;
                    case Const.EVENT_RET:
                        commandArgs.Command = Const.EVENT_RET;
                        commandArgs.Status = parts[1];
                        commandArgs.InstructionLabel = parts[2];
                        commandArgs.Params = parts[3];
                        break;
                    default:
                        commandCorrect = false;
                        break;
                }
                if (commandCorrect && CommandReceived != null)
                    CommandReceived(this, commandArgs);
                return commandCorrect;
            }
            catch (XmlException xmlE)
            {
                AdminTools.eventLog.WriteEntry(xmlE.Message, EventLogEntryType.Error);
                return false;
            }
            catch (Exception e)
            {
                AdminTools.eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
                return false;
            }
        }

        private void send(IModel model, String str)
        {
            if ((Type.Equals(clientType.NODE) || model.LinkStat.Equals(LinkStatus.ON) || str.Contains(Const.CONNECT)) && Running)
            {
               // if (!str.EndsWith(Environment.NewLine))
                    str += Environment.NewLine;
                model.Socket.Send(System.Text.Encoding.ASCII.GetBytes(str));                             
              //  AdminTools.eventLog.WriteEntry("SND: " + str);
            }
        }

        #region IPobicosController Members


        public void RegisterModel(PobicosLibrary.IModel model)
        {
            Models.Add(model);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            foreach (IPobicosModel m in Models)
            {
                //s.Shutdown(SocketShutdown.Both);
                m.Socket.Close(2000);
            }
        }

        #endregion
        #region IInstrEvents Members

        public void Instruction(IPobicosModel sender, InstructionsList instruction, string callID, string parameters)
        {
            string tmp = callID;
            if (callID == null)
                callID = sender.GetHashCode().ToString();
            send(sender, Const.INSTR + Const.DIV + sender.ClientID + Const.HASH + tmp + Const.DIV + instruction + Const.DIV + "(" + parameters + ")");

        }

        public void InstructionReturn(IPobicosModel sender, string callID, string value)
        {
            string tmp = callID;
            if (tmp == null)
                tmp = sender.GetHashCode().ToString();
            send(sender, Const.INSTR_RET + Const.DIV + sender.ClientID + Const.HASH + tmp + Const.DIV + value);
        }

        public void Event(IPobicosView sender, EventsList evnt, string callID, string parameters)
        {
            try
            {
                string tmp = callID;
                if (callID == null)
                    callID = sender.GetHashCode().ToString();
                send(sender.Model, Const.EVENT + Const.DIV + (sender.Model as IPobicosModel).ClientID + Const.HASH + tmp + Const.DIV + evnt + Const.DIV + "(" + parameters + ")");
            }
            catch (NullReferenceException)
            {
                if (Running)
                    AdminTools.eventLog.WriteEntry("Error in Client:Event", EventLogEntryType.Error);
                else
                    AdminTools.eventLog.WriteEntry("Event raised during disconnected state", EventLogEntryType.Information);
            }
        }

        public void EventReturn(IPobicosView sender, string callID, string value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


}
