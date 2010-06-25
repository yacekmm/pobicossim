using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PobicosLibrary
{
    /// <summary>
    /// passes arguments from commands (used in Client class)
    /// </summary>
    public class CommandArgs : EventArgs
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public CommandArgs() {} 
        /// <summary>
        /// command name
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// command status
        /// </summary>
        public string Status { get; set; }  
        /// <summary>
        /// instruction label
        /// </summary>
        public string InstructionLabel { get; set; }
        /// <summary>
        /// command params
        /// </summary>
        public string Params { get; set; }
        /// <summary>
        /// command id
        /// </summary>
        public string CallID { get; set; }
                
    }
}
