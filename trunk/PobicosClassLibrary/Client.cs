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
        public String Type = Const.OBJECT;
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
            AdminTools.eventLog.WriteEntry("Utworzono klienta sieciowego", EventLogEntryType.Information);
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
                    foreach (Model model in models)
                    {
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(Model.serverIP, int.Parse(Model.serverPort));
                        // if (socket.Connected)
                        //    eventLog.WriteEntry("Podłączono Model: " + Model.Id + " do Serwera.", EventLogEntryType.Information);

                        initializeNetwork(socket, model);                       
                        StringBuilder sb = new StringBuilder();
                        foreach (String s in model.resourceDescripton)
                        {
                            sb.Append(s);
                            sb.Append(';');
                        }
                        model.streamWriter.WriteLine(Const.CONNECT + Const.DIV + Type + Const.DIV + model.Id + Const.DIV + sb.ToString());
                        //  streamWriter.Flush();
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
                AdminTools.eventLog.WriteEntry("Błąd połączenia, host: " + Model.serverIP + ", port: " + Model.serverPort, EventLogEntryType.Error);
                return false;

            }


        }

        public bool Disconnect()
        {
            if (Running)
            {
                foreach (Model model in models)
                {

                    model.streamWriter.WriteLine(Const.DISCONNECT);// + Const.DIV + model.Id);
                    AdminTools.eventLog.WriteEntry("Klient " + model.Id + " odłączony ", EventLogEntryType.Information);


                }
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


                    foreach (IPobicosModel model in models)
                    {
                        tmp = model.streamReader.ReadLine();
                        if (!handleCommand(tmp))
                        {
                            // throw new Exception("Komenda nie obsługiwana : " + tmp);
                        }
                        // eventLog.WriteEntry("Odebrano: " + tmp, EventLogEntryType.Information);
                    }
                    Thread.Sleep(500);


                }
            }
            catch (ThreadAbortException)
            {
                AdminTools.eventLog.WriteEntry("Zakończono odczyt", EventLogEntryType.Information);

            }

            catch (Exception e)
            {
                AdminTools.eventLog.WriteEntry("Błąd odczytu: " + e.Message, EventLogEntryType.Error);
            }

        }

        bool handleCommand(String command)
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
                   /*     commandArgs.command = Const.HELLO;
                        commandArgs.arg1 = parts[1];
                        StringReader sr = new StringReader(parts[2]);
                        DataSet ds = new DataSet();
                        ds.ReadXml(sr);
                        commandArgs.nodeDefinition = ds;*/
                        commandCorrect = true;
                        break;
                    case Const.BYE:
                        commandArgs.command = Const.BYE;
                        commandArgs.arg1 = parts[1];
                        commandCorrect = true;
                        break;
                    case Const.STOP:
                        commandArgs.command = Const.STOP;
                        Disconnect();
                        commandCorrect = true;
                        break;
                    #endregion
                    case Const.LINK_STATUS:
                        commandArgs.command = Const.LINK_STATUS;
                        commandArgs.arg1 = parts[1];
                        /* if (commandArgs.arg1.Equals(Const.OFF) )
                            Dispose();*/
                        commandCorrect = true;
                        break;
                    case Const.INSTR:
                        commandArgs.command = Const.INSTR;
                        commandArgs.originatorId = parts[1].Split('#')[0];
                        commandArgs.callID = parts[1].Split('#')[1];
                        commandArgs.arg2 = parts[2];
                        commandArgs.arg3 = parts[3];
                        foreach (IPobicosModel model in models)
                        {
                            if (model.Id.Equals(commandArgs.originatorId))
                            { 
                                model.Instruction((InstructionsList)Enum.Parse(typeof(InstructionsList), commandArgs.arg2),commandArgs.callID, commandArgs.arg3);
                            }
                        }
                        commandCorrect = true;
                        break;
                    case Const.EVENT:
                        commandArgs.command = Const.EVENT;
                        commandArgs.arg1 = parts[1];
                        commandArgs.arg2 = parts[2];
                        commandArgs.arg3 = parts[3];
                        break;
                    case Const.INSTR_RET:
                        commandArgs.command = Const.INSTR_RET;
                        commandArgs.arg1 = parts[1].Split('#')[0];
                        commandArgs.arg2 = parts[2];
                        commandArgs.arg3 = parts[1].Split('#')[1];
                        foreach (IPobicosModel model in models)
                        {
                            if (model.Id.Equals(commandArgs.arg1))
                            {
                                model.InstructionReturn(commandArgs.arg3, commandArgs.arg2);
                            }
                        }
                        commandCorrect = true;
                        break;
                    case Const.EVENT_RET:
                        commandArgs.command = Const.EVENT_RET;
                        commandArgs.arg1 = parts[1];
                        commandArgs.arg2 = parts[2];
                        commandArgs.arg3 = parts[3];
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
            sender.streamWriter.WriteLine(Const.INSTR + Const.DIV + sender.Id + Const.HASH + tmp + Const.DIV + instruction + Const.DIV + "(" + parameters + ")");
        }

        public void InstructionReturn(IPobicosModel sender, string callID, string value)
        {
            string tmp = callID;
            if (tmp == null)
                tmp = sender.GetHashCode().ToString();
            sender.streamWriter.WriteLine(Const.INSTR_RET + Const.DIV + sender.Id + Const.HASH + tmp + Const.DIV + value);
        }

        public void Event(IPobicosView sender, EventsList evnt, string callID, string parameters)
        {
            string tmp = callID;
            if (callID == null)
                callID = sender.GetHashCode().ToString();
            sender.Model.streamWriter.WriteLine(Const.EVENT + Const.DIV + sender.Model.Id + Const.HASH + tmp + Const.DIV + evnt + Const.DIV + "(" + parameters + ")");
        }

        public void EventReturn(IPobicosView sender, string callID, string value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


}
