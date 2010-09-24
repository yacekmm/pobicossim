package pl.edu.pw.pobicos.mw.vm.avr;

import org.apache.log4j.Logger;

/**
 * Thread interpreting each opcode one after another.
 * @author Micha³ Krzysztof Szczerbak
 */
public class Processor implements Runnable{

	private boolean nextStep = false, stepByStep = false;
	
	private State state;
	
	private AvrVM parent;
	
	private Thread t;
	
	private final Logger LOG = Logger.getLogger(this.getClass());

	/**
	 * Constructor.
	 * @param state virtual machine's state
	 * @param parent virtual machine
	 */
	public Processor(State state, AvrVM parent)
	{
		this.state = state;
		this.parent = parent;
	}
	
	/**
	 * Sets the initial environment and starts a thread of a virtual processor.
	 */
	public void start()
	{
		nextStep = true;		
        state.depth = 0;
        state.running = true;
        state.status = "ST_OK";
        parent.reset_mcu(); 
		state._PC = (char) (parent.tasks.get(0).addr >> 1);
		LOG.info("RvAvrVM: execution started\n");
        if(Properties.VM_MAX_INSTRUCTIONS > 0)
        	state.instr_count = 0;        
        t = new Thread(this);
        t.start();
	}
	
	/* 
	 * Processes every opcode and verifies the correctness.
	 * @see java.lang.Runnable#run()
	 */
	public void run()
	{
        while (state.running) {
        	if(nextStep)
        	{
        		state.opcode = parent.getCurrAgent().getCode().get16((char)(state._PC << 1));
        		if (parent.single_step() != 0) {
	                state.status = "ST_INSTR";
	                parent.log.logError(state.opcode);
	                state.running = false;
	                continue;
	            }
	            if(Properties.VM_MAX_INSTRUCTIONS > 0)
	                if (++state.instr_count == Properties.VM_MAX_INSTRUCTIONS) {
	                    state.status = "ST_TIMEOUT";
	                    parent.log.logError(0xffff);
	                    state.running = false;
	                    continue;
	                }
	            if(stepByStep)
	            	nextStep = false;
        	}
        }
        LOG.info("RvAvrVM: execution ended\n" + state.status + "\n");
	    if(Properties.VM_MAX_INSTRUCTIONS > 0)
	    	LOG.debug("RvAvrVM: " + /*(unsigned)*/state.instr_count + " state.OPCODEs(s) interpreted\n");
        if (state.status == "ST_OK" && (state._SP != Properties.DATA_MEM_SIZE + Properties.AVR_REG_IO_SIZE - 1))
            state.status = "ST_STACK";
        parent.RvVMI_stop();
	}

	/**
	 * Fires next step in a step-by-step processing.
	 */
	public void nextStep()
	{
		nextStep = true;
	}
}
