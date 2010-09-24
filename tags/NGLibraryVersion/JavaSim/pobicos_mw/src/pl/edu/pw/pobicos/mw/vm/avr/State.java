package pl.edu.pw.pobicos.mw.vm.avr;

/**
 * Keeps the current state of an Avr virtual machine (pointers, registers, etc.)
 * @author Micha³ Krzysztof Szczerbak
 */
public class State {
	
	/////RvAvrVMM.nc
    protected boolean running;
    //WAS: RvInterpreterStatus_t status;
    protected String status;
    protected long instr_count;
    protected int depth;	      								/// depth of calls from event handler
    protected int opcode;    									/// current instruction
    protected int _PC;       									/// current instruction pointer
    protected int _SP;       									/// stack pointer
    protected byte _SREG;      									/// status register
    protected byte regs[] = new byte[Properties.AVR_NUM_REGS]; 	/// registers
}
