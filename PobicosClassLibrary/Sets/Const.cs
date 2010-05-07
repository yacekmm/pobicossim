using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PobicosLibrary
{
    public class Const
    {

        #region Protocol 1.0
        public const String STOP = "STOP";
        public const String CONNECT = "CONNECT";
        public const String DISCONNECT = "DISCONNECT";
        public const String HELLO = "HELLO";
        public const String BYE = "BYE";
        public const String EVENT = "EVENT";
        public const String INSTR = "INSTR";
        public const String DESCRIBE = "DESCRIBE";
        public const String RETURN = "RETURN";
        public const String NULL = "null";
        public const char DIV = '$';
        public const char HASH = '#';

        #endregion

        #region Protocol 1.1
        public const String INSTR_RET = "INSTR_RETURN";
        public const String EVENT_RET = "EVENT_RETURN";
        public const String LINK_STATUS = "LINK_STATUS";
        public const String ON = "ON";
        public const String OFF = "OFF";
        #endregion

        #region Log
        public const String logSource = "PKJM";
        public const String logName = "Application";
        #endregion

        public const String NODE = "NODE";
        public const String OBJECT = "OBJECT";



        #region Taxonomy trees
        public const String Product = "Product";
        public const String Location = "Location";
        #endregion
    }
}
