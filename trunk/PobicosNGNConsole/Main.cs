using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PobicosLibrary;

namespace PobicosNGNConsole
{
    class Main
    {
        
        public Main()
        {
            
           // PobicosClassLibrary.Env env = AdminTools.deserialize("list.xml");
            Client client = new Client();
            Console.WriteLine(Environment.CurrentDirectory);

            client.commandReceived += new Client.CommandReceivedEventHandler(client_commandReceived);
            client.Connect();
           
            

        }

        void client_commandReceived(object sender, CommandArgs args)
        {
            AdminTools.eventLog.WriteEntry("Otrzymano komendę: " + args.Command,EventLogEntryType.Information);
        }



        void eventLog_EntryWritten(object sender, EntryWrittenEventArgs e)
        {
            Console.WriteLine(e.Entry.EntryType.ToString() + " : " + e.Entry.Message);
        }


    }
}
