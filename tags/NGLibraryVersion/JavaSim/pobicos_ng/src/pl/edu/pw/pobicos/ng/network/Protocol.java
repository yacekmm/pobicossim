package pl.edu.pw.pobicos.ng.network;

public class Protocol {

    static final String CONNECT = "CONNECT";
    static final String DISCONNECT = "DISCONNECT";
    static final String LINK_STATUS = "LINK_STATUS";
	static final String OFF = "OFF";
	static final String ON = "ON";
    static final String EVENT = "EVENT";
    static final String EVENT_RETURN = "EVENT_RETURN";
    static final String INSTR = "INSTR";
    static final String INSTR_RETURN = "INSTR_RETURN";
    static final String RETURN = "RETURN";
    static final String NULL = "null";
    static final char div = '$';

    //TO BE DELETED:
    static final String HELLO = "HELLO";    
    
    //DELETED:
    //static final String STOP = "STOP";
    //static final String DESCRIBE = "DESCRIBE";    
    //static final String BYE = "BYE";
}
