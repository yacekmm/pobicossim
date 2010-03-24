using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PobicosLibrary;
using System.Net.Sockets;
using System.Collections;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace MiniMW
{
    public class Controller : IPobicosController
    {

        public Controller()
        {
        }
        public ArrayList models = new ArrayList();
        String IP = "localhost";
        int port = 40007;
        #region IController Members
        public bool Connect()
        {


            try
            {
                for (int i = 0; i < models.Count; i++)
                {
                    IPobicosModel model =(IPobicosModel) models[i];
                    
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(IP, port);
                    NetworkStream networkStream = new NetworkStream(socket);
                    model.streamWriter = new StreamWriter(networkStream);
                    model.streamWriter.AutoFlush = true;
                    model.streamReader = new StreamReader(networkStream);
                    
                    model.streamWriter.WriteLine(Const.CONNECT + Const.div + "NODE" + Const.div + model.Id + Const.div + AdminTools.RemoveWhitespace(model.Definition.GetXml()));
                    //  Thread readThread = new Thread(new ThreadStart(readStream));

                }
                return true;
               
            }
            catch (Exception)
            {
                AdminTools.eventLog.WriteEntry("Błąd połączenia, host: " + Model.serverIP + ", port: " + Model.serverPort, EventLogEntryType.Error);
                return false;
            }

        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public bool Running
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IPobicosController Members



        public void InstructionReturn(object sender, string value)
        {
            throw new NotImplementedException();
        }

        public void RegisterModel(IPobicosModel model)
        {
            throw new NotImplementedException();
        }

        public void RegisterView(IPobicosView view)
        {
            throw new NotImplementedException();
        }

        #endregion



        #region IPobicosController Members


        public void Instruction(IPobicosModel sender, InstructionsList instruction, string parameters)
        {
            sender.streamWriter.WriteLine(Const.INSTR + Const.div +sender.Id + Const.HASH + sender.GetHashCode() + Const.div + instruction + Const.div + "(" + parameters + ")");
        }

        public void Event(IPobicosView sender, EventsList evnt, string parameters)
        {
            sender.Model.streamWriter.WriteLine(Const.EVENT + Const.div + sender.Model.ClientId + Const.HASH + sender.GetHashCode() + Const.div + evnt + Const.DIV + "(" + parameters + ")");
        }

        #endregion
    }
}
