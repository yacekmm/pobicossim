using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PobicosLibrary
{
    /// <summary>
    /// The set of const for library
    /// </summary>
    public class Const
    {

        #region Protocol 1.0
        //public const String HELLO = "HELLO";
        //public const String BYE = "BYE";
        //public const String DESCRIBE = "DESCRIBE";
        //public const String RETURN = "RETURN";
        #endregion
        #region Protocol 1.1
        //public const String STOP = "STOP";
        /// <summary>
        /// RAP Connect command
        /// </summary>
        public const String CONNECT = "CONNECT";
        /// <summary>
        /// RAP Disconnect command
        /// </summary>
        public const String DISCONNECT = "DISCONNECT";

        /// <summary>
        /// RAP Event command
        /// </summary>
        public const String EVENT = "EVENT";
        /// <summary>
        /// RAP Instruction command
        /// </summary>
        public const String INSTR = "INSTR";

        /// <summary>
        /// Null const
        /// </summary>
        public const String NULL = "null";
        /// <summary>
        /// RAP separator
        /// </summary>
        public const char DIV = '$';
        /// <summary>
        /// RAP separator
        /// </summary>
        public const char HASH = '#';




        /// <summary>
        /// RAP Instruction return command
        /// </summary>
        public const String INSTR_RET = "INSTR_RETURN";
        /// <summary>
        /// RAP Event return command
        /// </summary>
        public const String EVENT_RET = "EVENT_RETURN";
        /// <summary>
        /// RAP Link status command
        /// </summary>
        public const String LINK_STATUS = "LINK_STATUS";
        /// <summary>
        /// RAP Link status on
        /// </summary>
        public const String ON = "ON";
        /// <summary>
        /// RAP Link status off
        /// </summary>
        public const String OFF = "OFF";
        #endregion

        /// <summary>
        /// RAP Node
        /// </summary>
        public const String NODE = "NODE";
        /// <summary>
        /// RAP Object
        /// </summary>
        public const String OBJECT = "OBJECT";



    }
}
