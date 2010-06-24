using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PobicosLibrary
{
    public class CommandArgs : EventArgs
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public CommandArgs() {}        
        public string Command { get; set; }
        public string Status { get; set; }
        public DataSet nodeDefinition { get; set; }
        public string InstructionLabel { get; set; }
        public string Params { get; set; }
        public string CallID { get; set; }
        public string NodeId { get; set; }        
    }
}
