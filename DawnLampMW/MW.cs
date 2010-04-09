using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PobicosLibrary;
using System.Threading;

namespace DawnLampMW
{
    public class MW
    {
        private Client client = new Client();
        private String[] files = { "dawnDetector.xml", "lamp.xml" };
        private bool working = false;
        private Random random = new Random(5444);
        private Thread thread;

        public bool load()
        {
            AdminTools.eventLog.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(eventLog_EntryWritten);
            client.commandReceived += new Client.CommandReceivedEventHandler(client_commandReceived);
            client.Type = Const.NODE;
            try
            {
                foreach (String file in files)
                {
                    List<IPobicosModel> models = AdminTools.readConfiguration(file);
                    if (models == null)
                    {
                        AdminTools.eventLog.WriteEntry("Error in XML file", System.Diagnostics.EventLogEntryType.Error);
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
                AdminTools.eventLog.WriteEntry(e.Message, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
            AdminTools.eventLog.WriteEntry("Init complete", System.Diagnostics.EventLogEntryType.Information);
            return true;
        }

        void client_commandReceived(object sender, CommandArgs args)
        {
                if (args.command == Const.INSTR_RET)
            {
                if (int.Parse(args.arg2) > 200)
                    client.Instruction((IPobicosModel) client.models[1], InstructionsList.pongiSwitchOff, random.Next(11110).ToString(), null);
                else
                    client.Instruction((IPobicosModel) client.models[1], InstructionsList.pongiSwitchOn, random.Next(11110).ToString(), null);

            }
        }

        void eventLog_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {
            Console.WriteLine(e.Entry.Message);
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
                    client.Instruction((IPobicosModel)client.models[0], InstructionsList.pongiGetBrightness, random.Next(1000000).ToString(), null);
                    Thread.Sleep(2000);
                }
            }
            catch (ThreadAbortException)
            {
                this.Disconnect();
                AdminTools.eventLog.WriteEntry("Work thread finished", System.Diagnostics.EventLogEntryType.Information);
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
            return client.Disconnect(true);
        }

    }
}
