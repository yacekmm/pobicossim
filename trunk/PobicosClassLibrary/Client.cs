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
        private Thread readThread;
        public bool Running { set; get; }
        public List<IModel> models = new List<IModel>();
        public clientType Type = clientType.OBJECT;
        private List<Socket> sockets = new List<Socket>();
        



        static readonly object padlock = new object();

        #region commandEvent
        public event CommandReceivedEventHandler commandReceived;
        public delegate void CommandReceivedEventHandler(object sender, CommandArgs args);

        public void CommandReceivedCallback(object sender, CommandArgs args)
        {
        }
        #endregion

        #region contructors

        public Client()
        {
            Running = false;
            AdminTools.eventLog.WriteEntry("Client constructed", EventLogEntryType.Information);
            AdminTools.eventLog.EntryWritten += new EntryWrittenEventHandler(eventLog_EntryWritten);
        }

        void eventLog_EntryWritten(object sender, EntryWrittenEventArgs e)
        {
            Console.WriteLine(e.Entry.Message);
        }


        #endregion

        #region connection

        private void initializeNetwork(Socket sock, IPobicosModel model)
        {
            NetworkStream networkStream = new NetworkStream(sock);
            sockets.Add(sock);

            if (model.streamWriter == null)
            {
                model.streamWriter = new StreamWriter(networkStream);
                model.streamWriter.AutoFlush = true;
            }
            if (model.streamReader == null)
            {
                model.streamReader = new StreamReader(networkStream);
            }

        }

        public bool Connect()
        {

            try
            {
                Socket socket;
                {
                    if (this.Type.Equals(clientType.OBJECT))
                    {
                        foreach (Model model in models)
                        {
                            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            socket.Connect(Model.serverIP, int.Parse(Model.serverPort));
                            initializeNetwork(socket, model);
                            StringBuilder sb = new StringBuilder();
                            foreach (String s in model.ResourceDescripton)
                            {
                                sb.Append(s);
                                sb.Append(';');
                            }
                            model.streamWriter.WriteLine(Const.CONNECT + Const.DIV + Type + Const.DIV + model.ClientID + Const.DIV + sb.ToString());
                        }
                    }
                    else
                    {
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(Model.serverIP, int.Parse(Model.serverPort));
                        NetworkStream networkStream = new NetworkStream(socket);
                        StreamReader sr = new StreamReader(networkStream);
                        StreamWriter sw = new StreamWriter(networkStream);
                        sockets.Add(socket);


                        foreach (Model model in models)
                        {
                            if (model.streamWriter == null)
                            {
                                model.streamWriter = sw;
                                model.streamWriter.AutoFlush = true;
                            }
                            if (model.streamReader == null)
                            {
                                model.streamReader = sr;
                            }
                            StringBuilder sb = new StringBuilder();
                            foreach (String s in model.ResourceDescripton)
                            {
                                sb.Append(s);
                                sb.Append(';');
                            }
                            model.streamWriter.WriteLine(Const.CONNECT + Const.DIV + Type + Const.DIV + model.ClientID + Const.DIV + sb.ToString());

                        }
                    }
                }
                if (readThread == null)
                {
                    readThread = new Thread(new ThreadStart(readStream));
                }
                readThread.Start();

                Running = true;

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
                    models[0].streamWriter.AutoFlush = false;
                    foreach (Model model in models)
                    {
                        counter++;
                       
                        if (Type.Equals(clientType.NODE))
                        {
                            model.streamWriter.WriteLine(Const.DISCONNECT + Const.DIV + model.ClientID);                            
                            AdminTools.eventLog.WriteEntry("NODE " + model.ClientID + " disconnected ", EventLogEntryType.Information);
                            if (counter == models.Count)
                            {
                                model.streamWriter.WriteLine(Const.DISCONNECT);
                                AdminTools.eventLog.WriteEntry("MW  disconnected ", EventLogEntryType.Information);
                            }
                        }
                        else
                        {
                            model.streamWriter.WriteLine(Const.DISCONNECT);
                            AdminTools.eventLog.WriteEntry("OBJECT " + model.ClientID + " disconnected ", EventLogEntryType.Information);
                        }

                       
                    }
                }
                catch (IOException e)
                {
                    AdminTools.eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
                }
                models[0].streamWriter.Flush();
                Running = false;
                if (readThread != null)
                    readThread.Abort();

                Dispose();
            }
            return true;
        }
        #endregion

        private void readStream()
        {
            try
            {
                string tmp;
                while (Running)
                {
                    if (Type.Equals(clientType.OBJECT))
                    {


                        foreach (IPobicosModel model in models)
                        {
                            tmp = model.streamReader.ReadLine();
                            if (!handleCommand(model, tmp))
                            {
                                AdminTools.eventLog.WriteEntry("Command not supported: " + tmp, EventLogEntryType.Error);
                            }
                        }
                    }
                    else
                    {
                        tmp = models[0].streamReader.ReadLine();
                        if (!handleCommand(null, tmp))
                        {
                            AdminTools.eventLog.WriteEntry("Command not supported: " + tmp, EventLogEntryType.Error);
                        }
                    }
                    Thread.Sleep(500);


                }
            }
            catch (ThreadAbortException)
            {
                AdminTools.eventLog.WriteEntry("Reading finished", EventLogEntryType.Information);

            }

            catch (Exception e)
            {
                AdminTools.eventLog.WriteEntry("Reading error: " + e.Message, EventLogEntryType.Error);
            }

        }

        bool handleCommand(IModel mdl, String command)
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
                        /* if (commandArgs.Status.Equals(Const.OFF) )
                            Dispose();*/
                        commandCorrect = true;
                        break;
                    case Const.INSTR:
                        commandArgs.Command = Const.INSTR;
                        commandArgs.NodeId = parts[1].Split('#')[0];
                        commandArgs.CallID = parts[1].Split('#')[1];
                        commandArgs.InstructionLabel = parts[2];
                        commandArgs.Params = parts[3];                        
                        (mdl as IPobicosModel).Instruction(commandArgs.InstructionLabel, commandArgs.CallID, commandArgs.Params);
                        
                        /*foreach (IPobicosModel model in models)
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
                        foreach (IPobicosModel model in models)
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
                if (commandCorrect && commandReceived != null)
                    commandReceived(this, commandArgs);
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

        #region IPobicosController Members


        public void RegisterModel(PobicosLibrary.IModel model)
        {
            models.Add(model);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            foreach (IPobicosModel m in models)
            {
                m.streamReader.Close();
                m.streamWriter.Close();
            }
            foreach (Socket s in sockets)
            {
                //s.Shutdown(SocketShutdown.Both);
                s.Close();
            }
        }

        #endregion
        #region IInstrEvents Members

        public void Instruction(IPobicosModel sender, InstructionsList instruction, string callID, string parameters)
        {
            string tmp = callID;
            if (callID == null)
                callID = sender.GetHashCode().ToString();
            sender.streamWriter.WriteLine(Const.INSTR + Const.DIV + sender.ClientID + Const.HASH + tmp + Const.DIV + instruction + Const.DIV + "(" + parameters + ")");
        }

        public void InstructionReturn(IPobicosModel sender, string callID, string value)
        {
            string tmp = callID;
            if (tmp == null)
                tmp = sender.GetHashCode().ToString();
            sender.streamWriter.WriteLine(Const.INSTR_RET + Const.DIV + sender.ClientID + Const.HASH + tmp + Const.DIV + value);
        }

        public void Event(IPobicosView sender, EventsList evnt, string callID, string parameters)
        {
            try
            {
                string tmp = callID;
                if (callID == null)
                    callID = sender.GetHashCode().ToString();
                sender.Model.streamWriter.WriteLine(Const.EVENT + Const.DIV + (sender.Model as IPobicosModel).ClientID + Const.HASH + tmp + Const.DIV + evnt + Const.DIV + "(" + parameters + ")");
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
