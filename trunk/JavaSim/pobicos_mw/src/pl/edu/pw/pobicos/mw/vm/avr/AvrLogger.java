package pl.edu.pw.pobicos.mw.vm.avr;

import org.apache.log4j.*;
import pl.edu.pw.pobicos.mw.agent.AbstractAgent;

/**
 * Logs state of a virtual machine and simple messages.
 * @author Micha³ Krzysztof Szczerbak
 */
public class AvrLogger {
	
	private final Logger LOG = Logger.getLogger(this.getClass());
	
	private State state;

	/**
	 * Constructor.
	 * @param state virtual machine's state object
	 */
	public AvrLogger(State state)
	{
		this.state = state;
	}
	
	/**
	 * Prints registers.
	 */
	protected void print_regs()
	{
		logMessage("0" + "(" + state.regs[0] + ") " +
	    		"1" + "(" + state.regs[1] + ") " +
	    		"2" + "(" + state.regs[2] + ") " +
	    		"3" + "(" + state.regs[3] + ") " +
	    		"4" + "(" + state.regs[4] + ") " +
	    		"5" + "(" + state.regs[5] + ") " +
	    		"6" + "(" + state.regs[6] + ") " +
	    		"7" + "(" + state.regs[7] + ") " +
	    		"8" + "(" + state.regs[8] + ") " +
	    		"9" + "(" + state.regs[9] + ") " +
	    		"10" + "(" + state.regs[10] + ") " +
	    		"11" + "(" + state.regs[11] + ") " +
	    		"12" + "(" + state.regs[12] + ") " +
	    		"13" + "(" + state.regs[13] + ") " +
	    		"14" + "(" + state.regs[14] + ") " +
	    		"15" + "(" + state.regs[15] + ") " + 
	    		"16" + "(" + state.regs[16] + ") " +
	    		"17" + "(" + state.regs[17] + ") " +
	    		"18" + "(" + state.regs[18] + ") " +
	    		"19" + "(" + state.regs[19] + ") " +
	    		"20" + "(" + state.regs[20] + ") " +
	    		"21" + "(" + state.regs[21] + ") " +
	    		"22" + "(" + state.regs[22] + ") " +
	    		"23" + "(" + state.regs[23] + ") " +
	    		"24" + "(" + state.regs[24] + ") " +
	    		"25" + "(" + state.regs[25] + ") " +
	    		"26" + "(" + state.regs[26] + ") " +
	    		"27" + "(" + state.regs[27] + ") " +
	    		"28" + "(" + state.regs[28] + ") " +
	    		"29" + "(" + state.regs[29] + ") " +
	    		"30" + "(" + state.regs[30] + ") " +
	    		"31" + "(" + state.regs[31] + ")\n"
	    );
	}
	
	/**
	 * Prints agent's data space.
	 * @param agent micro-agent
	 */
	protected void print_data(AbstractAgent agent)
	{
		String log = "data = ";
		for(int i = 0; i < agent.getData().data.length; i++)
			log += i + ":" + agent.getData().data[i] + ";";
		logMessage(log);
	}

	/**
	 * Prints special registers.
	 */
	protected void print_state()
	{
		logMessage("state._PC = " + Integer.toHexString(state._PC) + 
				" state._SP = " + Integer.toHexString(state._SP) + 
				" state._SREG = " + Integer.toHexString(state._SREG) + "\n"
				);
	}

	/**
	 * Prints instruction's decoded name.
	 * @param str instruction's name
	 */
	protected void copyInstrName(String str)
	{
		logMessage("******* opcode: " + str);
	}

    /**
     * Prints error.
     * @param extraInfo integer
     */
    protected void logError(int extraInfo) 
    {
    	logMessage("ERROR : " + extraInfo);
    }

    /**
     * Prints error.
     * @param extraInfo string
     */
    protected void logError(String extraInfo) 
    {
    	logMessage("ERROR : " + extraInfo);
    }

	/**
	 * Prints any message.
	 * @param str message
	 */
	protected void logMessage(String str)
	{
		LOG.debug(str);
	}
}
