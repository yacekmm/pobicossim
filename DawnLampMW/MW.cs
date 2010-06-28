using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PobicosLibrary;
using System.Threading;
using System.Diagnostics;

namespace DawnLampMW
{
    public class MW
    {
        private Client client = new Client();
        private String[] files = { "dawnDetector.xml", "lamp.xml" };
        private bool working = false;
        private Random random = new Random(DateTime.Now.Millisecond);
        private Thread thread;

        /// <summary>
        /// Loads configuration from config files
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            //AdminTools.eventLog.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(eventLog_EntryWritten);
            client.CommandReceived += new Client.CommandReceivedEventHandler(client_commandReceived);
            client.Type = clientType.NODE;
            try
            {
                foreach (String file in files)
                {
                    List<IPobicosModel> models = AdminTools.ReadConfiguration(file);
                    if (models == null)
                    {
                        Trace.TraceError("Error in XML file");
                     //   AdminTools.eventLog.WriteEntry("Error in XML file", System.Diagnostics.EventLogEntryType.Error);
                        return false;
                    }
                    foreach (IPobicosModel model in models)
                    {
                        client.RegisterModel(model);
                    }

                }
            }
            catch (Exception e)
            {
                //AdminTools.eventLog.WriteEntry(e.Message, System.Diagnostics.EventLogEntryType.Error);
                Trace.TraceError(e.Message);
                return false;
            }
            Trace.TraceInformation("Init complete");
          //  AdminTools.eventLog.WriteEntry("Init complete", System.Diagnostics.EventLogEntryType.Information);
            return true;
        }

        void client_commandReceived(object sender, CommandArgs args)
        {
                if (args.Command == Const.INSTR_RET)
            {
                if (int.Parse(args.InstructionLabel) > 200)
                    client.Instruction((IPobicosModel) client.Models[1], InstructionsList.SwitchOff, random.Next(10000).ToString(), null);
                else
                    client.Instruction((IPobicosModel) client.Models[1], InstructionsList.SwitchOn, random.Next(10000).ToString(), null);

            }
        }

        void eventLog_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {
            Console.Write(e.Entry.Message);
        }
        /// <summary>
        /// Makes application to start listening for and sending messages
        /// </summary>
        public void StartWorking()
        {
            if (working) return;            
            thread = new Thread(new ThreadStart(this.work));
            working = true;
            thread.Start();

            

        }
        private void work()
        {
            try
            {
                while (working)
                {
                    client.Instruction((IPobicosModel)client.Models[0], InstructionsList.GetBrightness, random.Next(10000).ToString(), null);
                    Thread.Sleep(2500);
                }
            }
            catch (ThreadAbortException)
            {
                this.Disconnect();
                Trace.TraceInformation("Work thread finished");
               // AdminTools.eventLog.WriteEntry("Work thread finished", System.Diagnostics.EventLogEntryType.Information);
                return;
            }
        }
        /// <summary>
        /// Makes application to stop listening for and sending messages
        /// </summary>
        public void StopWorking()
        {
            working = false;
            if (thread != null)
                thread.Abort();
        }
        /// <summary>
        /// Connects appliation to SS
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            return client.Connect();
        }
        /// <summary>
        /// Disconnects application from MW
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            return client.Disconnect();
        }

    }
}
