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
    /// <summary>
    /// Class connecting simulator with SS server, controller in MVC pattern
    /// </summary>
    public class Client : PobicosLibrary.IPobicosController, IDisposable
    {
        /// <summary>
        /// shows if client is running
        /// </summary>
        public bool Running { private set; get; }
        private List<IModel> _models = new List<IModel>();
        private clientType _type = clientType.OBJECT;
        private AsyncCallback _aSyncCallback;

        /// <summary>
        /// List of models connected to class
        /// </summary>
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
        /// <summary>
        /// Type of client (NODE or OBJECT)
        /// </summary>
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
        
        #region commandEvent
        /// <summary>
        /// Is raised when class receives any proper command
        /// </summary>
        public event CommandReceivedEventHandler CommandReceived;
        /// <summary>
        /// delegate, is executed when CommandReceived event happens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void CommandReceivedEventHandler(object sender, CommandArgs args); 
        #endregion

        #region contructors
        /// <summary>
        /// default constructor
        /// </summary>
        public Client()
        {
            Running = false;
            AdminTools.Init();
        }

        #endregion

        #region connection
        /// <summary>
        /// helper class, packs data received from socket
        /// </summary>
        public class SocketPacket
        {
            /// <summary>
            /// the data in packet are for this model
            /// </summary>
            public IModel Model;
            /// <summary>
            /// data in the packet
            /// </summary>
            public byte[] DataBuffer = new byte[1024];
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
                socketPacket.Model = Models[0];
                Models[0].Socket.BeginReceive(socketPacket.DataBuffer, 0, socketPacket.DataBuffer.Length, SocketFlags.None, _aSyncCallback, socketPacket);
            }
            else
            {
                foreach (IModel model in Models)
                {
                    if (model.Enabled)
                    {
                        socketPacket = new SocketPacket();
                        socketPacket.Model = model;
                        model.Socket.BeginReceive(socketPacket.DataBuffer, 0, socketPacket.DataBuffer.Length, SocketFlags.None, _aSyncCallback, socketPacket);
                    }

                }
            }
        }

        private void OnDataReceived(IAsyncResult result)
        {
            try
            {

                SocketPacket theSockId = (SocketPacket)result.AsyncState;
                int iRx = theSockId.Model.Socket.EndReceive(result);
                char[] chars = new char[iRx + 1];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(theSockId.DataBuffer, 0, iRx, chars, 0);
                System.String szData = new System.String(chars);
                HandleCommand(theSockId.Model, szData);
                WaitForData();
            }
            catch (SocketException)
            {
                Trace.TraceError("SS Sockect error, socket unexpectedly closed");              
                Running = false;
                return;
            }
            catch (ObjectDisposedException)
            {
                Trace.TraceError("OnDataReceived: Socket has been closed");
                return;
            }
        }
        private bool ConnectNode()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(Model.ServerIP, int.Parse(Model.ServerPort));
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
                if (model.Enabled)
                {
                    model.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    model.Socket.Connect(Model.ServerIP, int.Parse(Model.ServerPort));
                }
            }
            return true;
        }
        
        /// <summary>
        /// Connects client to SS server
        /// </summary>
        /// <returns></returns>
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
            catch (Exception)
            {                
                Trace.TraceError("Connection error, host: " + Model.ServerIP + ", port: " + Model.ServerPort);               
                return false;
            }
        }
        /// <summary>
        /// Disconnects client from SS server
        /// </summary>
        /// <returns></returns>
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
                            Trace.TraceInformation("NODE " + model.ClientID + " disconnected ");
                            if (counter == Models.Count)
                            {
                                sb.Append(Const.DISCONNECT);                                
                                send(model, sb.ToString());
                                sb.Remove(0, sb.Length);
                                Trace.TraceInformation("MW  disconnected ");                              
                            }
                        }
                        else
                        {
                            if (model.Enabled)
                            {
                                sb.Append(Const.DISCONNECT);
                                send(model, sb.ToString());
                                sb.Remove(0, sb.Length);
                                Trace.TraceInformation("OBJECT " + model.ClientID + " disconnected ");                             
                            }
                        }
                    }
                    Running = false;
                }
                catch (IOException e)
                {
                    Trace.TraceError(e.Message);                    
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
                        
                        commandArgs.CallID = parts[1].Split('#')[1];
                        commandArgs.InstructionLabel = parts[2];
                        commandArgs.Params = parts[3];
                        (mdl as IPobicosModel).Instruction(commandArgs.InstructionLabel, commandArgs.CallID, commandArgs.Params);
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
            catch (Exception e)
            {
                Trace.TraceError(e.Message);             
                return false;
            }
        }

        private void send(IModel model, String str)
        {
            if ((Type.Equals(clientType.NODE) || model.LinkStat.Equals(LinkStatus.ON) || str.Contains(Const.CONNECT)) && Running && model.Enabled)
            {               
                    str += Environment.NewLine;
                    model.Socket.Send(System.Text.Encoding.ASCII.GetBytes(str));
            }
        }

        #region IPobicosController Members
        /// <summary>
        /// Registers new model in client
        /// </summary>
        /// <param name="model"></param>
        public void RegisterModel(PobicosLibrary.IModel model)
        {
            Models.Add(model);
        }
        #endregion
        /// <summary>
        /// Disposes sockets in all models
        /// </summary>
        public void Dispose()
        {
            foreach (IPobicosModel m in Models)
            {
                if (m.Enabled)
                {                    
                    m.Socket.Close(2000);
                }
            }
        }
        #region IInstrEvents Members
        /// <summary>
        /// sends instruction
        /// </summary>
        /// <param name="sender">who sends</param>
        /// <param name="instruction">what instruction</param>
        /// <param name="callID">what call id</param>
        /// <param name="parameters">any parameters</param>
        public void Instruction(IPobicosModel sender, InstructionsList instruction, string callID, string parameters)
        {
            string tmp = callID;
            if (callID == null)
                callID = sender.GetHashCode().ToString();
            send(sender, Const.INSTR + Const.DIV + sender.ClientID + Const.HASH + tmp + Const.DIV + instruction + Const.DIV + "(" + parameters + ")");

        }
        /// <summary>
        /// sends instruction return
        /// </summary>
        /// <param name="sender">who sends it</param>
        /// <param name="callID"> what call id</param>
        /// <param name="value">the value</param>
        public void InstructionReturn(IPobicosModel sender, string callID, string value)
        {
            string tmp = callID;
            if (tmp == null)
                tmp = sender.GetHashCode().ToString();
            send(sender, Const.INSTR_RET + Const.DIV + sender.ClientID + Const.HASH + tmp + Const.DIV + value);
        }

/// <summary>
        ///  sends event return
/// </summary>
/// <param name="sender">who sends it </param>
/// <param name="evnt">what event</param>
/// <param name="callID">the call id </param>
/// <param name="parameters">list of parameters</param>
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
                {         
                    Trace.TraceError("Error in Client:Event");
                }
                else                   
                {         
                    Trace.TraceError("Event raised during disconnected state");
                }
            }
        }
        /// <summary>
        /// sends event return
        /// </summary>
        /// <param name="sender">who sends it</param>
        /// <param name="callID">what call id</param>
        /// <param name="value">return value</param>
        public void EventReturn(IPobicosView sender, string callID, string value)
        {
            string tmp = callID;
            if (tmp == null)
                tmp = sender.GetHashCode().ToString();
            send(sender.Model, Const.EVENT_RET + Const.DIV + sender.Model.ClientID + Const.HASH + tmp + Const.DIV + value);
        }

        #endregion
    }


}
