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

        public bool load()
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

        public void startWorking()
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
        public void stopWorking()
        {
            working = false;
            if (thread != null)
                thread.Abort();
        }

        public bool connect()
        {
            return client.Connect();
        }

        public bool Disconnect()
        {
            return client.Disconnect();
        }

    }
}
