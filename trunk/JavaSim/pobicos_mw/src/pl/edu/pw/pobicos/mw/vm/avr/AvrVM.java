package pl.edu.pw.pobicos.mw.vm.avr;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import java.util.List;
import java.util.ArrayList;

import org.apache.log4j.Logger;

/**
 * AVR virtual machine module.
 * @author Micha³ Krzysztof Szczerbak
 */
public class AvrVM {	
	
	private final Logger LOG = Logger.getLogger(this.getClass());
	
	/************************************ VARS *******************************************/
	private State state = new State();
	
	private Implementation implementation = new Implementation(state, this);
	
	private Processor processor = new Processor(state, this);
	
	protected AvrLogger log = new AvrLogger(state);

	protected List<Task> tasks = new ArrayList<Task>();
	
	protected class Task
	{
		int addr;
		AbstractAgent a;
	}
	
	/**
	 * Initiates a virtual machine.
	 */
	public void init() {
		LOG.info("RvAvrVM: initializing");
    	
        reset_mcu();    // zeros registers and I/O space

        state.status = "ST_OK";
        state.running = false;
        state.opcode = 0x0000;
    }

    /**
     * Clears the registers.
     */
    protected void reset_mcu() {
        for (int i = 0; i < Properties.AVR_NUM_REGS; i++)
            state.regs[i] = 0;
        state._SP = (char) (Properties.DATA_MEM_SIZE + Properties.AVR_REG_IO_SIZE - 1);
        state._SREG = 0x00;
    }
    
    /**
     * Sets a new task to be processed if there is task waiting in a queue and virtual machine is ready.
     */
    private void checkTasks()
    {
    	if(!state.running && !tasks.isEmpty())
    	{
    		LOG.info("task set");
    		RvVMI_run();
    	}
    }
    
    /**
     * Inserts a new task into a queue and fires it if virtual machine is not occupied.
     * @param a agent firing a task
     * @param addr address in code of the task
     */
    public void setTask(AbstractAgent a, int addr)
	{
    	LOG.info("task proposed " + addr + " @" + a);
		Task newTask = new Task();
		newTask.a = a;
		newTask.addr = addr;
		tasks.add(newTask);
		checkTasks();
	}
    
    /**
     * Starts processing a task.
     */
    public void RvVMI_run() 
    {
    	LOG.info("task fired");
    	processor.start();
    }

    /**
     * Orders a processor to perform next step.
     */
    public void nextStep()
    {
    	processor.nextStep();
    }

    /**
     * Stops a virtual machine and checks if other tasks wait in queue.
     */
    public void RvVMI_stop() 
    {
        state.status = "ST_EXIT";
        state.running = false;
        tasks.remove(0);
        checkTasks();
    }
    
	/**
	 * Processes one opcode instruction. Invokes an appropriate implementation method.
	 * @return 0 if processing succeeded
	 */
	protected short single_step()
	{
	    short op;
	    int OPCODE = state.opcode;

	    if (((OPCODE & 0x0f) == 0) && ((OPCODE & 0xf000) >> 12 == 9) && ((OPCODE & 0x0e00) >> 9 == 0))
	        implementation.OP_LDS();
	    else if (((OPCODE & 0x0f) == 0) && ((OPCODE & 0xf000) >> 12 == 9) && ((OPCODE & 0x0e00) >> 9 == 1))
	        implementation.OP_STS();
	    else if (((OPCODE >> 14) == 2) && ((OPCODE & 0x1000) >> 12 == 0)) 
	    {
	        if ((OPCODE & 0x0200) >> 9 == 0) 
	        {
	            if ((OPCODE & 8) >> 3 == 1)
	                implementation.OP_LDD_Y();
	            else if ((OPCODE & 8) >> 3 == 0)
	                implementation.OP_LDD_Z();
	        } 
	        else if ((OPCODE & 0x0200) >> 9 == 1) 
	        {
	            if ((OPCODE & 8) >> 3 == 1)
	                implementation.OP_STD_Y();
	            else if ((OPCODE & 8) >> 3 == 0)
	                implementation.OP_STD_Z();
	        }
	    }
	    else if ((OPCODE & 0xfe0e) == 0x940e) 
	    {
	         implementation.OP_CALL();
	    }
	    else if ((OPCODE & 0xfe0c) == 0x940C) 
	    {
	        implementation.OP_JMP();
	    }
	    else if (((OPCODE & 0xfc00) >> 10) == 0x0027) 
	    {
	        implementation.OP_MUL();
	    }
	    else if ((OPCODE & 0xfe00) == 0x0200) 
	    {
	        int tmp = OPCODE & 0x0100;
	        if (tmp == 0) 
	        {
	            implementation.OP_MULS();
	        } 
	        else 
	        {
	            switch (OPCODE & 0x88) 
	            {
	                case 0x0:
	                    implementation.OP_MULSU();
	                    break;
	                case 0x8:
	                    implementation.OP_FMUL();
	                    break;
	                case 0x80:
	                    implementation.OP_FMULS();
	                    break;
	                case 0x88:
	                    implementation.OP_FMULSU();
	                    break;
	            }
	        }
	    } 
	    else 
	    {
	        switch (OPCODE & 0x0000ffff) 
	        {
	            case 0x0000:
	                implementation.OP_NOP();
	                break;
	            case 0x9409:
	                implementation.OP_IJMP();
	                break;
	            case 0x9509:
	                implementation.OP_ICALL();
	                break;
	            case 0x9508:
	                implementation.OP_RET();
	                break;
	            case 0x9518:
	                implementation.OP_RETI();
	                break;
	            case 0x9588:
	                implementation.OP_SLEEP();
	                break;
	            case 0x95a8:
	                implementation.OP_WDR();
	                break;
	            case 0x95c8:
	                implementation.OP_LPM();
	                break;
	                // end

	            case 0x9408:
	            case 0x9418:
	            case 0x9428:
	            case 0x9438:
	            case 0x9448:
	            case 0x9458:
	            case 0x9468:
	            case 0x9478:
	                implementation.OP_BSET();
	                break;
	            case 0x9488:
	            case 0x9498:
	            case 0x94a8:
	            case 0x94b8:
	            case 0x94c8:
	            case 0x94d8:
	            case 0x94e8:
	            case 0x94f8:
	                implementation.OP_BCLR();
	                break;

//	            default:
//	                state._PC++;
	                // end
	        }
	        op = (short)((OPCODE & 0x0000ffff) >> 12);
	        switch (op) 
	        {
	            case 0x0e:
	                implementation.OP_LDI();
	                break;
	            case 9:
	                op = (short)((OPCODE & 0x0e00) >> 9);
	                switch (op) 
	                {
	                    case 0:
	                        op = (short)(OPCODE & 0x0f);
	                        switch (op) 
	                        {
	                            case 0x4:
	                            case 0x5:
	                                implementation.OP_LPM();
	                                break;
	                            // end

	                            case 0x0c:
	                                implementation.OP_LDX();
	                                break;
	                            case 0x0d:
	                                implementation.OP_LDX_ADD();
	                                break;
	                            case 0x0e:
	                                implementation.OP_SUB_LDX();
	                                break;
	                            case 0x9:
	                                implementation.OP_LDY_ADD();
	                                break;
	                            case 0x0a:
	                                implementation.OP_SUB_LDY();
	                                break;
	                            case 1:
	                                implementation.OP_LDZ_ADD();
	                                break;
	                            case 2:
	                                implementation.OP_SUB_LDZ();
	                                break;
	                            case 0x0f:
	                                implementation.OP_POP();
	                                break;
	                        }
	                        break;
	                    case 1:
	                        op = (short)(OPCODE & 0x0f);
	                        switch (op) 
	                        {
	                            case 0x0c:
	                                implementation.OP_STX();
	                                break;
	                            case 0x0d:
	                                implementation.OP_STX_ADD();
	                                break;
	                            case 0x0e:
	                                implementation.OP_SUB_STX();
	                                break;
	                            case 9:
	                                implementation.OP_STY_ADD();
	                                break;
	                            case 0x0a:
	                                implementation.OP_SUB_STY();
	                                break;
	                            case 1:
	                                implementation.OP_STZ_ADD();
	                                break;
	                            case 2:
	                                implementation.OP_SUB_STZ();
	                                break;
	                            case 0x0f:
	                                implementation.OP_PUSH();
	                                break;
	                        }
	                        break;
	                    case 2:
	                        op = (short)(OPCODE & 0x0f);
	                        switch (op) 
	                        {
	                            case 3:
	                                implementation.OP_INC();
	                                break;
	                            case 0:
	                                implementation.OP_COM();
	                                break;
	                            case 1:
	                                implementation.OP_NEG();
	                                break;
	                            case 2:
	                                implementation.OP_SWAP();
	                                break;
	                            case 0x0a:
	                                implementation.OP_DEC();
	                                break;
	                            case 5:
	                                implementation.OP_ASR();
	                                break;
	                            case 6:
	                                implementation.OP_LSR();
	                                break;
	                            case 7:
	                                implementation.OP_ROR();
	                                break;
	                        }
	                        break;
	                    case 3:
	                        op = (short)((OPCODE & 0x0f00) >> 8);
	                        switch (op) 
	                        {
	                            case 6:
	                                implementation.OP_ADIW();
	                                break;
	                            case 7:
	                                implementation.OP_SBIW();
	                                break;
	                        }
	                        break;
	                    case 4:
	                        op = (short)((OPCODE & 0x0f00) >> 8);
	                        switch (op) 
	                        {
	                            case 9:
	                                implementation.OP_SBIC();
	                                break;
	                            case 8:
	                                implementation.OP_CBI();
	                                break;
	                        }
	                        break;
	                    case 5:
	                        op = (short)((OPCODE & 0x0f00) >> 8);
	                        switch (op) 
	                        {
	                            case 0x0b:
	                                implementation.OP_SBIS();
	                                break;
	                            case 0x0a:
	                                implementation.OP_SBI();
	                                break;
	                        }
	                        break;
	                    }
	                    break;
	                case 8:
	                op = (short)((OPCODE & 0x0e00) >> 9);
	                switch (op) {
	                    case 0:
	                        op = (short)(OPCODE & 0x0f);
	                        switch (op) {
	                            case 0:
	                                implementation.OP_LDZ();
	                                break;
	                            case 8:
	                                implementation.OP_LDY();
	                                break;
	                        }
	                        break;
	                    case 1:
	                        op = (short)(OPCODE & 0x0f);
	                        switch (op) {
	                            case 0:
	                                implementation.OP_STZ();
	                                break;
	                            case 8:
	                                implementation.OP_STY();
	                                break;
	                        }
	                        break;
	                }
	                break;
	            case 0x0b:
	                op = (short)((OPCODE & 0x800) >> 11);
	                switch (op) {
	                    case 0:
	                        implementation.OP_IN();
	                        break;
	                    case 1:
	                        implementation.OP_OUT();
	                        break;
	                }
	                break;
	            case 0:

	                if ((OPCODE & 0x0f00) == 0x100) {
	                    implementation.OP_MOVW();
	                    break;
	                }
	                // end

	                op = (short)((OPCODE & 0x0c00) >> 10);
	                switch (op) {
	                    case 3:
	                        implementation.OP_ADD();
	                        break;
	                    case 2:
	                        implementation.OP_SBC();
	                        break;
	                    case 1:
	                    	implementation.OP_C_PC();
	                        break;
	                }
	                break;
	            case 1:
	                op = (short)((OPCODE & 0x0c00) >> 10);
	                switch (op) {
	                    case 3:
	                        implementation.OP_ADC();
	                        break;
	                    case 2:
	                        implementation.OP_SUB();
	                        break;
	                    case 1:
	                        implementation.OP_CP();
	                        break;
	                    case 0:
	                        implementation.OP_CPSE();
	                        break;
	                }
	                break;
	            case 5:
	                implementation.OP_SUBI();
	                break;
	            case 4:
	                implementation.OP_SBCI();
	                break;
	            case 3:
	                implementation.OP_CPI();
	                break;
	            case 2:
	                op = (short)((OPCODE & 0x0c00) >> 10);
	                switch (op) {
	                    case 0:
	                        implementation.OP_AND();
	                        break;
	                    case 2:
	                        implementation.OP_OR();
	                        break;
	                    case 1:
	                        implementation.OP_EOR();
	                        break;
	                    case 3:
	                        implementation.OP_MOV();
	                        break;
	                }
	                break;
	            case 7:
	                implementation.OP_ANDI();
	                break;
	            case 6:
	                implementation.OP_ORI();
	                break;
	            case 0x0c:
	                implementation.OP_RJMP();
	                break;
	            case 0x0d:
	                implementation.OP_RCALL();
	                break;
	            case 0x0f:
	                op = (short)((OPCODE & 0x0c00) >> 10);
	                switch (op) {
	                    case 0:
	                        implementation.OP_BRBS();
	                        break;
	                    case 1:
	                        implementation.OP_BRBC();
	                        break;
	                    case 2:
	                        op = (short)((OPCODE & 0x200) >> 9);
	                        switch (op) {
	                            case 0:
	                                implementation.OP_BLD();
	                                break;
	                            case 1:
	                                implementation.OP_BST();
	                                break;
	                        }
	                        break;
	                    case 3:
	                        op = (short)((OPCODE & 0x200) >> 9);
	                        switch (op) {
	                            case 0:
	                                implementation.OP_SBRC();
	                                break;
	                            case 1:
	                                implementation.OP_SBRS();
	                                break;
	                        }
	                        break;
	                }

	                break;
	            default:    // unknown instruction OPCODE
	                System.out.print("Unknown OPCODE!\n");
	                state._PC++;
	                break;
	                // end
	        }
	    }
	        // end
	    return 0;
	}

    /**
     * Gets agent currently being processed.
     * @return agent
     */
    public AbstractAgent getCurrAgent()
    {
    	return (tasks.isEmpty() ? null : tasks.get(0).a);
    }
    
	/**
	 * Checks if virtual machine is running.
	 * @return true if is running, false otherwise
	 */
	public boolean isRunning()
	{
		return state.running;
	}
	
	/**
	 * Gets size of data space in bytes.
	 * @return data size
	 */
	public long getDataSize() 
	{
		return Properties.DATA_MEM_SIZE;
	}
}
