using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PobicosLibrary
{
    public class CommandArgs : EventArgs
    {
        public CommandArgs()
        {
            
        }
        public string command { get; set; }
        public string arg1 { get; set; }
        public DataSet nodeDefinition { get; set; }

        public string arg2 { get; set; }
        public string arg3 { get; set; }
        public string callID { get; set; }
        public string originatorId { get; set; }
        
    }
}
